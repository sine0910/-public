using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventSlot : MonoBehaviour
{
    public EVENT_TYPE event_type;

    public string main;

    public EVENT_ITEM item;
    public int item_count;

    public Image result_image;
    public Text result_text;
    public Text time_count_text;

    public Text hide_text;
    public Text main_text;

    public int slot_index = 0;
    public string key_value = "";

    public void set(EVENT_TYPE event_type, int index, string _key, EVENT_ITEM _reward, int _reward_count, string _main, DateTime _deadline)
    {
        this.event_type = event_type;
        this.slot_index = index;
        this.key_value = _key;
        this.main = _main;
        this.item = _reward;
        this.item_count = _reward_count;

        //추후 아이템에 따라 이미지를 가져오는 함수를 호출하여 표시할 것
        switch (_reward)
        {
            case EVENT_ITEM.HEART:
                {
                    this.result_image.sprite = Resources.Load<Sprite>("Image/heart");
                    this.result_text.text = _reward_count + "";
                }
                break;
        }
        this.hide_text.text = _main;

        DateTime now_date = DateTime.Now;
        int compare_val = DateTime.Compare(_deadline, now_date);
        TimeSpan time_val = _deadline - now_date;
        if (compare_val > 0)
        {
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        if (time_val.Days > 0)
                        {
                            this.time_count_text.text = time_val.Days + "일 후 만료";
                        }
                        else if (time_val.Hours > 0)
                        {
                            this.time_count_text.text = time_val.Hours + "시간 후 만료";
                        }
                        else if (time_val.Minutes > 0)
                        {
                            this.time_count_text.text = time_val.Minutes + "분 후 만료";
                        }
                    }
                    break;
                case 1:
                    {
                        if (time_val.Days > 0)
                        {
                            this.time_count_text.text = time_val.Days + "日後の有効期限";
                        }
                        else if (time_val.Hours > 0)
                        {
                            this.time_count_text.text = time_val.Hours + "時間後、有効期限が切れ";
                        }
                        else if (time_val.Minutes > 0)
                        {
                            this.time_count_text.text = time_val.Minutes + "分後に有効期限が切れ";
                        }
                    }
                    break;
                case 2:
                    {
                        if (time_val.Days > 0)
                        {
                            this.time_count_text.text = "Expires in " + time_val.Days + " day";
                        }
                        else if (time_val.Hours > 0)
                        {
                            this.time_count_text.text = "Expires after " + time_val.Hours + " hour";
                        }
                        else if (time_val.Minutes > 0)
                        {
                            this.time_count_text.text = "Expires in " + time_val.Minutes + "minute";
                        }
                    }
                    break;
                case 3:
                    {
                        if (time_val.Days > 0)
                        {
                            this.time_count_text.text = time_val.Days + " 天后到期";
                        }
                        else if (time_val.Hours > 0)
                        {
                            this.time_count_text.text = time_val.Hours + " 小时后到期";
                        }
                        else if (time_val.Minutes > 0)
                        {
                            this.time_count_text.text = time_val.Minutes + " 分钟后到期";
                        }
                    }
                    break;
            }
        }
    }

    public void OnClick()
    {
        EventManager.instance.select_this_event(this.event_type, this, this.key_value);
    }

    void OnEnable()
    {
        this.StartCoroutine(this.text_setting(this.hide_text.text));
    }

    void OnDisable()
    {
        this.StopAllCoroutines();
    }

    IEnumerator text_setting(string main)
    {
        this.main_text.text = main;
        if (this.hide_text.rectTransform.rect.width > 360)
        {
            this.main_text.rectTransform.sizeDelta = new Vector2(this.hide_text.rectTransform.rect.width, this.hide_text.rectTransform.rect.height);
            this.main_text.rectTransform.localPosition = new Vector3((this.hide_text.rectTransform.rect.width - 360) / 2, 0);
            this.main_text.gameObject.SetActive(true);
            this.StartCoroutine(this.MovingText());
        }
        else
        {
            this.main_text.text = main;
            this.main_text.gameObject.SetActive(true);
        }
        yield return 0;
    }

    IEnumerator MovingText()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            this.main_text.gameObject.transform.Translate(-2.5f, 0, 0);
            if (this.main_text.gameObject.transform.localPosition.x < -(this.hide_text.rectTransform.rect.width))
            {
                this.main_text.gameObject.transform.localPosition = new Vector3(this.hide_text.rectTransform.rect.width, 0, 0);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
