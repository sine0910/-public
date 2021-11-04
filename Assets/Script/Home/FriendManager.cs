using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase.Firestore;

public class FriendManager : SingletonMonobehaviour<FriendManager>
{
    public GameObject invte_friend_page;

    WebViewObject webViewObject;

    ListenerRegistration listener;

    public GameObject friend_info_panel;
    public Text friend_info_text;

    void Start()
    {
        invte_friend_page = Resources.Load<GameObject>("Prefab/InviteFriend");
        listen_invite_friend();
    }

    void listen_invite_friend()
    {
        listener = FirebaseManager.instance.firestore.Collection("Friends").Document(DataManager.instance.accountID).Collection("invite_friend").Listen(snapshot =>
        {
            foreach (DocumentChange change in snapshot.GetChanges())
            {
                if (change.ChangeType.ToString() != "Removed")
                {
                    Dictionary<string, object> data = change.Document.ToDictionary();

                    InviteFriendPopup inviteFriend = Instantiate(invte_friend_page).GetComponent<InviteFriendPopup>();
                    inviteFriend.set(change.Document.Id, data["AccountID"].ToString(), data["Name"].ToString(),
                        Converter.to_country(data["Country"].ToString()), Converter.to_tier(data["Tier"].ToString()), Converter.to_old(data["Old"].ToString()), Converter.to_gender(data["Gender"].ToString()));

                    inviteFriend.transform.parent = HomeManager.instance.transform;
                    inviteFriend.transform.localScale = new Vector3(1, 1, 1);
                    inviteFriend.transform.localPosition = new Vector3(0, 0, 0);

                    inviteFriend.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
                }
            }
        });
    }

    string friend_key;
    string friend_accountID;
    public delegate void Callback(byte success);
    Callback friend_callback;

    bool ready = true;

    public void accept_friend(string key, string accountID, Callback callback)
    {
        if (ready)
        {
            FirebaseManager.AnalyticsLog("accept_invite_friend", null, null);
            if (DataManager.instance.my_friend_id_list.Count < 10)
            {
                ready = false;

                friend_key = key;
                friend_accountID = accountID;
                friend_callback = callback;

                FirebaseManager.instance.check_friend_count(friend_accountID, check_friend_count_result);
            }
            else
            {
                over_friend();
            }
        }
    }

    public void reject_friend(string key, string accountID, Callback callback)
    {
        if (ready)
        {
            FirebaseManager.AnalyticsLog("reject_invite_friend", null, null);
            ready = false;

            friend_key = key;
            friend_accountID = accountID;
            friend_callback = callback;

            FirebaseManager.instance.reject_invite_friend(friend_key, friend_accountID, friend_complete_result);
        }
    }

    public void check_friend_count_result(byte result)
    {
        switch (result)
        {
            case 1:
                {
                    FirebaseManager.instance.accept_invite_friend(friend_key, friend_accountID, friend_complete_result);
                }
                break;
            case 2:
                {
                    other_over_friend();
                    ready = true;
                }
                break;
            case 3:
                {
                    ready = true;
                }
                break;
        }
    }

    public void friend_complete_result(byte result)
    {
        ready = true;
        friend_callback(result);
    }

    public void over_friend()
    {
        friend_info_text.text = "더 이상 친구를 추가할 수 없습니다.";
        friend_info_panel.SetActive(true);
    }

    public void other_over_friend()
    {
        friend_info_text.text = "상대가 더 이상 친구를 추가할 수 없습니다.";
        friend_info_panel.SetActive(true);
    }

    public void close_friend_info_panel()
    {
        friend_info_panel.SetActive(false);
    }

