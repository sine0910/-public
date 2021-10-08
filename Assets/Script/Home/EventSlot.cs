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

        //���� �����ۿ� ���� �̹����� �������� �Լ��� ȣ���Ͽ� ǥ���� ��
        switch (_reward)
        {
            case EVENT_ITEM.HEART:
                {
                    this.result_image.sprite = Resources.Load<Sprite>("Image/heart");
                    this.result_text.text = _reward_count + "��";
                }
                break;
        }
        this.hide_text.text = _main;

        DateTime now_date = DateTime.Now;
        int compare_val = DateTime.Compare(_deadline, now_date);
        TimeSpan time_val = _deadline - now_date;
        if (compare_val > 0)
        {
            if (time_val.Days > 0)
            {
                this.time_count_text.text = time_val.Days + "�� �� ����";
            }
            else if (time_val.Hours > 0)
            {
                this.time_count_text.text = time_val.Hours + "�ð� �� ����";
            }
            else if (time_val.Minutes > 0)
            {
                this.time_count_text.text = time_val.Minutes + "�� �� ����";
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
