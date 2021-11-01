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

    public Text history_view_button_text;

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
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                result_text.text = "승리";
                            }
                            break;
                        case 1:
                            {
                                result_text.text = "勝利";
                            }
                            break;
                        case 2:
                            {
                                result_text.text = "Win";
                            }
                            break;
                        case 3:
                            {
                                result_text.text = "胜利";
                            }
                            break;
                    }
                }
                break;

            case 1:
                {
                    game_result_popup.sprite = lose_sprite;
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                result_text.text = "패배";
                            }
                            break;
                        case 1:
                            {
                                result_text.text = "敗北";
                            }
                            break;
                        case 2:
                            {
                                result_text.text = "Lose";
                            }
                            break;
                        case 3:
                            {
                                result_text.text = "打败";
                            }
                            break;
                    }
                }
                break;

            default:
                {
                    game_result_popup.sprite = lose_sprite;
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                result_text.text = "무승부";
                            }
                            break;
                        case 1:
                            {
                                result_text.text = "引き分け";
                            }
                            break;
                        case 2:
                            {
                                result_text.text = "Tie";
                            }
                            break;
                        case 3:
                            {
                                result_text.text = "领带";
                            }
                            break;
                    }
                }
                break;
        }

        win_text.text = (DataManager.instance.b_win_count + DataManager.instance.w_win_count) + "";
        lose_text.text = (DataManager.instance.b_lose_count + DataManager.instance.w_lose_count) + "";
        tie_text.text = (DataManager.instance.b_tie_count + DataManager.instance.w_tie_count) + "";
        winper_text.text = get_win_percent() + "%";

        setting_game_result();

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
        if (GameManager.instance.play_mode == PLAY.AI)
        {
            AIPlayManager.instance.GameQuit();
        }
        else if (GameManager.instance.play_mode == PLAY.PVP)
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

    void setting_game_result()
    {
        history_view = false;
        this.gameObject.GetComponent<Image>().color = new Color32(0, 0, 0, 150);
        this.gameObject.GetComponent<Image>().raycastTarget = true;
        switch (DataManager.instance.language)
        {
            case 0:
                {
                    history_view_button_text.text = "기록보기";
                }
                break;
            case 1:
                {
                    history_view_button_text.text = "履歴見る";
                }
                break;
            case 2:
                {
                    history_view_button_text.text = "View history";
                }
                break;
            case 3:
                {
                    history_view_button_text.text = "查看历史";
                }
                break;
        }
    }

    public bool history_view = false;

    public void set_history_view()
    {
        if (!history_view)
        {
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        history_view_button_text.text = "취소";
                    }
                    break;
                case 1:
                    {
                        history_view_button_text.text = "キャンセル";
                    }
                    break;
                case 2:
                    {
                        history_view_button_text.text = "Cancel";
                    }
                    break;
                case 3:
                    {
                        history_view_button_text.text = "取消";
                    }
                    break;
            }
            this.history_view = true;
            this.game_result_popup.gameObject.SetActive(false);
            this.gameObject.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
            this.gameObject.GetComponent<Image>().raycastTarget = false;
            UIManager.instance.on_history_view();
        }
        else
        {
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        history_view_button_text.text = "기록보기";
                    }
                    break;
                case 1:
                    {
                        history_view_button_text.text = "履歴見る";
                    }
                    break;
                case 2:
                    {
                        history_view_button_text.text = "View history";
                    }
                    break;
                case 3:
                    {
                        history_view_button_text.text = "查看历史";
                    }
                    break;
            }
            this.history_view = false;
            this.game_result_popup.gameObject.SetActive(true);
            this.gameObject.GetComponent<Image>().color = new Color32(0, 0, 0, 150);
            this.gameObject.GetComponent<Image>().raycastTarget = true;
            UIManager.instance.close_history_view();
        }
    }

    public void on_play_limit_panel()
    {
        panel.SetActive(true);
    }
}
