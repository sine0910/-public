using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReplayManager : SingletonMonobehaviour<ReplayManager>
{
    public GameObject select_page;

    #region OMOKTUBE
    public GameObject omoktube_page;
    public bool loading;

    public GameObject omoktube_record_slot_panel;
    public GameObject omoktube_record_slot;

    public List<OmoktubeRecordSlot> omoktube_play_recode_list = new List<OmoktubeRecordSlot>();
    public List<OmoktubeRecordData> omoktube_play_data_list = new List<OmoktubeRecordData>();

    public Scrollbar scrollbar;

    public Button time_button;
    public Button view_button;
    public Button friend_button;
    public Button country_button;
    public Button my_button;

    public Sprite on;
    public Sprite off;
    #endregion

    #region MY RECORD
    public GameObject my_record_page;

    public List<MyRecordSlot> my_play_record_list = new List<MyRecordSlot>();

    public int my_play_recode_page_count = 0;

    public Button prev_page_button;
    public Button next_page_button;
    #endregion

    void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            var recode = Instantiate(omoktube_record_slot);
            recode.transform.parent = omoktube_record_slot_panel.transform;
            recode.transform.localScale = new Vector3(1, 1, 1);
            recode.transform.localPosition = new Vector3(recode.transform.localPosition.x, recode.transform.localPosition.y, 0);
            omoktube_play_recode_list.Add(recode.GetComponent<OmoktubeRecordSlot>());
            omoktube_play_recode_list[i].set();
        }

        my_play_record_list = my_record_page.transform.Find("List").GetComponentsInChildren<MyRecordSlot>().ToList();
    }

    public void on_select_page()
    {
        on_omoktube_page();
        //select_page.SetActive(true);
    }

    public void close_select_page()
    {
        select_page.SetActive(false);
    }

    public void on_my_record_page()
    {
        set_button();
        my_button.image.sprite = on;

        my_play_recode_page_count = 0;
        set_my_play_record_list();
        my_record_page.SetActive(true);
    }

    public void close_my_play_page()
    {
        my_record_page.SetActive(false);
    }

    void set_my_play_record_list()
    {
        List<GameRecord> my_game_record = DataManager.instance.my_game_record.ToList();

        if (my_play_recode_page_count + my_play_record_list.Count < my_game_record.Count)
        {
            next_page_button.gameObject.SetActive(true);
        }
        else
        {
            next_page_button.gameObject.SetActive(false);
        }
        if (my_play_recode_page_count != 0)
        {
            prev_page_button.gameObject.SetActive(true);
        }
        else
        {
            prev_page_button.gameObject.SetActive(false);
        }

        initialize_my_play_record_list();

        int play_recode_count = my_game_record.Count - 1;

        for (int i = 0; i < my_play_record_list.Count; i++)
        {
            Debug.Log("show play_recode index: " + (play_recode_count - (my_play_recode_page_count + i)));
            if (play_recode_count - (my_play_recode_page_count + i) >= 0)
            {
                if (my_game_record[play_recode_count - (my_play_recode_page_count + i)].record.Count != 0)
                {
                    my_play_record_list[i].gameObject.SetActive(true);

                    my_play_record_list[i].set(play_recode_count - (my_play_recode_page_count + i),
                    my_game_record[play_recode_count - (my_play_recode_page_count + i)].record[0].play_time,
                    my_game_record[play_recode_count - (my_play_recode_page_count + i)].type_index,
                    my_game_record[play_recode_count - (my_play_recode_page_count + i)].score);
                }
            }
        }
    }

    public void prev_page()
    {
        my_play_recode_page_count -= 4;
        set_my_play_record_list();
    }

    public void next_page()
    {
        my_play_recode_page_count += 4;
        set_my_play_record_list();
    }

    public void initialize_my_play_record_list()
    {
        for (int i = 0; i < my_play_record_list.Count; i++)
        {
            my_play_record_list[i].gameObject.SetActive(false);
        }
    }

    public void select_play_this_my_play(int index)
    {
        Debug.Log("select_play_this_my_play index: " + index + "\nRecordManager.instance.my_game_record count: " + DataManager.instance.my_game_record.Count);
        Recorder.instance.replay_mode = REPLAY_MODE.PERSONAL;
        Recorder.instance.replay_record = DataManager.instance.my_game_record[index];
        SceneManager.LoadScene("ReplayScene");
    }

    public void on_omoktube_page()
    {
        FirebaseManager.instance.offline();

        get_time_omoktube_data();

        omoktube_page.SetActive(true);
    }

    public void close_omoktube_page()
    {
        FirebaseManager.instance.online();

        omoktube_page.SetActive(false);
    }

    public void set_button()
    {
        time_button.image.sprite = off;
        view_button.image.sprite = off;
        friend_button.image.sprite = off;
        country_button.image.sprite = off;
        my_button.image.sprite = off;
    }

    public void get_time_omoktube_data()
    {
        set_button();
        time_button.image.sprite = on;
        FirebaseManager.instance.load_omoktube_data("Time", result_get_omoktube_data);
    }

    public void get_view_omoktube_data()
    {
        set_button();
        view_button.image.sprite = on;
        FirebaseManager.instance.load_omoktube_data("View", result_get_omoktube_data);
    }

    public void get_friend_omoktube_data()
    {
        set_button();
        friend_button.image.sprite = on;
        StartCoroutine(ready_to_friend_omoktube_data());
    }

    IEnumerator ready_to_friend_omoktube_data()
    {
        List<string> friend_ranking_list = DataManager.instance.my_friend_id_list.ToList();

        for (int i = 0; i < friend_ranking_list.Count; i += 10)
        {
            if (friend_ranking_list.Count <= i + 10)
            {
                yield return StartCoroutine(FirebaseManager.instance.load_friend_omoktube_data(friend_ranking_list.GetRange(i, friend_ranking_list.Count - i)));
            }
            else
            {
                yield return StartCoroutine(FirebaseManager.instance.load_friend_omoktube_data(friend_ranking_list.GetRange(i, i + 10)));
            }

            if (omoktube_play_data_list.Count >= 50)
            {
                break;
            }
        }

        result_get_omoktube_data(1);
    }

    public void get_country_hwatube_data()
    {
        set_button();
        country_button.image.sprite = on;
        FirebaseManager.instance.load_omoktube_data("Country", result_get_omoktube_data);
    }

    public void get_my_hwatube_data()
    {
        set_button();
        my_button.image.sprite = on;
        FirebaseManager.instance.load_omoktube_data("My", result_get_omoktube_data);
    }

    void result_get_omoktube_data(byte result)
    {
        switch (result)
        {
            case 1:
                {
                    disable_record();
                    show_record_data();
                }
                break;

            case 2:
                {

                }
                break;

            case 3:
                {
                    NetworkManager.Network_Error();
                }
                break;
        }
    }

    public void disable_record()
    {
        close_my_play_page();
        for (int i = 0; i < omoktube_play_recode_list.Count; i++)
        {
            omoktube_play_recode_list[i].disable_record();
        }
    }

    public void show_record_data()
    {
        for (int i = 0; i < omoktube_play_data_list.Count; i++)
        {
            if (omoktube_play_recode_list.Count <= i)
            {
                break;
            }
            omoktube_play_recode_list[i].gameObject.SetActive(true);
            omoktube_play_recode_list[i].set_hwatube_data(omoktube_play_data_list[i].key, omoktube_play_data_list[i].title, omoktube_play_data_list[i].time,
              omoktube_play_data_list[i].score, omoktube_play_data_list[i].mode, omoktube_play_data_list[i].uploader_account_id, omoktube_play_data_list[i].uploader_name, omoktube_play_data_list[i].uploader_country, omoktube_play_data_list[i].uploader_tier, omoktube_play_data_list[i].view);
        }
        omoktube_play_data_list.Clear();

        scroll_to_top();
    }

    public void scroll_to_top()
    {
        Canvas.ForceUpdateCanvases();
        omoktube_record_slot_panel.transform.GetComponent<GridLayoutGroup>().enabled = false;
        omoktube_record_slot_panel.transform.GetComponent<GridLayoutGroup>().enabled = true;

        scrollbar.value = 1;
    }

    public void load_this_record_data(string key, string title)
    {
        Debug.Log("load_this_hwatube_record_data key: " + key);
        loading = true;
        Recorder.instance.replay_mode = REPLAY_MODE.OMOKTUBE;
        Recorder.instance.omoktube_replay_key = key;
        Recorder.instance.omoktube_replay_title = title;
        FirebaseManager.instance.load_this_record_data(key, load_omoktube_record_data);
    }

    void load_omoktube_record_data(byte result)
    {
        switch (result)
        {
            case 1:
                {
                    SceneManager.LoadScene("ReplayScene");
                }
                break;
            case 3:
                {
                    NetworkManager.Network_Error();
                }
                break;
        }
    }

    public class OmoktubeRecordData
    {
        public string key;
        public string title;
        public string time;
        public string score;
        public string mode;
        public string uploader_account_id;
        public string uploader_name;
        public COUNTRY uploader_country;
        public TIER uploader_tier;
        public int view;

        public OmoktubeRecordData(string key, string title, string time, string score, string mode, string id, string name, COUNTRY country, TIER tier, int view)
        {
            this.key = key;
            this.title = title;
            this.time = time;
            this.score = score;
            this.mode = mode;
            this.uploader_account_id = id;
            this.uploader_name = name;
            this.uploader_country = country;
            this.uploader_tier = tier;
            this.view = view;
        }
    }
}
