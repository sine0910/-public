using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : SingletonMonobehaviour<GameSoundManager>
{
    public AudioSource background;
    public AudioSource effect;

    AudioClip main_background;

    AudioClip put_ston;

    // Start is called before the first frame update
    void Start()
    {
        put_ston = Resources.Load<AudioClip>("Sound/put_ston");

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
    }

    public void mute_background(bool on)
    {
        background.mute = on;
    }

    public void mute_effect(bool on)
    {
        effect.mute = on;
    }

    public void put_ston_effect()
    {
        effect.PlayOneShot(put_ston);
    }
}
