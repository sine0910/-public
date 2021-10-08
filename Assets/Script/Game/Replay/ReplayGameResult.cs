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
