using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchingManager : SingletonMonobehaviour<MatchingManager>
{
    //�ڽ��� ��Ī ���¸� Ȯ���ϴ� ����
    bool matching;

    string received_data;

    public Sprite black_sprite;
    public Sprite white_sprite;

    //��û�� �޾��� ��� �� ������Ʈ�� ǥ��
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

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void matching_start()
    {
        if (!NetworkManager.Internet_Check())
        {
            return;
        }

        if (!matching)
        {
            matching = true;

            FirebaseManager.instance.ready_to_matching(DataManager.instance.accountID);

            matching_key = DataManager.instance.accountID + TimeStamp.GetUnixTimeStamp();

            FirebaseManager.instance.create_matching_room(matching_key);

            matching_status_info_page_text.text = "�뱹�� ��û�� ����ڸ�\nã���ֽ��ϴ�.";
            matching_status_info_page_button.onClick.AddListener(cancle_matching);
            matching_status_info_page_button.gameObject.SetActive(true);
            matching_status_info_page_button_text.text = "���";
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
            StopCoroutine(auto_select);
            matching_info_page.SetActive(false);
            FirebaseManager.instance.accept_multi_game(matching_key, matching_id);
        }
    }

    public void matching_reject()
    {
        if (!select)
        {
            select = true;
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
        matching_status_info_page_text.text = "����� �����Ͽ����ϴ�\n����� �̵��մϴ�.";
        matching_status_info_page_button.gameObject.SetActive(false);
        matching_status_info_page_button.onClick.RemoveAllListeners();
        matching_status_info_page_button_text.text = "";

        FirebaseManager.instance.move_scene_to_game_room(account);
        FirebaseManager.instance.move_scene_to_game_room(DataManager.instance.accountID);
    }

    IEnumerator check_ghost_user()
    {
        yield return new WaitForSecondsRealtime(15f);
        if (matching)
        {
            StartCoroutine(re_find_user());
        }
    }

    IEnumerator re_find_user()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        matching_start();
    }

    public void not_find_user()
    {
        matching_status_info_page_text.text = "����ڸ� ã�� �� �����ϴ�\n����Ŀ� ��õ����ּ���.";
        matching_status_info_page_button_text.text = "Ȯ��";
        matching_status_info_page.SetActive(true);
    }

    void success_find_user()
    {
        matching_status_info_page_text.text = "����ڸ� ã�ҽ��ϴ�\n������ ��û�մϴ�.";
        matching_status_info_page_button_text.text = "���";
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
        if (!NetworkManager.Internet_Check())
        {
            return;
        }

        if (!matching)
        {
            matching = true;

            FirebaseManager.instance.ready_to_matching(DataManager.instance.accountID);

            matching_key = DataManager.instance.accountID + TimeStamp.GetUnixTimeStamp();

            FirebaseManager.instance.friend_matching(friend_accountID, matching_key);

            friend_accountID = "";

            matching_status_info_page_text.text = "ģ������ �뱹��\n��û�ϰ� �ֽ��ϴ�.";
            matching_status_info_page_button.onClick.AddListener(cancle_matching);
            matching_status_info_page_button.gameObject.SetActive(true);
            matching_status_info_page_button_text.text = "���";
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
                                        Debug.Log("RandomMatching case 1 / �Խ�Ʈ�� ��� ��û�� ����");

                                        string key = PopAt(matching_data);

                                        if (!matching && matching_key != key)
                                        {
                                            matching = true;
                                            matching_key = key;

                                            //��Ī ����
                                            //byte matching_type = Convert.ToByte(PopAt(matching_data));
                                            string roomID = PopAt(matching_data);

                                            //�÷��̾� ����
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
                                            //�÷��̾� ����
                                            string acount = PopAt(matching_data);

                                            FirebaseManager.instance.reject_multi_game(key, acount);
                                        }
                                    }
                                    break;

                                case 2:
                                    {
                                        Debug.Log("RandomMatching case 2 / �Խ�Ʈ�� ��� ��û�� ������ ���� ȣ��Ʈ���� ����");

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
                                        Debug.Log("RandomMatching case 3 / �Խ�Ʈ�� ��� ��û�� ������ ���� ȣ��Ʈ���� ����");

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
                                        Debug.Log("RandomMatching case 4 / �������� ��� ��û�� ������ ���� ����");
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
                                        Debug.Log("RandomMatching case 5 / ��û�� ��밡 ���� �������� ��� ��û�� ��ҵ� ���� ȣ��Ʈ�� ����");
                                        if (matching)
                                        {
                                            not_find_user();
                                        }
                                    }
                                    break;

                                case 6:
                                    {
                                        Debug.Log("RandomMatching case 6 / ��Ƽ���� ������ �̵�");
                                        move_scene();
                                    }
                                    break;

                                case 99:
                                    {
                                        Debug.Log("RandomMatching case 99 / ȣ��Ʈ���� ��� ��û�� ����� ���� �Խ�Ʈ�� ����");

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
