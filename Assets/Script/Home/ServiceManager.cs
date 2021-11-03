using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase.Firestore;
using Firebase.Extensions;

public class ServiceManager : SingletonMonobehaviour<ServiceManager>
{
    public GameObject select_page;

    public GameObject notice_page;
    public GameObject qustion_page;
    public GameObject chat_page;

    public GameObject alarm_image;

    public GameObject notice_alarm_image;
    public GameObject qustion_alarm_image;

    bool notice_page_on;
    bool question_page_on;

    bool notice_on;
    bool question_on;
    
    public InputField input_field;

    public ChatManager chatManager;

    public Scrollbar scroll_bar;

    GameObject my_chat_prefab;
    GameObject other_chat_prefab;
    Queue<Chat> chat_list;

    WebViewObject webViewObject;

    void Start()
    {
        chat_list = new Queue<Chat>();

        my_chat_prefab = Resources.Load<GameObject>("Prefab/Chatting/MyChat");
        other_chat_prefab = Resources.Load<GameObject>("Prefab/Chatting/OtherChat");

        FirebaseManager.instance.get_question(chat_list, listen_chat_question);
    }

    public void listen_chat_question(byte i)
    {
        FirebaseManager.instance.firestore.Collection("ChatQuestion").Document(DataManager.instance.accountID).Collection("chat_value").Listen(snapshot =>
        {
            if (!snapshot.Metadata.IsFromCache)
            {
                foreach (DocumentChange change in snapshot.GetChanges())
                {
                    if (change.ChangeType.ToString() == "Added")
                    {
                        Dictionary<string, object> pairs = change.Document.ToDictionary();

                        byte player_index = Convert.ToByte(pairs["player"].ToString());
                        string msg = pairs["msg"].ToString();

                        chat_list.Enqueue(new Chat(player_index, msg));

                        if (!DataManager.instance.chat_question_key_list.Contains(change.Document.Id))
                        {
                            DataManager.instance.chat_question_key_list.Add(change.Document.Id);

                            DataManager.instance.save_chat_question_data();

                            on_alarm_image();
                        }

                        set_question_chatting_page();
                    }
                }
            }
        });
    }

    public void on_alarm_image()
    {
        if (!question_page_on)
        {
            question_on = true;
            alarm_image.SetActive(true);
            qustion_alarm_image.SetActive(true);
        }
    }

    public void on_select_page()
    {
        select_page.SetActive(true);
    }

    public void close_select_page()
    {
        if (!notice_on && !question_on)
        {
            alarm_image.SetActive(false);
        }
        select_page.SetActive(false);
    }

    public void on_chat_question_page()
    {
        question_on = false;
        question_page_on = true;
        qustion_alarm_image.SetActive(false);
        qustion_page.SetActive(true);
        close_select_page();
    }

    public void close_chat_question_page()
    {
        question_page_on = false;
        qustion_page.SetActive(false);
    }

    public void set_question_chatting_page()
    {
        try
        {
            Chat chat_data = chat_list.Dequeue();

            if (chat_data.player_index == 0)
            {
                ChatArea chat = Instantiate(my_chat_prefab).GetComponent<ChatArea>();
                chat.transform.parent = chat_page.transform;
                chat.transform.localScale = new Vector3(1, 1, 1);
                chatManager.chat_List.Add(chat);
                chat.set_msg(chat_data.msg);
            }
            else
            {
                ChatArea chat = Instantiate(other_chat_prefab).GetComponent<ChatArea>();
                chat.transform.parent = chat_page.transform;
                chat.transform.localScale = new Vector3(1, 1, 1);
                chatManager.chat_List.Add(chat);
                chat.set_msg(chat_data.msg);
            }
            chatManager.ChatSort();

            scroll_to_bottom();
        }
        catch (Exception e)
        {
            Debug.Log("set_chatting_page error: " + e);
        }
    }

