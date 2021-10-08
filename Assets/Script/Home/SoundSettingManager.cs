using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingManager : MonoBehaviour
{
    public GameObject sound_setting_page;

    Sprite on;
    Sprite off;

    Image background;
    Image effect;

    Text background_text;
    Text effect_text;

    void Start()
    {
        on = Resources.Load<Sprite>("Image/Sound");
        off = Resources.Load<Sprite>("Image/Soundm");

        background = transform.Find("SoundSettingPage/Main/Background/Image").GetComponent<Image>();
        effect = transform.Find("SoundSettingPage/Main/Effect/Image").GetComponent<Image>();

        background_text = transform.Find("SoundSettingPage/Main/Background/Button/Text").GetComponent<Text>();
        effect_text = transform.Find("SoundSettingPage/Main/Effect/Button/Text").GetComponent<Text>();

        transform.Find("SoundSettingPage/Main/Background/Button").GetComponent<Button>().onClick.AddListener(on_click_background);
        transform.Find("SoundSettingPage/Main/Effect/Button").GetComponent<Button>().onClick.AddListener(on_click_effect);

        transform.Find("SoundSettingPage/Title/CloseButton").GetComponent<Button>().onClick.AddListener(close_sound_setting);
    }

    public void on_sound_setting()
    {
        set_sound_page();
        sound_setting_page.SetActive(true);
    }

    void close_sound_setting()
    {
        sound_setting_page.SetActive(false);
    }

    void set_sound_page()
    {
        if (DataManager.instance.background_sound)
        {
            background.sprite = on;
            background_text.text = "음소거";
        }
        else
        {
            background.sprite = off;
            background_text.text = "음소거 해제";
        }

        if (DataManager.instance.effect_sound)
        {
            effect.sprite = on;
            effect_text.text = "음소거";
        }
        else
        {
            effect.sprite = off;
            effect_text.text = "음소거 해제";
        }
    }

    void on_click_background()
    {
        if (!DataManager.instance.background_sound)
        {
            DataManager.instance.background_sound = true;
            background.sprite = on;
            background_text.text = "음소거";
        }
        else
        {
            DataManager.instance.background_sound = false;
            background.sprite = off;
            background_text.text = "음소거 해제";
        }
        HomeSoundManager.instance.mute_background(!DataManager.instance.background_sound);
        DataManager.instance.save_sound_data();
    }

    void on_click_effect()
    {
        if (!DataManager.instance.effect_sound)
        {
            DataManager.instance.effect_sound = true;
            effect.sprite = on;
            effect_text.text = "음소거";
        }
        else
        {
            DataManager.instance.effect_sound = false;
            effect.sprite = off;
            effect_text.text = "음소거 해제";
        }
        HomeSoundManager.instance.mute_effect(!DataManager.instance.effect_sound);
        DataManager.instance.save_sound_data();
    }
}
