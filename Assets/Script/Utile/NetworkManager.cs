using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public static bool online_mode;

    GameObject offline_popup;
    static GameObject network_popup;
    static GameObject get_account_network_popup;
    static GameObject https_popup;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        network_popup = transform.Find("PopupCanvas/Set_Network").gameObject;
        get_account_network_popup = transform.Find("PopupCanvas/Network_Error").gameObject;
        transform.Find("PopupCanvas/Set_Network/SetNetworkPage/Button").GetComponent<Button>().onClick.AddListener(Confirm_Network_Error);
        transform.Find("PopupCanvas/Network_Error/SetNetworkPage/Button").GetComponent<Button>().onClick.AddListener(GameExit);
    }

    public void Offline()
    {
        offline_popup.SetActive(true);
    }

    public void Confirm_Offline()
    {
        offline_popup.SetActive(false);
    }

    public static void Network_Error()
    {
        network_popup.SetActive(true);
    }

    void Confirm_Network_Error()
    {
        network_popup.SetActive(false);
    }

    public static bool Internet_Error_Check()
    {
        Debug.Log("Internet_Error_Check Application internetReachability: " + Application.internetReachability);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Internet_Error_Check Internet error");
            Network_Error();
            return true;
        }
        else
        {
            Debug.Log("Internet_Error_Check Internet on");
            online_mode = true;
            return false;
        }
    }

    public static bool Get_Account_Internet_Error_Check()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            get_account_network_popup.SetActive(true);
            return true;
        }
        else
        {
            online_mode = true;
            return false;
        }
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public static void Https_Error()
    {
        https_popup.SetActive(true);
    }

    void Confirm_Https_Error()
    {
        https_popup.SetActive(false);
    }
}
