using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RankingManager : MonoBehaviour
{
    WebViewObject webViewObject;

    public void on_rangking_page()
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
                        StartCoroutine(check_rankingBoard("https://okgostop.kr/55/ranking/KOREAranking.html?AccountID=" + DataManager.instance.accountID));
                    }
                    break;
                case 1:
                    {
                        StartCoroutine(check_rankingBoard("https://okgostop.kr/55/ranking/JAPANranking.html?AccountID=" + DataManager.instance.accountID));
                    }
                    break;
                case 2:
                    {
                        StartCoroutine(check_rankingBoard("https://okgostop.kr/55/ranking/KOREAranking.html?AccountID=" + DataManager.instance.accountID));
                    }
                    break;
                case 3:
                    {
                        StartCoroutine(check_rankingBoard("https://okgostop.kr/55/ranking/CHINAranking.html?AccountID=" + DataManager.instance.accountID));
                    }
                    break;
            }

        }
    }

    IEnumerator check_rankingBoard(string Url)
    {
        Debug.Log("check_rankingBoard url: " + Url);

        UnityWebRequest unityWebRequest = UnityWebRequest.Post(Url, "");

        yield return unityWebRequest.SendWebRequest();

        if (!unityWebRequest.isDone || unityWebRequest.error != null || unityWebRequest.responseCode != 200)
        {
            Debug.Log("check_rankingBoard unityWebRequest error " + unityWebRequest.responseCode);
            NetworkManager.Https_Error();
        }
        else
        {
            Debug.Log("check_rankingBoard unityWebRequest isDone");
            StartCoroutine(open_ranking_page(Url));
        }
    }

    void close_rangking_page()
    {
        Destroy(GameObject.Find("RankingWebViewObject"));
        webViewObject = null;
        FirebaseManager.instance.online();
    }

    IEnumerator open_ranking_page(string Url)
    {
        Debug.Log("open_ranking_page url: " + Url);

        webViewObject = (new GameObject("RankingWebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(cb: msg =>
        {
            Debug.Log(string.Format("ServiceManager OpenWebView CallFromJS[{0}]", msg));
            if (msg.ToString().Equals("window_close"))
            {
                close_rangking_page();
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
