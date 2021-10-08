using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
    public Sprite win_sprite;
    public Sprite lose_sprite;

    public Image game_result_popup;

    public Text result_text;

    public Text win_text;
    public Text lose_text;
    public Text tie_text;
    public Text winper_text;

    public GameObject panel;

    public delegate void SendFn(List<string> msg);

    SendFn send;

    public void on_result(byte win, SendFn send)
    {
        this.send = send;

        switch (win)
        {
            case 0:
                {
                    game_result_popup.sprite = win_sprite;
                    result_text.text = "�¸�";
                }
                break;

            case 1:
                {
                    game_result_popup.sprite = lose_sprite;
                    result_text.text = "�й�";
                }
                break;

            default:
                {
                    game_result_popup.sprite = lose_sprite;
                    result_text.text = "���º�";
                }
                break;
        }

        win_text.text = (DataManager.instance.b_win_count + DataManager.instance.w_win_count) + "ȸ";
        lose_text.text = (DataManager.instance.b_lose_count + DataManager.instance.w_lose_count) + "ȸ";
        tie_text.text = (DataManager.instance.b_tie_count + DataManager.instance.w_tie_count) + "ȸ";
        winper_text.text = get_win_percent() + "%";

        panel.SetActive(false);
    }

    public float get_win_percent()
    {
        float val = 0;

        if (DataManager.instance.b_win_count + DataManager.instance.w_win_count > 0)
        {
            float win_count = DataManager.instance.b_win_count + DataManager.instance.w_win_count;

            float total_count = (DataManager.instance.b_win_count + DataManager.instance.w_win_count)
                + (DataManager.instance.b_lose_count + DataManager.instance.w_lose_count)
                + (DataManager.instance.b_tie_count + DataManager.instance.w_tie_count);

            val = win_count / total_count * 100;
            System.Math.Round(val, 1);
        }
        else
        {
            val = 0;
        }
        return val;
    }

    public void quit_game()
    {
        if (AIPlayManager.instance != null)
        {
            AIPlayManager.instance.GameQuit();
        }
        else if (MultiPlayManager.instance != null)
        {
            MultiPlayManager.instance.QuitGame();
        }
    }

    public void play_game()
    {
        if (GameManager.instance.play_mode == PLAY.PVP && DataManager.instance.my_heart == 0)
        {
            GameManager.instance.on_empty_heart();
            quit_game();
        }
        else
        {
            this.gameObject.SetActive(false);

            List<string> send_msg = new List<string>();
            send_msg.Add((byte)PROTOCOL.READY_TO_START + "");
            send(send_msg);
        }
    }

    public void on_play_limit_panel()
    {
        panel.SetActive(true);
    }
}
