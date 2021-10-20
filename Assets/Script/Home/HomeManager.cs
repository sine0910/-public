using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeManager : SingletonMonobehaviour<HomeManager>
{
    public GameObject player_set_game;

    byte game_type;

    byte player_select_type;

    public GameObject black_panel;
    public GameObject white_panel;

    public HomeProfile home_profile;

    GameObject game_quit_popup;

    GameObject set_game_popup;

    void Start()
    {
        Time.timeScale = 1;

        game_quit_popup = transform.Find("Quit_Game").gameObject;
        set_game_popup = transform.Find("GameSetting").gameObject;

        transform.Find("QuitButton").GetComponent<Button>().onClick.AddListener(on_quit_game_popup);
        //transform.Find("SetButton").GetComponent<Button>().onClick.AddListener(on_set_game_page);

        transform.Find("Quit_Game/QuitGamePage/QuitButton").GetComponent<Button>().onClick.AddListener(quit_game);
        transform.Find("Quit_Game/QuitGamePage/NotQuitButton").GetComponent<Button>().onClick.AddListener(not_quit_game);

        transform.Find("GameSetting/GameSetPage/LanguageButton").GetComponent<Button>().onClick.AddListener(set_language);
        transform.Find("GameSetting/GameSetPage/CloseButton").GetComponent<Button>().onClick.AddListener(close_set_game_page);

        player_set_game.SetActive(false);

        if (DataManager.instance.other_day)
        {
            DataManager.instance.other_day = false;

            FirebaseManager.instance.send_dailey_event();
        }

        if (DataManager.instance.other_month)
        {
            DataManager.instance.other_month = false;
        }

        FirebaseManager.instance.online();
        Home();

        if (Recorder.instance.replay_record != null)
        {
            Debug.Log("Recorder.instance.replay_record = " + Recorder.instance.replay_record);
            Recorder.instance.replay_record = null;

            ReplayManager.instance.on_omoktube_page();
        }
    }

    public void Home()
    {
        home_profile.Set();
    }

    public void AI()
    {
        game_type = 0;
        on_player_set_game();
    }

    public void Multi()
    {
        if (DataManager.instance.my_heart == 0)
        {
            GameManager.instance.on_empty_heart();
            return;
        }

        game_type = 1;
        on_player_set_game();
    }

    public void Friend()
    {
        if (DataManager.instance.my_heart == 0)
        {
            GameManager.instance.on_empty_heart();
            return;
        }

        game_type = 2;
        on_player_set_game();
    }

    public void on_player_set_game()
    {
        reset_player_setting();
        player_set_game.SetActive(true);
    }

    public void close_player_set_game()
    {
        player_set_game.SetActive(false);
    }

    public void reset_player_setting()
    {
        player_select_type = byte.MaxValue;
        white_panel.SetActive(false);
        black_panel.SetActive(false);
    }

    public void player_select_black()
    {
        select_type(0);
    }

    public void player_select_white()
    {
        select_type(1);
    }

    public void select_type(byte type)
    {
        player_select_type = type;

        switch (player_select_type)
        {
            case 0:
                {
                    white_panel.SetActive(false);
                    black_panel.SetActive(true);
                }
                break;

            case 1:
                {
                    black_panel.SetActive(false);
                    white_panel.SetActive(true);
                }
                break;
        }
    }

    public void complete_select_type()
    {
        if (player_select_type != byte.MaxValue)
        {
            switch (player_select_type)
            {
                case 0:
                    {
                        GameManager.instance.set_my_color(PLAYER_TYPE.BLACK);
                    }
                    break;

                case 1:
                    {
                        GameManager.instance.set_my_color(PLAYER_TYPE.WHITE);
                    }
                    break;
            }
            close_player_set_game();

            switch (game_type)
            {
                case 0:
                    {
                        PlayAIGame();
                    }
                    break;

                case 1:
                    {
                        PlayMultiGame();
                    }
                    break;

                case 2:
                    {
                        PlayFriendGame();
                    }
                    break;
            }
        }
    }

    void PlayAIGame()
    {
        GameManager.instance.set_player_data(0, GameManager.instance.get_my_player_type(),
            DataManager.instance.my_name, DataManager.instance.my_tier, DataManager.instance.my_old, DataManager.instance.my_gender, DataManager.instance.my_country);
        GameManager.instance.set_player_data(1, GameManager.instance.get_other_player_type(),
            "AI", TIER.PRACTICE, OLD.NONE, GENDER.NONE, DataManager.instance.my_country);
        GameManager.instance.play_mode = PLAY.AI;
        SceneManager.LoadScene("PlayScene");
    }

    void PlayMultiGame()
    {
        MatchingManager.instance.matching_start();
    }

    void PlayFriendGame()
    {
        MatchingManager.instance.friend_matching_start();
    }

    void on_set_game_page()
    {
        set_game_popup.SetActive(true);
    }

    void close_set_game_page()
    {
        set_game_popup.SetActive(false);
    }

    public void set_language()
    {

    }

    void on_quit_game_popup()
    {
        game_quit_popup.SetActive(true);
    }

    void not_quit_game()
    {
        game_quit_popup.SetActive(false);
    }

    public void quit_game()
    {
        Application.Quit();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            FirebaseManager.instance.offline();
        }
        else
        {
            FirebaseManager.instance.online();
        }
    }

    private void OnApplicationQuit()
    {
        FirebaseManager.instance.offline();
    }
}
