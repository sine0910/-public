using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReplayRoom : SingletonMonobehaviour<ReplayRoom>
{
    REPLAY_MODE mode;

    public string key;
    public string title;
    public GameRecord replay_record;

    bool start = false;

    private string game_mode = "";
    private bool player_index = true;

    private float time_Scale = 1;

    float time;

    private int replay_index = 0;

    private bool on_menu = false;
    public GameObject menu_panel;

    public GameObject hwatube_panel;
    public GameObject personal_panel;

    public Text title_text;
    public Text count_text;

    public Button puase_button;
    public Sprite puase_sprite;
    public Sprite play_sprite;

    public Text speed_text;

    bool ready_move_recode = true;

    public GameObject end_replay_panel;

    public GameObject out_panel;

    public GameObject upload_recode_panel;
    public InputField title_input;

    public GameObject cannot_upload_recode_panel;
    public GameObject success_upload_recode_panel;

    Dictionary<int, PlayerProfile> player_profiles;
    public GameObject my_profile;
    public GameObject other_profile;

    IEnumerator recode_player;

    private void Awake()
    {
        mode = Recorder.instance.replay_mode;

        replay_record = new GameRecord
            (Recorder.instance.replay_record.key,
            Recorder.instance.replay_record.type_index,
            Recorder.instance.replay_record.score,
            Recorder.instance.replay_record.my_name,
            Recorder.instance.replay_record.my_country,
            Recorder.instance.replay_record.my_tier,
            Recorder.instance.replay_record.my_type,
            Recorder.instance.replay_record.other_name,
            Recorder.instance.replay_record.other_country,
            Recorder.instance.replay_record.other_tier,
            Recorder.instance.replay_record.other_type,
            Recorder.instance.replay_record.record);

        if (mode == REPLAY_MODE.OMOKTUBE)
        {
            key = Recorder.instance.omoktube_replay_key;
            title = Recorder.instance.omoktube_replay_title;
        }
        else
        {
            key = replay_record.key;
            title = replay_record.type_index + "\n" + replay_record.score;
        }
    }

    void Start()
    {
        Time.timeScale = time_Scale;

        title_text.text = title;
 
        set_profile();

        switch (mode)
        {
            case REPLAY_MODE.OMOKTUBE:
                {
                    personal_panel.SetActive(false);
                    hwatube_panel.SetActive(true);
                }
                break;
            case REPLAY_MODE.PERSONAL:
                {
                    hwatube_panel.SetActive(false);
                    personal_panel.SetActive(true);
                }
                break;
            default:
                {
                    hwatube_panel.SetActive(false);
                    personal_panel.SetActive(false);
                }
                break;
        }

        start_replay();
    }

    void start_replay()
    {
        if (start)
        {
            return;
        }
        start = true;

        if (recode_player != null)
        {
            StopCoroutine(recode_player);
            recode_player = null;
        }

        Debug.Log("start_replay");

        if (replay_index < replay_record.record.Count)
        {
            count_text.text = (replay_index + 1) + "/" + replay_record.record.Count;
            recode_player = GameReplaying(Recorder.Clone(replay_record.record[replay_index].record));
            StartCoroutine(recode_player);
        }
        else
        {
            end_replay();
        }
    }

    IEnumerator GameReplaying(List<Record> playRecod)
    {
        time = playRecod[0].time;
        ready_move_recode = true;
        while (true)
        {
            time += Time.deltaTime;
            if (playRecod.Count > 0)
            {
                if (time >= playRecod[0].time)
                {
                    ReplayUIManager.instance.on_receive(playRecod[0].packet);
                    playRecod.RemoveAt(0);
                }
            }
            else
            {
                start = false;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    void set_profile()
    {
        player_profiles = new Dictionary<int, PlayerProfile>();

        player_profiles.Add(0, my_profile.GetComponent<PlayerProfile>());
        player_profiles.Add(1, other_profile.GetComponent<PlayerProfile>());

        player_profiles[0].set_player_data(replay_record.my_type, replay_record.my_name, replay_record.my_tier, replay_record.my_country);
        player_profiles[1].set_player_data(replay_record.other_type, replay_record.other_name, replay_record.other_tier, replay_record.other_country);
    }

    public void Menu()
    {
        if (!on_menu)
        {
            on_menu = true;
            menu_panel.SetActive(true);
        }
        else
        {
            CloseMenu();
        }
    }

    void CloseMenu()
    {
        if (on_menu)
        {
            on_menu = false;
            menu_panel.SetActive(false);
        }
    }

    bool puase = false;
    public void OnPuase()
    {
        if (!puase)
        {
            puase = true;
            puase_button.image.sprite = puase_sprite;
            Time.timeScale = 0;
        }
        else
        {
            puase = false;
            puase_button.image.sprite = play_sprite;
            Time.timeScale = time_Scale;
        }
    }

    public void OnPrev()
    {
        if (!ready_move_recode)
        {
            return;
        }
        ready_move_recode = false;

        if (replay_index > 0)
        {
            replay_index--;
        }
        start = false;
        start_replay();
    }

    public void OnNext()
    {
        if (!ready_move_recode)
        {
            return;
        }
        ready_move_recode = false;

        replay_index++;

        start = false;
        start_replay();
    }

    public void replay_set_speed()
    {
        switch (time_Scale)
        {
            case 1:
                {
                    speed_text.text = "x2";
                    time_Scale = 2f;
                }
                break;

            case 2:
                {
                    speed_text.text = "x3";
                    time_Scale = 3;
                }
                break;

            case 3:
                {
                    speed_text.text = "x1";
                    time_Scale = 1;
                }
                break;
        }
        Time.timeScale = time_Scale;
    }

    void end_replay()
    {
        end_replay_panel.SetActive(true);
    }

    void close_end_replay()
    {
        end_replay_panel.SetActive(false);
    }

    public void on_upload_recode_panel()
    {
        upload_recode_panel.SetActive(true);
    }

    bool upload_ready = false;

    public void on_upload_recode()
    {
        if (title_input.text.Trim() != "" && title_input.text.Length < 25)
        {
            if (!upload_ready)
            {
                upload_ready = true;
                FirebaseManager.instance.check_upload_recode(key, check_upload_recode);
            }
        }
    }

    void check_upload_recode(byte result)
    {
        switch (result)
        {
            case 1:
                {
                    Recorder.instance.upload_to_omoktube(title_input.text, replay_record);
                    close_upload_recode_panel();
                    upload_ready = false;
                    success_upload_this_record();
                }
                break;
            case 2:
                {
                    upload_ready = false;
                    cannot_upload_this_record();
                }
                break;
            case 3:
                {
                    upload_ready = false;
                    NetworkManager.Network_Error();
                }
                break;
        }
    }

    public void close_upload_recode_panel()
    {
        upload_recode_panel.SetActive(false);
    }

    public void cannot_upload_this_record()
    {
        cannot_upload_recode_panel.SetActive(true);
    }

    public void close_cannot_upload_this_record()
    {
        cannot_upload_recode_panel.SetActive(false);
    }

    public void success_upload_this_record()
    {
        success_upload_recode_panel.SetActive(true);
    }

    public void close_success_upload_this_record()
    {
        success_upload_recode_panel.SetActive(false);
    }


    public void replay_replay()
    {
        close_end_replay();

        replay_index = 0;
        start = false;
        start_replay();
    }

    public void on_quit_replay_page()
    {
        out_panel.SetActive(true);
    }

    public void close_quit_replay_page()
    {
        out_panel.SetActive(false);
    }

    public void stop_replay()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("HomeScene");
    }
}
