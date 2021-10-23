using System;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Google;

public class LoginManager : SingletonMonobehaviour<LoginManager>
{
    private AndroidJavaObject androidJavaObject;

    public GameObject login_select_panel;

    public GameObject login_panel;
    public InputField InputID;
    public InputField InputPw;

    public GameObject login_popup;
    public Text login_popup_text;

    public GameObject create_account_panel;
    public InputField input_account_ID;
    public GameObject account_ID_ckeck_on;
    public GameObject account_ID_ckeck_button;
    public InputField input_account_Pw;
    public InputField input_account_check_Pw;
    public GameObject account_Pw_not_equal;

    public GameObject account_create_popup;
    public Text account_create_popup_text;

    public GameObject google_login_panel;
    public Text google_login_text;

    string web_clientId = "91721955365-9kn4dpcfvk00lvct4nb26tt595epuije.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;

    string account;

    public bool select_language;

    bool login = false;

    void Start()
    {
        //카카오 연동시 카카오 키를 받기 위한 코드
        androidJavaObject = new AndroidJavaObject("com.RimeFox.omock_game.Kakao_Plugin");
        androidJavaObject.Call("Global");

        //구글 로그인
        configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = web_clientId
        };
    }

    public void check_login()
    {
#if FALSE//DEVELOPMENT_BUILD//UNITY_EDITOR || DEVELOPMENT_BUILD
        DataManager.instance.accountID = SystemInfo.deviceUniqueIdentifier;
        DataManager.instance.my_country = "대한민국";
        check_user_set_data();
        //SceneManager.LoadScene("HomeScene");
#else
        StartCoroutine(check_login_start());
#endif
    }

    bool complete = false;
    public IEnumerator check_login_start()
    {
        //로그인 필요
        if (DataManager.instance.accountID == "" || DataManager.instance.accountID == null)
        {
            select_language = false;
            LanguageManager.instance.on_select_language_page();

            yield return new WaitUntil(() => select_language);

            StartServiceManager.instance.On();
            on_login_page();
        }
        else
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                FirebaseManager.instance.check_login_day();

                yield return new WaitUntil(() => FirebaseManager.check_login_end);

                DataManager.instance.load_my_data();
                if (DataManager.instance.check_my_data() != 0)
                {
                    Debug.Log("DataManager data load error!");
                    check_user_set_firebase_data();
                }
                else
                {
                    check_user_set_data();
                }
            }
            else
            {
                DataManager.instance.load_my_data();
                check_user_set_data();
            }
        }
    }

    void on_login_page()
    {
        login_select_panel.SetActive(true);
    }

    public void on_login()
    {
        login_panel.SetActive(true);
    }

    public void close_login()
    {
        login_panel.SetActive(false);
    }

    public void AccountLogin()
    {
        if (login)
        {
            return;
        }

        login = true;

        if (InputID.text == "" || InputPw.text == "")
        {
            //아이디와 패스워드를 입력하라는 팝업을 띄워준다.
            login_popup.SetActive(true);

            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        login_popup_text.text = "아이디와 비밀번호를\n입력해주세요";
                    }
                    break;

                case 1:
                    {
                        login_popup_text.text = "ユーザ名とパスワードを入力してください";
                    }
                    break;

                case 2:
                    {
                        login_popup_text.text = "Please enter your ID and password";
                    }
                    break;

                case 3:
                    {
                        login_popup_text.text = "请输入您的ID和密码";
                    }
                    break;
            }

            login = false;
            return;
        }

        FirebaseManager.AnalyticsLog("account_login", null, null);

        account = InputID.text;
        FirebaseManager.instance.check_account_login(InputID.text, InputPw.text, login_result);
    }

    void login_result(byte result)
    {
        switch (result)
        {
            //로그인 성공 시
            case 1:
                {
                    login_select_panel.SetActive(false);
                    login_panel.SetActive(false);
                    success_login();
                }
                break;

            //로그인 실패 시
            case 2:
                {
                    login = false;
                    account = "";
                    login_popup.SetActive(true);

                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                login_popup_text.text = "존재하지 않는\n아이디입니다";
                            }
                            break;

                        case 1:
                            {
                                login_popup_text.text = "存在しないIDです";
                            }
                            break;

                        case 2:
                            {
                                login_popup_text.text = "ID does not exist";
                            }
                            break;

                        case 3:
                            {
                                login_popup_text.text = "身份证不存在";
                            }
                            break;
                    }
                }
                break;

            case 3:
                {
                    login = false;
                    account = "";
                    login_popup.SetActive(true);

                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                login_popup_text.text = "잘못된 비밀번호를\n입력하였습니다";
                            }
                            break;

                        case 1:
                            {
                                login_popup_text.text = "間違ったパスワードを入力しました";
                            }
                            break;

                        case 2:
                            {
                                login_popup_text.text = "Entered an incorrect password";
                            }
                            break;

                        case 3:
                            {
                                login_popup_text.text = "您输入了错误的密码";
                            }
                            break;
                    }
                }
                break;

            case 4:
                {
                    login = false;
                    NetworkManager.Network_Error();
                }
                break;
        }
    }

    public void close_login_popup_panel()
    {
        login_popup.SetActive(false);
    }

    public void success_login()
    {
        Debug.Log("success_login " + account);
        FirebaseManager.AnalyticsLog("account_login_success", null, null);
        StartCoroutine(set_accounID(account));
    }

    public void CreateAccount()
    {
        create_account_panel.SetActive(true);

        input_account_ID.characterLimit = 12;
        input_account_ID.onValueChanged.AddListener((word) =>
            input_account_ID.text = Regex.Replace(word, @"[^0-9a-zA-Z]", "")
        );
        input_account_ID.onEndEdit.AddListener((word) =>
        {
            check_id = false;
            account_ID_ckeck_on.SetActive(false);

            if (word.Length < 4)
            {
                short_id = true;
            }
            else
            {
                short_id = false;
            }
        });

        input_account_Pw.characterLimit = 12;
        input_account_Pw.onValueChanged.AddListener((word) =>
            input_account_Pw.text = Regex.Replace(word, @"[^0-9a-zA-Z]", "")
        );
        input_account_Pw.onEndEdit.AddListener((word) =>
        {
            if (word.Length < 6)
            {
                short_pw = true;
            }
            else
            {
                short_pw = false;
            }
        });

        input_account_check_Pw.onValueChanged.AddListener(delegate { check_equal_to_password(); });
    }

    public void close_create_account_panel()
    {
        create_id = "";
        create_pw = "";

        input_account_ID.text = "";
        input_account_Pw.text = "";
        input_account_check_Pw.text = "";

        check_id = false;
        check_pw = false;

        account_ID_ckeck_button.SetActive(true);
        account_ID_ckeck_on.SetActive(false);
        account_Pw_not_equal.SetActive(false);

        create_account_panel.SetActive(false);
        input_account_check_Pw.onValueChanged.RemoveAllListeners();
    }

    string create_id;
    string create_pw;

    bool short_id = false;
    bool check_id = false;
    public void check_account_ID()
    {
        if (short_id)
        {
            return;
        }

        string val = input_account_ID.text;
        if (val.Trim() != "")
        {
            FirebaseManager.instance.check_account_id(val, check_account_ID_result);
        }
    }

    public void check_account_ID_result(byte result)
    {
        switch (result)
        {
            case 1:
                {
                    check_id = true;
                    create_id = input_account_ID.text;
                    account_ID_ckeck_button.SetActive(false);
                    account_ID_ckeck_on.SetActive(true);
                }
                break;
            case 2:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                account_create_popup_text.text = "사용할 수 없는\n아이디 입니다.";
                            }
                            break;

                        case 1:
                            {
                                account_create_popup_text.text = "使用できないIDです。";
                            }
                            break;

                        case 2:
                            {
                                account_create_popup_text.text = "Can not use this ID.";
                            }
                            break;

                        case 3:
                            {
                                account_create_popup_text.text = "您不能使用此 ID";
                            }
                            break;
                    }
                    account_create_popup.SetActive(true);
                }
                break;
            case 3:
                {
                    NetworkManager.Network_Error();
                }
                break;
        }
    }

    bool short_pw = false;
    bool check_pw = false;
    public void check_equal_to_password()
    {
        if (short_pw)
        {
            return;
        }

        if (input_account_Pw.text == input_account_check_Pw.text && input_account_Pw.text.Trim() != "")
        {
            check_pw = true;
            create_pw = input_account_Pw.text;
            account_Pw_not_equal.SetActive(false);
        }
        else
        {
            check_pw = false;
            account_Pw_not_equal.SetActive(true);
        }
    }

    public void complete_create_account_id()
    {
        if (check_id && check_pw)
        {
            FirebaseManager.instance.save_create_account(create_id, create_pw, save_account_result);
        }
        else
        {
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        account_create_popup_text.text = "아이디와 비밀번호 확인이 필요합니다.";
                    }
                    break;

                case 1:
                    {
                        account_create_popup_text.text = "ユーザ名とパスワードの確認が必要です。";
                    }
                    break;

                case 2:
                    {
                        account_create_popup_text.text = "ID and password confirmation is required.";
                    }
                    break;

                case 3:
                    {
                        account_create_popup_text.text = "需要ID和密码确认。";
                    }
                    break;
            }

            account_create_popup.SetActive(true);
        }
    }

    public void save_account_result(byte result)
    {
        switch (result)
        {
            case 1:
                {
                    close_create_account_panel();

                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                account_create_popup_text.text = "계정이 성공적으로\n생성되었습니다";
                            }
                            break;

                        case 1:
                            {
                                account_create_popup_text.text = "アカウントが正常に作成されました";
                            }
                            break;

                        case 2:
                            {
                                account_create_popup_text.text = "Your account has been successfully created";
                            }
                            break;

                        case 3:
                            {
                                account_create_popup_text.text = "您的帐户已经创建成功";
                            }
                            break;
                    }

                    account_create_popup.SetActive(true);
                }
                break;

            case 3:
                {
                    NetworkManager.Network_Error();
                }
                break;
        }
    }

    public void close_account_create_popup()
    {
        account_create_popup.SetActive(false);
    }

    #region KAKAO
    public void KakaoLogin()
    {
        if (login)
        {
            return;
        }

        FirebaseManager.AnalyticsLog("kakao_login", null, null);

        Debug.Log("KakaoLogin");
        login = true;
        androidJavaObject.Call("Login", new KakaoCallback());
    }

    public void get_kakao_id()
    {
        Debug.Log("get_kakao_id");
        androidJavaObject.Call("UserID", new KakaoCallback());
    }

    public void failed_kakao_login()
    {
        Debug.Log("get_failed_kakao_id");
        login = false;
    }

    public void success_kakao_login(string kakao_account)
    {
        FirebaseManager.AnalyticsLog("kakao_login_success", null, null);
        Debug.Log("success_kakao_login " + kakao_account);
        login_select_panel.SetActive(false);
        account = kakao_account;
        StartCoroutine(set_accounID(kakao_account));
    }
    #endregion

    #region GOOGLE

    IEnumerator google_callback;
    public void google_login()
    {
        FirebaseManager.AnalyticsLog("google_login", null, null);

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        //콜백을 받을 코루틴 함수를 호출시켜 콜백 대기 상태로 준비한다.
        google_callback = check_get_google_account();
        StartCoroutine(google_callback);

        google_login_panel.SetActive(true);
        AddStatusText("Calling SignIn");

        //비동기로 구글 로그인 결과를 받는다.
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    public void google_logout()
    {
        AddStatusText("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
         google_login_panel.SetActive(false);
    }

    public void OnDisconnect()
    {
        AddStatusText("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
        google_login_panel.SetActive(false);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted || task.IsCanceled)//실패
        {
            if (task.IsFaulted)
            {
                using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                        AddStatusText(web_clientId + "\n Got Error: " + error.Status + " " + error.Message + "\n" + error.Data);

                        FirebaseManager.AnalyticsLog("google_login_error", null, null);
                    }
                    else
                    {
                        AddStatusText("Got Unexpected Exception?!?" + task.Exception);
                        FirebaseManager.AnalyticsLog("google_login_error", null, null);
                    }
                }
            }
            else
            {
                AddStatusText("Canceled");
                FirebaseManager.AnalyticsLog("google_login_error", null, null);  
            }
            google_login_panel.SetActive(false);

            //초기화제거
            if (google_callback != null)
            {
                StopCoroutine(google_callback);
                google_callback = null;
            }
        }
        //else if (task.IsCanceled)//취소
        //{
        //    AddStatusText("Canceled");
        //    FirebaseManager.AnalyticsLog("google_login_error", null, null);
        //    google_login_panel.SetActive(false);
        //}
        else//정상일 경우
        {
            AddStatusText("Wellcome!\n" + task.Result.DisplayName);

            get_google_account(task.Result.UserId);
        }
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddStatusText("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }


    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        AddStatusText("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private List<string> messages = new List<string>();
    void AddStatusText(string text)
    {
        if (messages.Count == 5)
        {
            messages.RemoveAt(0);
        }
        messages.Add(text);
        string txt = "";
        foreach (string s in messages)
        {
            txt += "\n" + s;
        }
        google_login_text.text = txt;
    }

    void get_google_account(string google_id)
    {
        Debug.Log("get_google_account: " + google_id);

        FirebaseManager.AnalyticsLog("google_login_success", null, null);

        google_login_panel.SetActive(false);

        //비동기로 호출된 함수에서 구글 아이디 결과를 받아 임시 변수에 저장하고 콜백 대기용 변수를 변경한다.
        google_user_id = google_id;
        get_google_id = true;
        //StartCoroutine(get_accounID(google_id));
    }

    bool get_google_id = false;
    string google_user_id = "";

    IEnumerator check_get_google_account()
    {
        //콜백 대기용 변수가 변경될 때까지 대기
        yield return new WaitUntil(() => get_google_id);
        //정상적으로 값을 받아왔는지 확인한 후에 다음으로 진행하거나 다시시도하게 관변 변수를 초기화하고 다시시도하게 한다.
        if (google_user_id != "")
        {
            StartCoroutine(set_accounID(google_user_id));
        }
        else
        {
            get_google_id = false;
            google_user_id = "";
            NetworkManager.Network_Error();
        }
    }
    #endregion

    IEnumerator set_accounID(string accounID)
    {
        Debug.Log("get_accounID: " + accounID);
        DataManager.instance.save_my_account_data(accounID);

        if (NetworkManager.Internet_Error_Check())
        {
            Debug.Log("Internet_Error! : " + accounID);
            //DataManager.instance.load_my_data();
            //자신의 데이터에서 누락된 부분이 있는지 확인하고 설정하도록 한다.
            //check_user_set_data();
        }
        else//인터넷이 정상일 경우 
        {
            FirebaseManager.instance.check_login_day();
            yield return new WaitUntil(() => FirebaseManager.check_login_end);
            FirebaseManager.check_login_end = false;
            DataManager.instance.load_my_data();

            if (DataManager.instance.check_my_data() != 0)
            {
                //서버에서 자신의 데이터를 가져오면서 로컬에 데이터를 저장한다.
                FirebaseManager.instance.get_account_data(get_account_data_result);
            }
            else
            {
                //자신의 데이터에서 누락된 부분이 있는지 확인하고 설정하도록 한다.
                check_user_set_data();
            } 
        }
    }

    public void check_user_set_firebase_data()
    {
        FirebaseManager.instance.get_account_data(get_account_data_result);
    }

    void get_account_data_result(byte result)
    {
        switch (result)
        {
            case 1:
                {
                    check_user_set_data();
                }
                break;

            case 3:
                {
                    login = false;
                    NetworkManager.Network_Error();
                }
                break;
        }
    }

    public void check_user_set_data()
    {
        byte check_data = DataManager.instance.check_my_data();

        switch (check_data)
        {
            case 0:
                {
                    complete_login();
                    return;
                }
            case 1:
                {
                    Debug.Log("check_user_set_data case 1 Name");
                    NameManager.instance.player_set_name();
                    return;
                }
            case 2:
                {
                    Debug.Log("check_user_set_data case 2 Old");
                    OldManager.instance.player_set_old();
                    return;
                }
            case 3:
                {
                    Debug.Log("check_user_set_data case 3 Gender");
                    GenderManager.instance.player_set_gender();
                    return;
                }
            case 4:
                {
                    Debug.Log("check_user_set_data case 4 Country");
                    CountryManager.instance.player_set_country();
                    return;
                }
        }
    }

    public void complete_login()
    {
        Debug.Log("complete login");

        FirebaseManager.AnalyticsLog("complete_login", null, null);

        FirebaseManager.instance.get_notice_token();

        DataManager.instance.get_rating_score();
        DataManager.instance.load_friend_data();
        DataManager.instance.load_notice_data();
        DataManager.instance.load_question_data();
        DataManager.instance.load_my_game_record();

        FirebaseManager.instance.get_my_friend_list();

        MatchingManager.instance.matching_listening();

        SceneManager.LoadScene("HomeScene");
    }
}

public class KakaoCallback : AndroidJavaProxy
{
    public KakaoCallback() : base("com.RimeFox.omock_game.Kakao_Callback") { }

    public void Kakao_Login(bool on)
    {
        if (on)
        {
            Debug.Log("AndroidPluginCallback Login success");
            LoginManager.instance.get_kakao_id();
        }
        else
        {
            Debug.Log("AndroidPluginCallback Login failed");
            LoginManager.instance.failed_kakao_login();
        }
    }

    public void Get_UserID(long id)
    {
        Debug.Log("Kakao ID = " + id);
        string kakao_Id = id.ToString();
        LoginManager.instance.success_kakao_login(kakao_Id);
    }
}

