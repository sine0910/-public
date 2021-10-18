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

            switch(DataManager.instance.language)
            {
                case 0:
                    {
                        background_text.text = "음소거";
                    }
                    break;
                case 1:
                    {
                        background_text.text = "ミュート";
                    }
                    break;
                case 2:
                    {
                        background_text.text = "mute";
                    }
                    break;
                case 3:
                    {
                        background_text.text = "沉默的";
                    }
                    break;
            }
        }
        else
        {
            background.sprite = off;

            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        background_text.text = "음소거 해제";
                    }
                    break;
                case 1:
                    {
                        background_text.text = "ミュート解除";
                    }
                    break;
                case 2:
                    {
                        background_text.text = "unmute";
                    }
                    break;
                case 3:
                    {
                        background_text.text = "取消静音";
                    }
                    break;
            }
        }

        if (DataManager.instance.effect_sound)
        {
            effect.sprite = on;
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        effect_text.text = "음소거";
                    }
                    break;
                case 1:
                    {
                        effect_text.text = "ミュート";
                    }
                    break;
                case 2:
                    {
                        effect_text.text = "mute";
                    }
                    break;
                case 3:
                    {
                        effect_text.text = "沉默的";
                    }
                    break;
            }
        }
        else
        {
            effect.sprite = off;
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        effect_text.text = "음소거 해제";
                    }
                    break;
                case 1:
                    {
                        effect_text.text = "ミュート解除";
                    }
                    break;
                case 2:
                    {
                        effect_text.text = "unmute";
                    }
                    break;
                case 3:
                    {
                        effect_text.text = "取消静音";
                    }
                    break;
            }
        }
    }

    void on_click_background()
    {
        if (!DataManager.instance.background_sound)
        {
            DataManager.instance.background_sound = true;
            background.sprite = on;
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        background_text.text = "음소거";
                    }
                    break;
                case 1:
                    {
                        background_text.text = "ミュート";
                    }
                    break;
                case 2:
                    {
                        background_text.text = "mute";
                    }
                    break;
                case 3:
                    {
                        background_text.text = "沉默的";
                    }
                    break;
            }
        }
        else
        {
            DataManager.instance.background_sound = false;
            background.sprite = off;
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        background_text.text = "음소거 해제";
                    }
                    break;
                case 1:
                    {
                        background_text.text = "ミュート解除";
                    }
                    break;
                case 2:
                    {
                        background_text.text = "unmute";
                    }
                    break;
                case 3:
                    {
                        background_text.text = "取消静音";
                    }
                    break;
            }
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
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        effect_text.text = "음소거";
                    }
                    break;
                case 1:
                    {
                        effect_text.text = "ミュート";
                    }
                    break;
                case 2:
                    {
                        effect_text.text = "mute";
                    }
                    break;
                case 3:
                    {
                        effect_text.text = "沉默的";
                    }
                    break;
            }
        }
        else
        {
            DataManager.instance.effect_sound = false;
            effect.sprite = off;
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        effect_text.text = "음소거 해제";
                    }
                    break;
                case 1:
                    {
                        effect_text.text = "ミュート解除";
                    }
                    break;
                case 2:
                    {
                        effect_text.text = "unmute";
                    }
                    break;
                case 3:
                    {
                        effect_text.text = "取消静音";
                    }
                    break;
            }
        }
        HomeSoundManager.instance.mute_effect(!DataManager.instance.effect_sound);
        DataManager.instance.save_sound_data();
    }
}
