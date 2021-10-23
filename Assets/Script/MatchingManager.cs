using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchingManager : SingletonMonobehaviour<MatchingManager>
{
    //자신의 매칭 상태를 확인하는 변수
    bool matching;

    string received_data;

    public Sprite black_sprite;
    public Sprite white_sprite; 

    //신청을 받았을 경우 이 오브젝트를 표시
    public GameObject matching_info_page;
    public Text name_text;
    public Text tier_text;
    public Text country_text;
    public Text old_text;
    public Text gender_text;
    public Image gender_image;
    public Text player_type_text;
    public Image player_type_image;

    public GameObject matching_status_info_page;
    public Text matching_status_info_page_text;
    public Button matching_status_info_page_button;
    public Text matching_status_info_page_button_text;

    string matching_id = "";
    string matching_key = "";

    string account;
    string name;
    COUNTRY country;
    TIER tier;
    OLD old;
    GENDER gender;
    PLAYER_TYPE type;

    public IEnumerator auto_select;
    public IEnumerator on_matching_info;
    public IEnumerator check_ghost;

    public GameObject cancle_game_panel;

    public string friend_accountID;

    public int matching_score;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        matching_score = 1;
    }

    public void matching_start()
    {
        if (NetworkManager.Internet_Error_Check())
        {
            return;
        }

        if (!matching)
        {
            FirebaseManager.AnalyticsLog("matching_start", null, null);

            matching = true;
            matching_score = 2;

            FirebaseManager.instance.ready_to_matching(DataManager.instance.accountID);

            matching_key = DataManager.instance.accountID + TimeStamp.GetUnixTimeStamp();

            FirebaseManager.instance.create_matching_room(matching_key);

            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        matching_status_info_page_button_text.text = "취소";
                        matching_status_info_page_text.text = "대국을 신청할 사용자를\n찾고있습니다.";
                    }
                    break;

                case 1:
                    {
                        matching_status_info_page_button_text.text = "キャンセル";
                        matching_status_info_page_text.text = "大国を申請するユーザーを探しています。";
                    }
                    break;

                case 2:
                    {
                        matching_status_info_page_button_text.text = "Cancel";
                        matching_status_info_page_text.text = "Looking for users to apply for a match.";
                    }
                    break;

                case 3:
                    {
                        matching_status_info_page_button_text.text = "取消";
                        matching_status_info_page_text.text = "我们正在寻找用户申请匹配。";
                    }
                    break;
            }
            matching_status_info_page_button.onClick.AddListener(cancle_matching);
            matching_status_info_page_button.gameObject.SetActive(true);

            matching_status_info_page.SetActive(true);
        }
    }

    public void cancle_matching()
    {
        if (check_ghost != null)
        {
            StopCoroutine(check_ghost);
            check_ghost = null;
        }
        if (matching_id != "")
        {
            FirebaseManager.instance.cancle_multi_game(matching_key, matching_id);
        }
        matching_status_info_page.SetActive(false);
        reset_status();
    }

    public IEnumerator on_matching(string name, COUNTRY country, TIER tier, OLD old, GENDER gender, PLAYER_TYPE type)
    {
        matching_info_page.SetActive(true);
        name_text.text = name;
        country_text.text = Converter.country_to_string(country);
        tier_text.text = Converter.tier_to_string(tier);
        old_text.text = Converter.old_to_string(old);
        gender_text.text = Converter.gender_to_string(gender);
        gender_image.sprite = GenderManager.instance.get_gender_sprite(gender);

        player_type_text.text = Converter.player_type_to_string(type);
        switch (type)
        {
            case PLAYER_TYPE.BLACK:
                {
                    player_type_image.sprite = black_sprite;
                }
                break;
            case PLAYER_TYPE.WHITE:
                {
                    player_type_image.sprite = white_sprite;
                }
                break;
        }

        auto_select = auto_reject();
        StartCoroutine(auto_select);

        yield return 0;
    }

    public void matching_accept()
    {
        if (!select)
        {
            select = true;
            if (DataManager.instance.my_heart > 0)
            {
                matching_score = 3;

                StopCoroutine(auto_select);
                matching_info_page.SetActive(false);
                FirebaseManager.instance.accept_multi_game(matching_key, matching_id);
            }
            else
            {
                matching_score = 0;

                GameManager.instance.on_empty_heart();

                StopCoroutine(auto_select);
                matching_info_page.SetActive(false);
                FirebaseManager.instance.reject_multi_game(matching_key, matching_id);
                StartCoroutine(reject_game_delay());
            }
        }
    }

    public void matching_reject()
    {
        if (!select)
        {
            select = true;

            matching_score = 0;
            StopCoroutine(auto_select);
            matching_info_page.SetActive(false);
            FirebaseManager.instance.reject_multi_game(matching_key, matching_id);
            StartCoroutine(reject_game_delay());
        }
    }

    public void reset_status()
    {
        matching = false;

        matching_id = "";
        matching_key = "";

        GameManager.instance.host = false;
        GameManager.instance.game_room_id = "";

        FirebaseManager.instance.initialize_my_multi_data();
    }

    bool select = false;
    IEnumerator auto_reject()
    {
        select = false;
        yield return new WaitForSecondsRealtime(10f);
        if (!select)
        {
            select = true;

            matching_score = 0;
            matching_info_page.SetActive(false);

            FirebaseManager.instance.reject_multi_game(matching_key, matching_id);
            reset_status();
        }
    }

    public IEnumerator reject_game_delay()
    {
        yield return new WaitForSecondsRealtime(30f);
        reset_status();
    }

    void ready_to_move_scene()
    {
        switch (DataManager.instance.language)
        {
            case 0:
                {
                    matching_status_info_page_text.text = "대국을 수락하였습니다\n잠시후 이동합니다.";
                }
                break;

            case 1:
                {
                    matching_status_info_page_text.text = "大国を受け入れるました。しばらくして移動します。";
                }
                break;

            case 2:
                {
                    matching_status_info_page_text.text = "Accepted the game. I'll move on after a while.";
                }
                break;

            case 3:
                {
                    matching_status_info_page_text.text = "我接受了这个游戏。\n过一会儿我会继续前进。";
                }
                break;
        }

        matching_status_info_page_button.gameObject.SetActive(false);
        matching_status_info_page_button.onClick.RemoveAllListeners();
        matching_status_info_page_button_text.text = "";

        FirebaseManager.instance.move_scene_to_game_room(account);
        FirebaseManager.instance.move_scene_to_game_room(DataManager.instance.accountID);
    }

    IEnumerator check_ghost_user()
    {
        Debug.Log("check_ghost_user");
        yield return new WaitForSecondsRealtime(15f);
        Debug.Log("check_ghost_user after 15sc");
        if (matching)
        {
            yield return StartCoroutine(re_find_user());
        }
        else
        {
            Debug.Log("check_ghost_user after 15sc matching false!");
        }
    }

    IEnumerator re_find_user()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        matching = false;
        matching_start();
    }

    public void not_find_user()
    {
        switch (DataManager.instance.language)
        {
            case 0:
                {
                    matching_status_info_page_text.text = "사용자를 찾을 수 없습니다\n잠시후에 재시도해주세요.";
                    matching_status_info_page_button_text.text = "확인";
                }
                break;

            case 1:
                {
                    matching_status_info_page_text.text = "ユーザーを見つけることができません。しばらく再試行してください。";
                    matching_status_info_page_button_text.text = "確認";
                }
                break;

            case 2:
                {
                    matching_status_info_page_text.text = "User not found\nPlease try again later.";
                    matching_status_info_page_button_text.text = "Confirm";
                }
                break;

            case 3:
                {
                    matching_status_info_page_text.text = "未找到用户。 请稍后再试。";
                    matching_status_info_page_button_text.text = "确认";
                }
                break;
        }
        matching_status_info_page.SetActive(true);
    }

    void success_find_user()
    {
        switch (DataManager.instance.language)
        {
            case 0:
                {
                    matching_status_info_page_text.text = "사용자를 찾았습니다\n대전을 신청합니다.";
                    matching_status_info_page_button_text.text = "취소";
                }
                break;

            case 1:
                {
                    matching_status_info_page_text.text = "ユーザーが見つかりました。対戦を申し込みます。";
                    matching_status_info_page_button_text.text = "キャンセル";
                }
                break;

            case 2:
                {
                    matching_status_info_page_text.text = "User found. Apply for battle.";
                    matching_status_info_page_button_text.text = "Cancel";
                }
                break;

            case 3:
                {
                    matching_status_info_page_text.text = "用户找到。 申请战斗。";
                    matching_status_info_page_button_text.text = "取消";
                }
                break;
        }

        matching_status_info_page.SetActive(true);
    }

    void move_scene()
    {
        matching = false;
        matching_id = "";
        matching_key = "";

        matching_status_info_page.gameObject.SetActive(false);

        GameManager.instance.play_mode = PLAY.PVP;

        if (!GameManager.instance.host)
        {
            switch (type)
            {
                case PLAYER_TYPE.BLACK:
                    {
                        GameManager.instance.set_my_color(PLAYER_TYPE.WHITE);
                    }
                    break;
                case PLAYER_TYPE.WHITE:
                    {
                        GameManager.instance.set_my_color(PLAYER_TYPE.BLACK);
                    }
                    break;
            }
        }

        GameManager.instance.set_player_data(0, GameManager.instance.get_my_player_type(),
            DataManager.instance.my_name, DataManager.instance.my_tier, DataManager.instance.my_old, DataManager.instance.my_gender, DataManager.instance.my_country);
        GameManager.instance.set_player_data(1, GameManager.instance.get_other_player_type(),
            name, tier, old, gender, country);

        FirebaseManager.instance.offline();
        SceneManager.LoadScene("MultiScene");
    }

    void cancle_this_game()
    {
        cancle_game_panel.SetActive(true);
    }

    public void close_cancle_this_game()
    {
        cancle_game_panel.SetActive(false);
        reset_status();
    }

    public void friend_matching_start()
    {
        if (NetworkManager.Internet_Error_Check())
        {
            return;
        }

        if (!matching)
        {
            FirebaseManager.AnalyticsLog("friend_matching_start", null, null);

            matching = true;

            FirebaseManager.instance.ready_to_matching(DataManager.instance.accountID);

            matching_key = DataManager.instance.accountID + TimeStamp.GetUnixTimeStamp();

            FirebaseManager.instance.friend_matching(friend_accountID, matching_key);

            friend_accountID = "";

            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        matching_status_info_page_text.text = "친구에게 대국을\n신청하고 있습니다.";
                        matching_status_info_page_button_text.text = "취소";
                    }
                    break;

                case 1:
                    {
                        matching_status_info_page_text.text = "友達に大国を申請しています。";
                        matching_status_info_page_button_text.text = "キャンセル";
                    }
                    break;

                case 2:
                    {
                        matching_status_info_page_text.text = "You are asking a friend to play a game.";
                        matching_status_info_page_button_text.text = "Cancel";
                    }
                    break;

                case 3:
                    {
                        matching_status_info_page_text.text = "您正在邀请朋友玩游戏。";
                        matching_status_info_page_button_text.text = "取消";
                    }
                    break;
            }

            matching_status_info_page_button.onClick.AddListener(cancle_matching);
            matching_status_info_page_button.gameObject.SetActive(true);
            matching_status_info_page.SetActive(true);
        }
    }

    public void matching_listening()
    {
        FirebaseManager.instance.firestore.Collection("OnlineUser").Document(DataManager.instance.accountID).Listen(snapshot =>
        {
            if (!snapshot.Metadata.IsFromCache)
            {
                if (snapshot.Exists)
                {
                    Debug.Log("get snapshot data");
                    Dictionary<string, object> pairs = snapshot.ToDictionary();

                    if (pairs.ContainsKey("status"))
                    {
                        if (!is_received(pairs["status"].ToString()))
                        {
                            string data = pairs["status"].ToString();
                            Debug.Log("MultiHandleValueChanged packet: " + data);
                            string[] lines = data.Split(new string[] { "/" }, StringSplitOptions.None);

                            List<string> matching_data = new List<string>();

                            foreach (string line in lines)
                            {
                                matching_data.Add(line);
                            }

                            int grade = Int32.Parse(PopAt(matching_data));

                            switch (grade)
                            {
                                case 1:
                                    {
                                        Debug.Log("RandomMatching case 1 / 게스트가 대결 신청을 받음");

                                        string key = PopAt(matching_data);

                                        if (!matching && matching_key != key)
                                        {
                                            matching = true;
                                            matching_key = key;

                                            //매칭 정보
                                            //byte matching_type = Convert.ToByte(PopAt(matching_data));
                                            string roomID = PopAt(matching_data);

                                            //플레이어 정보
                                            account = PopAt(matching_data);
                                            name = PopAt(matching_data);
                                            country = Converter.to_country(PopAt(matching_data));
                                            tier = Converter.to_tier(PopAt(matching_data));
                                            old = Converter.to_old(PopAt(matching_data));
                                            gender = Converter.to_gender(PopAt(matching_data));
                                            type = Converter.to_player_type(PopAt(matching_data));

                                            if (account != DataManager.instance.accountID)
                                            {
                                                matching_id = account;

                                                GameManager.instance.game_room_id = roomID;
                                                GameManager.instance.other_account_id = account;
                                                GameManager.instance.host = false;

                                                on_matching_info = on_matching(name, country, tier, old, gender, type);
                                                StartCoroutine(on_matching_info);
                                            }
                                        }
                                        else
                                        {
                                            string roomID = PopAt(matching_data);
                                            //플레이어 정보
                                            string acount = PopAt(matching_data);

                                            FirebaseManager.instance.reject_multi_game(key, acount);
                                        }
                                    }
                                    break;

                                case 2:
                                    {
                                        Debug.Log("RandomMatching case 2 / 게스트가 대결 신청을 수락한 것을 호스트에서 받음");

                                        if (check_ghost != null)
                                        {
                                            StopCoroutine(check_ghost);
                                            check_ghost = null;
                                        }

                                        string key = PopAt(matching_data);

                                        if (matching && matching_key == key)
                                        {
                                            account = PopAt(matching_data);
                                            name = PopAt(matching_data);
                                            country = Converter.to_country(PopAt(matching_data));
                                            tier = Converter.to_tier(PopAt(matching_data));
                                            old = Converter.to_old(PopAt(matching_data));
                                            gender = Converter.to_gender(PopAt(matching_data));

                                            matching_id = account;

                                            GameManager.instance.game_room_id = DataManager.instance.accountID;
                                            GameManager.instance.other_account_id = account;
                                            GameManager.instance.host = true;

                                            ready_to_move_scene();
                                        }
                                        else
                                        {
                                            Debug.Log("not matching user matching_key: " + matching_key + " key: " + key);
                                            string acount = PopAt(matching_data);
                                            FirebaseManager.instance.cancle_multi_game(key, acount);
                                        }
                                    }
                                    break;

                                case 3:
                                    {
                                        Debug.Log("RandomMatching case 3 / 게스트가 대결 신청을 거절한 것을 호스트에서 받음");

                                        if (check_ghost != null)
                                        {
                                            StopCoroutine(check_ghost);
                                            check_ghost = null;
                                        }

                                        if (matching)
                                        {
                                            StartCoroutine(re_find_user());
                                        }
                                    }
                                    break;

                                case 4:
                                    {
                                        Debug.Log("RandomMatching case 4 / 서버에서 대결 신청에 성공한 것을 받음");
                                        if (matching)
                                        {
                                            success_find_user();
                                            check_ghost = check_ghost_user();
                                            StartCoroutine(check_ghost);
                                        }
                                    }
                                    break;

                                case 5:
                                    {
                                        Debug.Log("RandomMatching case 5 / 신청할 상대가 없어 서버에서 대결 신청이 취소된 것을 호스트가 받음");
                                        if (matching)
                                        {
                                            not_find_user();
                                        }
                                    }
                                    break;

                                case 6:
                                    {
                                        Debug.Log("RandomMatching case 6 / 멀티게임 씬으로 이동");
                                        move_scene();
                                    }
                                    break;

                                case 99:
                                    {
                                        Debug.Log("RandomMatching case 99 / 호스트에서 대결 신청을 취소한 것을 게스트가 받음");

                                        string key = PopAt(matching_data);

                                        if (matching_key == key)
                                        {
                                            reset_status();
                                            cancle_this_game();
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        });
    }

    bool is_received(string data)
    {
        if (received_data == data)
        {
            return true;
        }
        return false;
    }

    public string PopAt(List<string> list)
    {
        string r = list[0];
        list.RemoveAt(0);
        return r;
    }
}
