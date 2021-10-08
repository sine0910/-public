using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSoundManager : SingletonMonobehaviour<HomeSoundManager>
{
    public AudioSource background;
    public AudioSource effect;

    AudioClip main_background;

    // Start is called before the first frame update
    void Start()
    {
        main_background = Resources.Load<AudioClip>("Sound/Background/Main_Soundtrack");

        if (DataManager.instance.background_sound)
        {
            background.mute = false;
        }
        else
        {
            background.mute = true;
        }

        if (DataManager.instance.effect_sound)
        {
            effect.mute = false;
        }
        else
        {
            effect.mute = true;
        }

        background.clip = main_background;
        background.Play();
        background.loop = true;
    }

    public void mute_background(bool on)
    {
        background.mute = on;
    }

    public void mute_effect(bool on)
    {
        effect.mute = on;
    }
}
