using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Purchasing.Extension;

public class PurchaseManager : SingletonMonobehaviour<PurchaseManager>, IStoreListener
{
    private IStoreController storeController;
    private IGooglePlayStoreExtensions googleExtensions;
    private ITransactionHistoryExtensions transactionHistoryExtensions;

    string productID = "";
    string purchase_id = "";
    string purchase_time = "";

    public GameObject success_purchase;

    public bool purchasing;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            var module = StandardPurchasingModule.Instance();
            var builder = ConfigurationBuilder.Instance(module);
            builder.AddProduct("omok_vip", ProductType.Subscription);
            UnityPurchasing.Initialize(this, builder);
        }
        catch (Exception e)
        {
            Debug.Log("Start builder " + e);
        }
    }

    public void check_subscription()
    {
        Debug.Log("check_subscription");
        Product product = storeController.products.WithID("omok_vip");

        if (checkIfProductIsAvailableForSubscriptionManager(product.receipt))
        {
            SubscriptionManager sub_manager = new SubscriptionManager(product, null);
            SubscriptionInfo sub_info = sub_manager.getSubscriptionInfo();

            if (sub_info.isSubscribed() == Result.True)
            {
                if (DataManager.instance.my_rating == RATING.NOMAL)
                {
                    DataManager.instance.my_rating = RATING.VIP;
                    DataManager.instance.save_my_rating_data();
                }
            }
            else
            {
                if (DataManager.instance.my_rating == RATING.VIP)
                {
                    DataManager.instance.my_rating = RATING.NOMAL;
                    DataManager.instance.save_my_rating_data();
                }
            }
        }
        else
        {
            Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
        }
    }

    public void SuccessPurchase()
    {
        Debug.Log("SuccessPurchase");

        purchasing = false;
        success_purchase.SetActive(true);

        DataManager.instance.my_rating = RATING.VIP;
        DataManager.instance.save_my_rating_data();

        if (AccountManager.instance != null)
        {
            AccountManager.instance.on_success_purchase();
        }
    }

    public void CloseSuccessPurchase()
    {
        success_purchase.SetActive(false);
    }

    public void on_purchase()
    {
        if (!purchasing)
        {
            purchasing = true;
            storeController.InitiatePurchase(storeController.products.WithID("omok_vip"));
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        bool t_IsBuy = true;

#if !UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        try
        {
            var t_Result = validator.Validate(purchaseEvent.purchasedProduct.receipt);

            foreach (IPurchaseReceipt productReceipt in t_Result)
            {
                purchase_id = productReceipt.transactionID;
                purchase_time = System.DateTime.Now.ToString("yyyy-MM-dd H:mm");
                Debug.Log("pay_id " + purchase_id);
                Debug.Log("pay_time " + purchase_time);
            }
        }
        catch (IAPSecurityException i)
        {
            Debug.Log(i);
            t_IsBuy = false;
        }
#endif
        if (t_IsBuy)//2021-02-07 16:28 구매 성공 후 영수증의 사용가능 여부 체크
        {
            if (purchase_id == "" && purchase_time == "")
            {
                Debug.Log("purchase receipt: " + purchaseEvent.purchasedProduct.receipt);

                purchase_id = purchaseEvent.purchasedProduct.transactionID;
                purchase_time = System.DateTime.Now.ToString("yyyy-MM-dd H:mm");
            }
        }
        else
        {
            purchase_id = purchaseEvent.purchasedProduct.transactionID;
            purchase_time = System.DateTime.Now.ToString("yyyy-MM-dd H:mm");
        }
        SuccessPurchase();

        return PurchaseProcessingResult.Complete;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed");

        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("OnPurchaseFailed");
        //2021-02-07 16:29 구매 실패 시 구매 실패함수 실행

        purchasing = false;

        Debug.Log("Store specific error code: " + transactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
        if (transactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
        {
            Debug.Log("Purchase failure description message: " + transactionHistoryExtensions.GetLastPurchaseFailureDescription().message);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized");
        storeController = controller;
        googleExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        transactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();

        try
        {
            check_subscription();
        }
        catch (Exception e)
        {
            Debug.Log("OnInitialized StackTrace" + e.StackTrace);
        }
    }


    private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
    {
        try
        {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
            {
                Debug.Log("The product receipt does not contain enough information");
                return false;
            }
            var store = (string)receipt_wrapper["Store"];

            if (store == GooglePlay.Name)
            {
                return true;
            }

            var payload = (string)receipt_wrapper["Payload"];

            if (payload != null)
            {
                switch (store)
                {
                    case GooglePlay.Name:
                        {
                            var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                            if (!payload_wrapper.ContainsKey("json"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                                return false;
                            }
                            var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                            if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                                return false;
                            }
                            var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                            var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                            if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                                return false;
                            }
                            return true;
                        }
                }
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.Log("checkIfProductIsAvailableForSubscriptionManager e " + e);
            return false;
            throw e;
        }
    }
}
