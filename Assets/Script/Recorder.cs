using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum REPLAY_MODE : short
{
    OMOKTUBE,
    PERSONAL
}

public class Recorder : SingletonMonobehaviour<Recorder>
{
    public List<Record> play_data = new List<Record>();
    public List<RecordList> game_play_data = new List<RecordList>();

    public GameRecord replay_record = null;

    public REPLAY_MODE replay_mode;

    public string omoktube_replay_key;
    public string omoktube_replay_title;

    public float play_game_time = 0f;
    IEnumerator timeStamp;
     
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        replay_record = null;
    }

    void reset()
    {
        play_data.Clear();
    }

    public void record_start()
    {
        play_game_time = 0f;
        timeStamp = OnTimeStamp();
        StartCoroutine(timeStamp);
    }

    public void stop_record()
    {
        if (timeStamp != null)
        {
            StopCoroutine(timeStamp);
        }
        reset();
    }

    public void save_record(List<string> packet)
    {
        play_data.Add(new Record(play_game_time, Clone(packet)));
    }

    public void save_game_play_record(byte winner)
    {
        game_play_data.Add(new RecordList(Clone(play_data), DateTime.Now.ToString("yyyy-MM-dd H:mm"), winner));
        stop_record();
    }

    public void save_game_record(string mode, int win_count, int lose_count, int tie_count)
    {
        if (ProfileManager.instance == null || game_play_data.Count == 0 || (win_count == 0 && lose_count == 0 && tie_count == 0))
        {
            game_play_data.Clear();
            return;
        }

        PlayerData my_data = ProfileManager.instance.get_player_data(0);
        PlayerData other_data = ProfileManager.instance.get_player_data(1);

        string key = DataManager.instance.accountID + TimeStamp.GetUnixTimeStamp();
        string score = "";

        switch (DataManager.instance.language)
        {
            case 0:
                {
                    score = win_count + lose_count + tie_count + "전 " + win_count + "승 " + lose_count + "패";
                    if (tie_count > 0)
                    {
                        score += " " + tie_count + "무";
                    }
                }
                break;
            case 1:
                {
                    score = win_count + lose_count + tie_count + "戦" + win_count + "勝" + lose_count + "敗";
                    if (tie_count > 0)
                    {
                        score += tie_count + "分け";
                    }
                }
                break;
            case 2:
                {
                    score = win_count + lose_count + tie_count + " matches " + win_count + " win " + lose_count + " lose";
                    if (tie_count > 0)
                    {
                        score += " " + tie_count + " tie";
                    }
                }
                break;
            case 3:
                {
                    score = win_count + lose_count + tie_count + "场比赛" + win_count + "胜" + lose_count + "负";
                    if (tie_count > 0)
                    {
                        score += " " + tie_count + "平";
                    }
                }
                break;
        }

        GameRecord save_record = new GameRecord(key, mode, score,
            my_data.name, my_data.country, my_data.tier, my_data.type,
            other_data.name, other_data.country, other_data.tier, other_data.type,
           Clone(game_play_data));

        game_play_data.Clear();
        stop_record();

        DataManager.instance.save_my_game_record(save_record);

        if (GameManager.instance.play_mode == PLAY.PVP)
        {
            //오튜브 게스트 등급이 호스트 등급으로 보여지는 오류 수정
            upload_to_omoktube(my_data.name + " " + Converter.tier_to_string(my_data.tier) + " vs " + other_data.name + " " +Converter.tier_to_string(other_data.tier), save_record);
        }
    }

    IEnumerator OnTimeStamp()
    {
        while (true)
        {
            play_game_time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    public void upload_to_omoktube(string title, GameRecord gameRecord)
    {
        FirebaseManager.instance.upload_to_omoktube(gameRecord.key, title, gameRecord.score, gameRecord.type_index, convert_to_json(gameRecord));
    }

    public static string convert_to_json(GameRecord recode)
    {
        string json = JsonUtility.ToJson(recode);
        return json;
    }

    public static GameRecord convert_to_game_record(string json)
    {
        GameRecord my_recode = JsonUtility.FromJson<GameRecord>(json);
        return my_recode;
    }

    public static List<T> Clone<T>(List<T> List)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, List);
            stream.Position = 0;
            return (List<T>)formatter.Deserialize(stream);
        }
        catch (Exception e)
        {
            Debug.Log("CloneList Error: " + e);
            return null;
        }
    }
}

[System.Serializable]
public class GameRecord
{
    public string key;

    public string type_index;
    public string score;

    public string my_name;
    public COUNTRY my_country;
    public TIER my_tier;
    public PLAYER_TYPE my_type;

    public string other_name;
    public COUNTRY other_country;
    public TIER other_tier;
    public PLAYER_TYPE other_type;

    public List<RecordList> record;

    public GameRecord(string key, string type_index, string score, string my_name, COUNTRY my_country, TIER my_tier, PLAYER_TYPE my_type,
        string other_name, COUNTRY other_country, TIER other_tier, PLAYER_TYPE other_type, List<RecordList> record)
    {
        this.key = key;

        this.type_index = type_index;
        this.score = score;

        this.my_name = my_name;
        this.my_country = my_country;
        this.my_tier = my_tier;
        this.my_type = my_type;

        this.other_name = other_name;
        this.other_country = other_country;
        this.other_tier = other_tier;
        this.other_type = other_type;

        this.record = record;
    }
}

[System.Serializable]
public class RecordList
{
    public List<Record> record;
    public string play_time;
    public byte winner;
    public int score;
    public int double_value;
    public string money;

    public RecordList(List<Record> record, string play_time, byte winner)
    {
        this.record = record;
        this.play_time = play_time;
        this.winner = winner;
    }
}

[System.Serializable]
public class Record
{
    public float time;
    public List<string> packet;

    public Record(float _time, List<string> _packet)
    {
        this.time = _time;
        this.packet = _packet;
    }
}