    public void on_friend_page()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            NetworkManager.Network_Error();
        }
        else
        {
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        StartCoroutine(check_friend_page(string.Format("https://okgostop.kr/55/friend/KOREAFriends.html?AccountID={0}&Gender={1}&Name={2}&Old={3}&Tier={4}&Country={5}",
                            DataManager.instance.accountID, DataManager.instance.my_gender.ToString(), DataManager.instance.my_name, DataManager.instance.my_old.ToString(), DataManager.instance.my_tier.ToString(), DataManager.instance.my_country.ToString())));
                    }
                    break;
                case 1:
                    {
                        StartCoroutine(check_friend_page(string.Format("https://okgostop.kr/55/friend/JAPANFriends.html?AccountID={0}&Gender={1}&Name={2}&Old={3}&Tier={4}&Country={5}",
                            DataManager.instance.accountID, DataManager.instance.my_gender.ToString(), DataManager.instance.my_name, DataManager.instance.my_old.ToString(), DataManager.instance.my_tier.ToString(), DataManager.instance.my_country.ToString())));
                    }
                    break;
                case 2:
                    {
                        StartCoroutine(check_friend_page(string.Format("https://okgostop.kr/55/friend/USAFriends.html?AccountID={0}&Gender={1}&Name={2}&Old={3}&Tier={4}&Country={5}",
                            DataManager.instance.accountID, DataManager.instance.my_gender.ToString(), DataManager.instance.my_name, DataManager.instance.my_old.ToString(), DataManager.instance.my_tier.ToString(), DataManager.instance.my_country.ToString())));
                    }
                    break;
                case 3:
                    {
                        StartCoroutine(check_friend_page(string.Format("https://okgostop.kr/55/friend/CHINAFriends.html?AccountID={0}&Gender={1}&Name={2}&Old={3}&Tier={4}&Country={5}",
                            DataManager.instance.accountID, DataManager.instance.my_gender.ToString(), DataManager.instance.my_name, DataManager.instance.my_old.ToString(), DataManager.instance.my_tier.ToString(), DataManager.instance.my_country.ToString())));
                    }
                    break;
            }
        }
    }

    IEnumerator check_friend_page(string Url)
    {
        Debug.Log("friend_page url: " + Url);

        UnityWebRequest unityWebRequest = UnityWebRequest.Post(Url, "");

        yield return unityWebRequest.SendWebRequest();

        if (!unityWebRequest.isDone || unityWebRequest.error != null || unityWebRequest.responseCode != 200)
        {
            Debug.Log("check_rankingBoard unityWebRequest error " + unityWebRequest.responseCode);
            NetworkManager.Network_Error();
        }
        else
        {
            Debug.Log("check_rankingBoard unityWebRequest isDone");
            StartCoroutine(open_friend_page(Url));
        }
    }

    void close_rangking_page()
    {
        Destroy(GameObject.Find("FriendWebViewObject"));
        webViewObject = null;
        FirebaseManager.instance.online();
    }

    IEnumerator open_friend_page(string Url)
    {
        Debug.Log("open_friend_page url: " + Url);

        webViewObject = (new GameObject("FriendWebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(cb: msg =>
        {
            Debug.Log(string.Format("FriendManager OpenWebView CallFromJS[{0}]", msg));
            if (msg.ToString().Equals("window_close"))
            {
                close_rangking_page();
            }
            else if (msg.ToString().Contains("ad@"))
            {
                close_rangking_page();
                string url = msg.Replace("ad@", "");
                Application.OpenURL(url);
            }
            else if (msg.ToString().Contains("friend_game/"))
            {
                string other_id = msg.Replace("friend_game/", "");

                close_rangking_page();
                MatchingManager.instance.friend_accountID = other_id;
                HomeManager.instance.Friend();
            }
        }, err: msg =>
        {
            Debug.Log(string.Format("CallOnError[{0}]", msg));
        }, started: msg =>
        {
            Debug.Log(string.Format("CallOnStarted[{0}]", msg));
        }, hooked: msg =>
        {
            Debug.Log(string.Format("CallOnHooked[{0}]", msg));
        }, ld: (msg) =>
        {
            Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
#if UNITY_EDITOR_OSX || (!UNITY_ANDROID && !UNITY_WEBPLAYER && !UNITY_WEBGL)
                // NOTE: depending on the situation, you might prefer
                // the 'iframe' approach.
                // cf. https://github.com/gree/unity-webview/issues/189
#if true
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        window.location = 'unity:' + msg;
                      }
                    }
                  }
                ");
#else
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        var iframe = document.createElement('IFRAME');
                        iframe.setAttribute('src', 'unity:' + msg);
                        document.documentElement.appendChild(iframe);
                        iframe.parentNode.removeChild(iframe);
                        iframe = null;
                      }
                    }
                  }
                ");
