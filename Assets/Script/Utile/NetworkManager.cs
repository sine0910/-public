using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public static bool online_mode;

    GameObject offline_popup;
    static GameObject network_popup;
    static GameObject https_popup;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        network_popup = transform.Find("PopupCanvas/Set_Network").gameObject;
        transform.Find("PopupCanvas/Set_Network/SetNetworkPage/Button").GetComponent<Button>().onClick.AddListener(Confirm_Network_Error);
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

    public static bool Internet_Check()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Network_Error();
            return false;
        }
        else
        {
            online_mode = true;
            return true;
        }
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
