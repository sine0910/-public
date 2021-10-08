using System;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    string account;

    bool login = false;

    void Start()
    {
        //카카오 연동시 카카오 키를 받기 위한 코드
        androidJavaObject = new AndroidJavaObject("com.RimeFox.omock_game.Kakao_Plugin");
        androidJavaObject.Call("Global");
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
            login_popup_text.text = "아이디와 비밀번호를\n입력해주세요";
            login = false;
            return;
        }

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
                    login_popup_text.text = "존재하지 않는\n아이디 입니다";
                }
                break;

            case 3:
                {
                    login = false;
                    account = "";
                    login_popup.SetActive(true);
                    login_popup_text.text = "잘못된 비밀번호를\n입력하였습니다";
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
        StartCoroutine(get_accounID(account));
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
                    account_create_popup_text.text = "사용할 수 없는\n아이디 입니다.";
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
            account_create_popup_text.text = "아이디와 비밀번호 확인이\n필요합니다.";
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

                    account_create_popup_text.text = "계정이 성공적으로\n생성되었습니다";
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
        Debug.Log("success_kakao_login");
        login_select_panel.SetActive(false);
        account = kakao_account;
        StartCoroutine(get_accounID(kakao_account));
    }
    #endregion

    IEnumerator get_accounID(string accounID)
    {
        Debug.Log("get_accounID");
        DataManager.instance.save_my_account_data(accounID);

        if (NetworkManager.Internet_Check())
        {
            FirebaseManager.instance.check_login_day();
            yield return new WaitUntil(() => FirebaseManager.check_login_end);
            FirebaseManager.check_login_end = false;
            DataManager.instance.load_my_data();

            if (DataManager.instance.check_my_data() != 0)
            {
                FirebaseManager.instance.get_account_data(get_account_data_result);
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
        Debug.Log("complete_login");

        FirebaseManager.instance.get_notice_token();

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