#endif
#elif UNITY_WEBPLAYER || UNITY_WEBGL
                webViewObject.EvaluateJS(
                    "window.Unity = {" +
                    "   call:function(msg) {" +
                    "       parent.unityWebView.sendMessage('WebViewObject', msg)" +
                    "   }" +
                    "};");
#endif
            webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
        },
            //transparent: false,
            //zoom: true,
            //ua: "custom user agent string",
#if UNITY_EDITOR
            separated: false,
#endif
            enableWKWebView: true,
            wkContentMode: 0);  // 0: recommended, 1: mobile, 2: desktop
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        webViewObject.bitmapRefreshCycle = 1;
#endif
        // cf. https://github.com/gree/unity-webview/pull/512
        // Added alertDialogEnabled flag to enable/disable alert/confirm/prompt dialogs. by KojiNakamaru · Pull Request #512 · gree/unity-webview
        //webViewObject.SetAlertDialogEnabled(false);

        // cf. https://github.com/gree/unity-webview/pull/550
        // introduced SetURLPattern(..., hookPattern). by KojiNakamaru · Pull Request #550 · gree/unity-webview
        //webViewObject.SetURLPattern("", "^https://.*youtube.com", "^https://.*google.com");

        // cf. https://github.com/gree/unity-webview/pull/570
        // Add BASIC authentication feature (Android and iOS with WKWebView only) by takeh1k0 · Pull Request #570 · gree/unity-webview
        //webViewObject.SetBasicAuthInfo("id", "password");

        webViewObject.SetMargins(0, 0, 0, 0);
        webViewObject.SetVisibility(true);
        Debug.Log("StartNotice 2");

#if !UNITY_WEBPLAYER && !UNITY_WEBGL
        if (Url.StartsWith("http"))
        {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
            Debug.Log("StartNotice 3-1");
        }
        else
        {
            Debug.Log("StartNotice 3-2");
            var exts = new string[]{
                ".jpg",
                ".js",
                ".html"  // should be last
            };
            foreach (var ext in exts)
            {
                var url = Url.Replace(".html", ext);
                var src = System.IO.Path.Combine(Application.streamingAssetsPath, url);
                var dst = System.IO.Path.Combine(Application.persistentDataPath, url);
                byte[] result = null;
                if (src.Contains("://"))
                {  // for Android
#if UNITY_2018_4_OR_NEWER
                    // NOTE: a more complete code that utilizes UnityWebRequest can be found in https://github.com/gree/unity-webview/commit/2a07e82f760a8495aa3a77a23453f384869caba7#diff-4379160fa4c2a287f414c07eb10ee36d
                    var unityWebRequest = UnityWebRequest.Get(src);
                    yield return unityWebRequest.SendWebRequest();
                    result = unityWebRequest.downloadHandler.data;
#else
                    var www = new WWW(src);
                    yield return www;
                    result = www.bytes;
#endif
                }
                else
                {
                    result = System.IO.File.ReadAllBytes(src);
                }
                System.IO.File.WriteAllBytes(dst, result);
                if (ext == ".html")
                {
                    webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
                    break;
                }
            }
        }
#else
        if (Url.StartsWith("http")) {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
        } else {
            webViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
        }
#endif
        yield break;
    }

    private void OnDestroy()
    {
        if (listener != null)
        {
            listener.Stop();
        }
    }
}