    public void scroll_to_bottom()
    {
        Canvas.ForceUpdateCanvases();
        chat_page.transform.GetComponent<VerticalLayoutGroup>().enabled = false;
        chat_page.transform.GetComponent<VerticalLayoutGroup>().enabled = true;

        scroll_bar.value = 0;
    }

    bool sending_msg = false;
    public void send_question()
    {
        if (!sending_msg)
        {
            sending_msg = true;
            string msg = input_field.text;

            if (msg.Trim() != "")
            {
                FirebaseManager.instance.send_question(msg, send_question_result);
            }
        }
    }

    public void send_question_result(byte result)
    {
        switch (result)
        {
            case 1:
                {
                    sending_msg = false;
                    input_field.text = "";
                }
                break;
            case 3:
                {
                    sending_msg = false;
                    NetworkManager.Network_Error();
                }
                break;
        }
    }

    public class Chat
    {
        public byte player_index;
        public string msg;

        public Chat(byte player_index, string msg)
        {
            this.player_index = player_index;
            this.msg = msg;
        }
    }

    public void on_notice_service()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            NetworkManager.Network_Error();
        }
        else
        {
            FirebaseManager.instance.offline();

            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        StartCoroutine(check_noticeBoard("https://okgostop.kr/55/noticeBoard/noticeKOREABoard.html"));
                    }
                    break;
                case 1:
                    {
                        StartCoroutine(check_noticeBoard("https://okgostop.kr/55/noticeBoard/noticeJAPANBoard.html"));
                    }
                    break;
                case 2:
                    {
                        StartCoroutine(check_noticeBoard("https://okgostop.kr/55/noticeBoard/noticeUSABoard.html"));
                    }
                    break;
                case 3:
                    {
                        StartCoroutine(check_noticeBoard("https://okgostop.kr/55/noticeBoard/noticeCHINABoard.html"));
                    }
                    break;
            }
        }
    }

    IEnumerator check_noticeBoard(string Url)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(Url, "");

        yield return unityWebRequest.SendWebRequest();

        if (!unityWebRequest.isDone || unityWebRequest.error != null || unityWebRequest.responseCode != 200) 
        {
            Debug.Log("check_noticeBoard unityWebRequest error " + unityWebRequest.responseCode);
            NetworkManager.Network_Error();
        }
        else
        {
            Debug.Log("check_noticeBoard unityWebRequest isDone");
            StartCoroutine(open_notice_service(Url));
        }
        close_select_page();
    }

    public void close_notice_page()
    {
        Destroy(GameObject.Find("NoticeWebViewObject"));
        webViewObject = null;
        //MatchingManager.not_match_mode = false;
        FirebaseManager.instance.online();
    }

    IEnumerator open_notice_service(string Url)
    {
        webViewObject = (new GameObject("NoticeWebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(cb: msg =>
        {
            Debug.Log(string.Format("ServiceManager OpenWebView CallFromJS[{0}]", msg));
            if (msg.ToString().Equals("window_close"))
            {
                close_notice_page();
            }
            else if (msg.ToString().Equals("hwatoo_gambling"))
            {
                Application.OpenURL("https://play.google.com/store/apps/details?id=com.RimeFox.hwatooGambling");
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
        // Added alertDialogEnabled flag to enable/disable alert/confirm/prompt dialogs. by KojiNakamaru ， Pull Request #512 ， gree/unity-webview
        //webViewObject.SetAlertDialogEnabled(false);

        // cf. https://github.com/gree/unity-webview/pull/550
        // introduced SetURLPattern(..., hookPattern). by KojiNakamaru ， Pull Request #550 ， gree/unity-webview
        //webViewObject.SetURLPattern("", "^https://.*youtube.com", "^https://.*google.com");

        // cf. https://github.com/gree/unity-webview/pull/570
        // Add BASIC authentication feature (Android and iOS with WKWebView only) by takeh1k0 ， Pull Request #570 ， gree/unity-webview
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
}
