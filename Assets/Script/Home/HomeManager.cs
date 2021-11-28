using System;
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

    public GameObject multi_black_panel;
    public GameObject multi_white_panel;

    public HomeProfile home_profile;

    GameObject game_quit_popup;

    GameObject multi_set_game_popup;

    void Start()
    {
        Time.timeScale = 1;

        game_quit_popup = transform.Find("Quit_Game").gameObject;

        multi_set_game_popup = transform.Find("MultiSettingGame").gameObject;

        transform.Find("QuitButton").GetComponent<Button>().onClick.AddListener(on_quit_game_popup);
        //transform.Find("SetButton").GetComponent<Button>().onClick.AddListener(on_set_game_page);

        transform.Find("Quit_Game/QuitGamePage/QuitButton").GetComponent<Button>().onClick.AddListener(quit_game);
        transform.Find("Quit_Game/QuitGamePage/NotQuitButton").GetComponent<Button>().onClick.AddListener(not_quit_game);

        player_set_game.SetActive(false);

        Home();

        StartCoroutine(CheckLoginDate());

        FirebaseManager.instance.online();

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

    public IEnumerator CheckLoginDate()
    {
        try
        {
            //최초 로그인일 경우
            if (DataManager.instance.login_time == null || DataManager.instance.login_time == DateTime.MinValue)
            {
                Debug.Log("First Home Scene CheckLoginDate");
                FirebaseManager.instance.check_login_day();
            }
            else
            {
                Debug.Log("Home Scene CheckLoginDate");
                TimeStamp.compair_login_date(DataManager.instance.login_time);

                if (DataManager.instance.other_day || DataManager.instance.other_month)
                {
                    DataManager.instance.login_time = DateTime.UtcNow.Add(TimeStamp.time_span);
                }

                FirebaseManager.instance.initialize_date_data();
            }
        }
        catch (Exception e)
        {
            Debug.Log("CheckLoginDateError: " + e);
        }
        yield return 0;
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
        if (game_type != 1)
        {
            player_set_game.SetActive(true);
        }
        else
        {
            multi_set_game_popup.SetActive(true);
        }
    }

    public void close_player_set_game()
    {
        player_set_game.SetActive(false);
        multi_set_game_popup.SetActive(false);
    }

    public void reset_player_setting()
    {
        player_select_type = byte.MaxValue;
        white_panel.SetActive(false);
        black_panel.SetActive(false);
        multi_white_panel.SetActive(false);
        multi_black_panel.SetActive(false);

        player_select_black();
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
                    if (game_type != 1)
                    {
                        white_panel.SetActive(false);
                        black_panel.SetActive(true);
                    }
                    else
                    {
                        multi_white_panel.SetActive(false);
                        multi_black_panel.SetActive(true);
                    }
                }
                break;

            case 1:
                {
                    if (game_type != 1)
                    { 
                        black_panel.SetActive(false);
                        white_panel.SetActive(true);
                    }
                    else
                    {
                        multi_white_panel.SetActive(true);
                        multi_black_panel.SetActive(false);
                    }
                }
                break;
        }
    }

    public void complete_select_type()//ai 연습의 확인 버튼
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

    void PlayAIGame()//ai 게임
    {
        GameManager.instance.set_player_data(
            0, 
            GameManager.instance.get_my_player_type(),
            DataManager.instance.my_name, 
            DataManager.instance.my_tier, 
            DataManager.instance.my_old, 
            DataManager.instance.my_gender, 
            DataManager.instance.my_country
            );

        string ai;
        if (DataManager.instance.my_tier < TIER.GRADE_11TH)
        {
            DataManager.instance.AI_IQ = 1;
            ai = "IQ 90";
        }
        else if (DataManager.instance.my_tier < TIER.GRADE_10TH)
        {
            DataManager.instance.AI_IQ = 2;
            ai = "IQ 100";
        }
        else
        {
            DataManager.instance.AI_IQ = 3;
            ai = "IQ 110";
        }     


        GameManager.instance.set_player_data(
            1, 
            GameManager.instance.get_other_player_type(),
            ai, 
            TIER.PRACTICE, 
            OLD.NONE, 
            GENDER.NONE, 
            DataManager.instance.my_country
            );
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
