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
}
