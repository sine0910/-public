using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyRecordSlot : MonoBehaviour
{
    public int recode_index;
    public string key;

    Text time_text;
    Text other_text;
    Text score_text;

    public void set(int index, string time, string other, string score)
    {
        this.recode_index = index;

        this.set_ui();

        this.time_text.text = time;
        this.other_text.text = other;
        this.score_text.text = score;

        this.transform.Find("PlayButton").GetComponent<Button>().onClick.AddListener(this.on_click);
    }

    public void set_ui()
    {
        this.time_text = this.transform.Find("TimeText").GetComponent<Text>();
        this.other_text = this.transform.Find("OtherText").GetComponent<Text>();
        this.score_text = this.transform.Find("ScoreText").GetComponent<Text>();
    }

    public void on_click()
    {
        ReplayManager.instance.select_play_this_my_play(this.recode_index);
    }
}
