using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    public static readonly int BOARD_SIZE = 15;

    public byte black_index;
    public byte white_index;

    public PLAY play_mode;
    public RULE play_rule;

    public string game_room_id = "";
    public string other_account_id = "";
    public bool host = false;

    public List<PlayerData> player_data_list;

    public GameObject non_heart;

    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        player_data_list = new List<PlayerData>();
        player_data_list.Add(new PlayerData(PLAYER_TYPE.NONE, null, TIER.NONE, OLD.NONE, GENDER.NONE, COUNTRY.NONE));
        player_data_list.Add(new PlayerData(PLAYER_TYPE.NONE, null, TIER.NONE, OLD.NONE, GENDER.NONE, COUNTRY.NONE));

        black_index = 0;
        white_index = 1;
    }

    public void on_empty_heart()
    {
        if (non_heart.activeSelf)
        {
            non_heart.SetActive(false);
        }
        else
        {
            non_heart.SetActive(true);
        }
    }

    public void set_my_color(PLAYER_TYPE type)
    {
        switch(type)
        {
            case PLAYER_TYPE.BLACK:
                {
                    black_index = 0;
                    white_index = 1;
                }
                break;
            case PLAYER_TYPE.WHITE:
                {
                    white_index = 0;
                    black_index = 1;
                }
                break;
        }
    }

    public PLAYER_TYPE get_my_player_type()
    {
        if (black_index == 0)
        {
            return PLAYER_TYPE.BLACK;
        }
        else if(white_index == 0)
        {
            return PLAYER_TYPE.WHITE;
        }
        else
        {
            return PLAYER_TYPE.NONE;
        }
    }

    public PLAYER_TYPE get_other_player_type()
    {
        if (black_index == 1)
        {
            return PLAYER_TYPE.BLACK;
        }
        else if (white_index == 1)
        {
            return PLAYER_TYPE.WHITE;
        }
        else
        {
            return PLAYER_TYPE.NONE;
        }
    }

    public void set_player_data(byte player_index, PLAYER_TYPE type, string name, TIER tier, OLD old, GENDER gender, COUNTRY country)
    {
        player_data_list[player_index] = new PlayerData(type, name, tier, old, gender, country);
    }

    public PlayerData get_player_data(byte player_index)
    {
        return player_data_list[player_index];
    }
}

public class PlayerData
{
    public PLAYER_TYPE type;
    public string name;
    public TIER tier;
    public OLD old;
    public GENDER gender;
    public COUNTRY country;

    public PlayerData(PLAYER_TYPE type, string name, TIER tier, OLD old, GENDER gender, COUNTRY country)
    {
        this.type = type;
        this.name = name;
        this.tier = tier;
        this.old = old;
        this.gender = gender;
        this.country = country;
    }
}