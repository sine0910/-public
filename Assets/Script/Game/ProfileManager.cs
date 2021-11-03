using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : SingletonMonobehaviour<ProfileManager>
{
    public List<PlayerProfile> profile_list = new List<PlayerProfile>();
    public List<PlayerData> profile_data_list = new List<PlayerData>();

    public PlayerProfile my_profile;
    public PlayerProfile other_profile;

    void Start()
    {
        my_profile = transform.Find("my_profile").GetComponent<PlayerProfile>();
        other_profile = transform.Find("other_profile").GetComponent<PlayerProfile>();

        profile_list.Add(my_profile);
        profile_list.Add(other_profile);

        for (int i = 0; i < profile_list.Count; i++)
        {
            PlayerData profile_data = GameManager.instance.get_player_data((byte)i);
            profile_data_list.Add(profile_data);
            profile_list[i].set_player_data(profile_data.type, profile_data.name, profile_data.tier, profile_data.country);
        }

        if(GameManager.instance.play_mode == PLAY.PVP && my_profile.ask_get_heart_profile())
        {
            my_profile.set_player_heart_text();
        }
    }

    public PlayerData get_player_data(int i)
    {
        return profile_data_list[i];
    }
}
