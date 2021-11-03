using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayGameResult : MonoBehaviour
{
    public Sprite win_sprite;
    public Sprite lose_sprite;

    public Image game_result_popup;

    public Text result_text;

    public Text text;

    public void on_result(byte win)
    {
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
    }

    public void quit_game()
    {
        ReplayRoom.instance.stop_replay();
        gameObject.SetActive(false);
    }

    public void next_game()
    {
        ReplayRoom.instance.OnNext();
        gameObject.SetActive(false);
    }

    public Text history_view_button_text;

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
            ReplayUIManager.instance.on_history_view();
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
            ReplayUIManager.instance.close_history_view();
        }
    }
}
