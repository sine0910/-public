using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OmoktubeRecordSlot : MonoBehaviour
{
    string key;
    string title;
    string time;
    string uploader_account_id;
    string uploader_name;
    TIER uploader_tier;
    COUNTRY uploader_country;

    Text title_text;
    Text time_text;
    Text uploader_name_text;
    Text view_text;
    Text score_text;
    Text mode_text;
    Text tier_text;
    Image country_image;

    public void set()
    {
        this.title_text = this.transform.Find("TitleText").GetComponent<Text>();
        this.time_text = this.transform.Find("TimeText").GetComponent<Text>();
        this.uploader_name_text = this.transform.Find("OtherText").GetComponent<Text>();
        this.tier_text = this.transform.Find("OtherTierText").GetComponent<Text>();
        this.country_image = this.transform.Find("CountryImage").GetComponent<Image>();
        //this.view_text = this.transform.Find("View/ViewText").GetComponent<Text>();
        this.score_text = this.transform.Find("ScoreText").GetComponent<Text>();
        //this.mode_text = this.transform.Find("ModeText").GetComponent<Text>();

        disable_record();
    }

    public void set_hwatube_data(string key, string title, object time, string score, string mode, string uploader_account_id, string uploader_name, COUNTRY uploader_country, TIER uploader_tier, int view)
    {
        this.key = key;
        this.title = title;
        this.time = get_time_to_string(time);
        this.uploader_account_id = uploader_account_id;
        this.uploader_name = uploader_name;
        this.uploader_tier = uploader_tier;
        this.uploader_country = uploader_country;
        //this.view = view.ToString();

        this.score_text.text = score;
        //this.mode_text.text = mode;

        this.title_text.text = this.title;
        this.time_text.text = this.time;
        this.uploader_name_text.text = this.uploader_name;
        this.tier_text.text = Converter.tier_to_string(this.uploader_tier);
        this.country_image.sprite = CountryManager.instance.get_country_sprite(this.uploader_country);
    }

    public void disable_record()
    {
        this.title_text.text = "";
        this.time_text.text = "";
        this.uploader_name_text.text = "";
        //this.view_text.text = "";
        this.score_text.text = "";
        //this.mode_text.text = "";
        this.tier_text.text = "";

        this.gameObject.SetActive(false);
    }

    public void on_click()
    {
        if (!ReplayManager.instance.loading)
        {
            ReplayManager.instance.load_this_record_data(this.key, this.title);
        }
    }

    string get_time_to_string(object time)
    {
        DateTime start_date = Convert.ToDateTime(time).Add(TimeStamp.time_span);
        DateTime now_date = DateTime.Now;
        TimeSpan time_val = now_date - start_date;

        if (time_val.Days > 0)
        {
            if (time_val.Days > 365)
            {
                return (time_val.Days / 365) + "�� ��";
            }
            else if (time_val.Days > 30)
            {
                return (time_val.Days / 30) + "�� ��";
            }
            else
            {
                return time_val.Days + "�� ��";
            }
        }
        else if (time_val.Hours > 0)
        {
            return time_val.Hours + "�ð� ��";
        }
        else
        {
            return time_val.Minutes + "�� ��";
        }
    }
}
