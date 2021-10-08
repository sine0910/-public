using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum RATING
{
    NOMAL,
    VIP
}

public class DataManager : SingletonMonobehaviour<DataManager>
{
    [System.Serializable]
    public class AccountData
    {
        public string acountID;
    }
    public AccountData account_data;

    [System.Serializable]
    public class PlayData
    {
        public int heart;

        public string noticeToken;

        public string my_name;
        public RATING my_rating;
        public TIER my_tier;
        public GENDER my_gender;
        public OLD my_old;
        public COUNTRY my_country;

        public int rating_score;

        public bool background_sound;
        public bool effect_sound;

        public byte preview;

        //흑 관련 점수
        public int b_win_count = 0;
        public int b_lose_count = 0;
        public int b_tie_count = 0;

        //백 관련 점수
        public int w_win_count = 0;
        public int w_lose_count = 0;
        public int w_tie_count = 0;

        //흑 관련 점수
        public int b_month_win_count = 0;
        public int b_month_lose_count = 0;
        public int b_month_tie_count = 0;

        //백 관련 점수
        public int w_month_win_count = 0;
        public int w_month_lose_count = 0;
        public int w_month_tie_count = 0;

        //흑 관련 점수
        public int b_day_win_count = 0;
        public int b_day_lose_count = 0;
        public int b_day_tie_count = 0;

        //백 관련 점수
        public int w_day_win_count = 0;
        public int w_day_lose_count = 0;
        public int w_day_tie_count = 0;
    }
    public PlayData play_data;

    [System.Serializable]
    public class FriendData
    {
        public List<string> my_friend_id_list;
    }
    public FriendData friend_data;

    [System.Serializable]
    public class NoticeData
    {
        public List<string> notice_key_list;
    }
    public NoticeData notice_data;

    [System.Serializable]
    public class EventData
    {
        public List<string> server_event_key_list;
    }
    public EventData event_data;

    [System.Serializable]
    public class ChatQuestionData
    {
        public List<string> chat_question_key_list;
    }
    public ChatQuestionData question_data;

    [System.Serializable]
    public class PlayRecodData
    {
        public List<GameRecord> game_recorde = new List<GameRecord>();
    }
    public PlayRecodData recordData;

    public string accountID;

    public string noticeToken;

    public int my_heart;

    public string my_name;
    public RATING my_rating;
    public TIER my_tier;
    public OLD my_old;
    public GENDER my_gender;
    public COUNTRY my_country;

    //흑 관련 점수
    public int b_win_count = 0;
    public int b_lose_count = 0;
    public int b_tie_count = 0;

    //백 관련 점수
    public int w_win_count = 0;
    public int w_lose_count = 0;
    public int w_tie_count = 0;

    //흑 관련 점수
    public int b_month_win_count = 0;
    public int b_month_lose_count = 0;
    public int b_month_tie_count = 0;

    //백 관련 점수
    public int w_month_win_count = 0;
    public int w_month_lose_count = 0;
    public int w_month_tie_count = 0;

    //흑 관련 점수
    public int b_day_win_count = 0;
    public int b_day_lose_count = 0;
    public int b_day_tie_count = 0;

    //백 관련 점수
    public int w_day_win_count = 0;
    public int w_day_lose_count = 0;
    public int w_day_tie_count = 0;

    public int rating_score;

    public bool background_sound;
    public bool effect_sound;

    public byte preview = 1; 

    public List<string> my_friend_id_list = new List<string>();
    public List<string> notice_key_list = new List<string>();
    public List<string> chat_question_key_list = new List<string>();
    public List<GameRecord> my_game_record = new List<GameRecord>();

    public bool other_day;
    public bool other_month;

    public DateTime login_time;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        load_my_account();
    }

    public void save_my_account_data(string account_id)
    {
        Debug.Log("save_my_account_data " + account_id);

        accountID = account_id;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/AccountDataFile.data");

        account_data.acountID = accountID;

        bf.Serialize(file, account_data);
        file.Close();
    }

    public void save_heart()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.heart = my_heart;

        bf.Serialize(file, play_data);
        file.Close();
    }

    public void save_my_data()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.my_name = my_name;
        play_data.my_tier = my_tier;
        play_data.my_old = my_old;
        play_data.my_gender = my_gender;
        play_data.my_country = my_country;

        play_data.b_win_count = b_win_count;
        play_data.b_lose_count = b_lose_count;
        play_data.b_tie_count = b_tie_count;

        play_data.w_win_count = w_win_count;
        play_data.w_lose_count = w_lose_count;
        play_data.w_tie_count = w_tie_count;

        play_data.b_month_win_count = b_month_win_count;
        play_data.b_month_lose_count = b_month_lose_count;
        play_data.b_month_tie_count = b_month_tie_count;

        play_data.w_month_win_count = w_month_win_count;
        play_data.w_month_lose_count = w_month_lose_count;
        play_data.w_month_tie_count = w_month_tie_count;

        play_data.b_day_win_count = b_day_win_count;
        play_data.b_day_lose_count = b_day_lose_count;
        play_data.b_day_tie_count = b_day_tie_count;

        play_data.w_win_count = w_win_count;
        play_data.w_lose_count = w_lose_count;
        play_data.w_tie_count = w_tie_count;

        play_data.rating_score = rating_score;

        play_data.background_sound = true;
        play_data.effect_sound = true;

        play_data.heart = my_heart;

        bf.Serialize(file, play_data);
        file.Close();
    }

    public void save_my_name_data(string name)
    {
        Debug.Log("save_my_name_data " + name);

        my_name = name;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.my_name = my_name;

        bf.Serialize(file, play_data);
        file.Close();

        FirebaseManager.instance.update_my_data();
    }

    public void save_my_rating_data()
    {
        Debug.Log("save_my_tiesave_my_rating_datar_data");

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.my_rating = my_rating;

        bf.Serialize(file, play_data);
        file.Close();

        FirebaseManager.instance.update_my_data();
    }

    public void save_my_tier_data(TIER tier)
    {
        Debug.Log("save_my_tier_data " + tier.ToString());

        my_tier = tier;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.my_tier = my_tier;

        bf.Serialize(file, play_data);
        file.Close();

        FirebaseManager.instance.update_my_data();
    }

    public void save_my_old_data(OLD old)
    {
        Debug.Log("save_my_old_data " + old.ToString());

        my_old = old;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.my_old = my_old;

        bf.Serialize(file, play_data);
        file.Close();

        FirebaseManager.instance.update_my_data();
    }

    public void save_my_gender_data(GENDER gender)
    {
        Debug.Log("save_my_gender_data " + gender.ToString());

        my_gender = gender;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.my_gender = my_gender;

        bf.Serialize(file, play_data);
        file.Close();

        FirebaseManager.instance.update_my_data();
    }

    public void save_my_country_data(COUNTRY country)
    {
        Debug.Log("save_my_country_data " + country.ToString());

        my_country = country;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.my_country = my_country;

        bf.Serialize(file, play_data);
        file.Close();

        FirebaseManager.instance.update_my_data();
    }

    public void save_my_play_data()
    {
        if (rating_score < 0)
        {
            rating_score = 0;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.b_win_count = b_win_count;
        play_data.b_lose_count = b_lose_count;
        play_data.b_tie_count = b_tie_count;

        play_data.w_win_count = w_win_count;
        play_data.w_lose_count = w_lose_count;
        play_data.w_tie_count = w_tie_count;

        play_data.b_month_win_count = b_month_win_count;
        play_data.b_month_lose_count = b_month_lose_count;
        play_data.b_month_tie_count = b_month_tie_count;

        play_data.w_month_win_count = w_month_win_count;
        play_data.w_month_lose_count = w_month_lose_count;
        play_data.w_month_tie_count = w_month_tie_count;

        play_data.b_day_win_count = b_day_win_count;
        play_data.b_day_lose_count = b_day_lose_count;
        play_data.b_day_tie_count = b_day_tie_count;

        play_data.w_win_count = w_win_count;
        play_data.w_lose_count = w_lose_count;
        play_data.w_tie_count = w_tie_count;

        play_data.rating_score = rating_score;

        bf.Serialize(file, play_data);
        file.Close();

        FirebaseManager.instance.update_my_data();
    }

    public void win(PLAYER_TYPE type)
    {
        switch (type)
        {
            case PLAYER_TYPE.BLACK:
                {
                    b_win_count++;
                    b_month_win_count++;
                    b_day_win_count++;
                }
                break;

            case PLAYER_TYPE.WHITE:
                {
                    w_win_count++;
                    w_month_win_count++;
                    w_day_win_count++;
                }
                break;
        }

        rating_score += 2;

        save_my_play_data();
    }

    public void lose(PLAYER_TYPE type)
    {
        switch (type)
        {
            case PLAYER_TYPE.BLACK:
                {
                    b_lose_count++;
                    b_month_lose_count++;
                    b_day_lose_count++;
                }
                break;

            case PLAYER_TYPE.WHITE:
                {
                    w_lose_count++;
                    w_month_lose_count++;
                    w_day_lose_count++;
                }
                break;
        }

        rating_score -= 2;

        save_my_play_data();
    }

    public void tie(PLAYER_TYPE type)
    {
        switch (type)
        {
            case PLAYER_TYPE.BLACK:
                {
                    b_tie_count++;
                    b_month_tie_count++;
                    b_day_tie_count++;
                }
                break;

            case PLAYER_TYPE.WHITE:
                {
                    w_tie_count++;
                    w_month_tie_count++;
                    w_day_tie_count++;
                }
                break;
        }

        rating_score += 1;

        save_my_play_data();
    }

    public void save_sound_data()
    {
        Debug.Log("save_sound_data");

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.background_sound = background_sound;
        play_data.effect_sound = effect_sound;

        bf.Serialize(file, play_data);
        file.Close();
    }

    public void save_preview_data()
    {
        Debug.Log("save_preview_data");

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.preview = preview;

        bf.Serialize(file, play_data);
        file.Close();
    }

    public void save_my_notice_token_data(string token)
    {
        Debug.Log("save_my_notice_token_data " + token);

        noticeToken = token;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.noticeToken = noticeToken;

        bf.Serialize(file, play_data);
        file.Close();
    }

    public void save_my_friend_data()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "FriendFile.data");

        friend_data.my_friend_id_list = my_friend_id_list;

        bf.Serialize(file, friend_data);
        file.Close();
    }

    public void save_notice_data()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "NoticeFile.data");

        notice_data.notice_key_list = notice_key_list;

        bf.Serialize(file, notice_data);
        file.Close();
    }

    public void save_chat_question_data()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "ChatFile.data");

        question_data.chat_question_key_list = chat_question_key_list;

        bf.Serialize(file, question_data);
        file.Close();
    }

    public void save_event_get_result_key_data(List<string> server_event_key_list)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "EventFile.data");

        event_data.server_event_key_list = server_event_key_list;

        bf.Serialize(file, event_data);
        file.Close();
    }

    public void save_my_game_record(GameRecord record)
    {
        my_game_record.Add(record);

        if (my_game_record.Count > 50)
        {
            my_game_record.RemoveAt(0);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayRecodeData.data");

        recordData.game_recorde = my_game_record;

        bf.Serialize(file, recordData);
        file.Close();
    }

    public void load_my_account()
    {
        if (File.Exists(Application.persistentDataPath + "/AccountDataFile.data"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/AccountDataFile.data", FileMode.Open);

            if (file != null && file.Length > 0)
            {
                account_data = (AccountData)bf.Deserialize(file);

                accountID = account_data.acountID;
            }
            file.Close();
        }
    }

    public void load_my_data()
    {
        Debug.Log("load_my_data");

        if (other_day)
        {
            reset_day_data();
        }
        if (other_month)
        {
            reset_month_data();
        }

        if (File.Exists(Application.persistentDataPath + "/" + accountID + "PlayFile.data"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + accountID + "PlayFile.data", FileMode.Open);

            try
            {
                if (file != null && file.Length > 0)
                {
                    play_data = (PlayData)bf.Deserialize(file);

                    my_heart += play_data.heart;

                    noticeToken = play_data.noticeToken;

                    my_name = play_data.my_name;
                    my_rating = play_data.my_rating;
                    my_country = play_data.my_country;
                    my_tier = play_data.my_tier;
                    my_gender = play_data.my_gender;
                    my_old = play_data.my_old;

                    b_win_count = play_data.b_win_count;
                    b_lose_count = play_data.b_lose_count;
                    b_tie_count = play_data.b_tie_count;

                    w_win_count = play_data.w_win_count;
                    w_lose_count = play_data.w_lose_count;
                    w_tie_count = play_data.w_tie_count;

                    b_month_win_count = play_data.b_month_win_count;
                    b_month_lose_count = play_data.b_month_lose_count;
                    b_month_tie_count = play_data.b_month_tie_count;

                    w_month_win_count = play_data.w_month_win_count;
                    w_month_lose_count = play_data.w_month_lose_count;
                    w_month_tie_count = play_data.w_month_tie_count;

                    b_day_win_count = play_data.b_day_win_count;
                    b_day_lose_count = play_data.b_day_lose_count;
                    b_day_tie_count = play_data.b_day_tie_count;

                    w_day_win_count = play_data.w_day_win_count;
                    w_day_lose_count = play_data.w_day_lose_count;
                    w_day_tie_count = play_data.w_day_tie_count;

                    rating_score = play_data.rating_score;

                    background_sound = play_data.background_sound;
                    effect_sound = play_data.effect_sound;

                    //preview 변수는 추후에 생긴 것으로 기존 사용자들의 데이터에 연동되도록 조건을 추가한다
                    if (play_data.preview != 0)
                    {
                        preview = play_data.preview;
                    }
                    else
                    {
                        preview = 1;
                    }
                }
                file.Close();
            }
            catch (Exception e)
            {
                preview = 1;
                my_heart = 50;

                file.Close();

                Debug.Log("load my data load error!: " + e);
            }
        }
        else
        {
            set_default_data();
        }
    }

    public void reset_day_data()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.b_day_win_count = 0;
        play_data.b_day_lose_count = 0;
        play_data.b_day_tie_count = 0;

        play_data.w_day_win_count = 0;
        play_data.w_day_lose_count = 0;
        play_data.w_day_tie_count = 0;

        bf.Serialize(file, recordData);
        file.Close();
    }

    public void reset_month_data()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.b_month_win_count = 0;
        play_data.b_month_lose_count = 0;
        play_data.b_month_tie_count = 0;

        play_data.w_month_win_count = 0;
        play_data.w_month_lose_count = 0;
        play_data.w_month_tie_count = 0;

        bf.Serialize(file, recordData);
        file.Close();
    }

    public void set_default_data()
    {
        //초기 시작 데이터를 부여한다.
        my_tier = TIER.GRADE_12TH;
        my_gender = GENDER.NONE;
        my_old = OLD.NONE;

        background_sound = true;
        effect_sound = true;

        my_heart = 50;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + accountID + "PlayFile.data");

        play_data.my_tier = my_tier;
        play_data.my_gender = my_gender;
        play_data.my_old = my_old;

        play_data.background_sound = background_sound;
        play_data.effect_sound = effect_sound;

        play_data.heart = my_heart;

        bf.Serialize(file, play_data);
        file.Close();
    }

    public void load_friend_data()
    {
        if (File.Exists(Application.persistentDataPath + "/" + accountID + "FriendFile.data"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + accountID + "FriendFile.data", FileMode.Open);

            if (file != null && file.Length > 0)
            {
                friend_data = (FriendData)bf.Deserialize(file);

                my_friend_id_list = friend_data.my_friend_id_list;
            }
            file.Close();
        }
    }

    public void load_notice_data()
    {
        if (File.Exists(Application.persistentDataPath + "/" + accountID + "NoticeFile.data"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + accountID + "NoticeFile.data", FileMode.Open);

            if (file != null && file.Length > 0)
            {
                notice_data = (NoticeData)bf.Deserialize(file);

                notice_key_list = notice_data.notice_key_list;
            }
            file.Close();
        }
    }

    public void load_question_data()
    {
        if (File.Exists(Application.persistentDataPath + "/" + accountID + "ChatFile.data"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + accountID + "ChatFile.data", FileMode.Open);

            if (file != null && file.Length > 0)
            {
                question_data = (ChatQuestionData)bf.Deserialize(file);

                chat_question_key_list = question_data.chat_question_key_list;
            }
            file.Close();
        }
    }

    public List<string> load_event_data()
    {
        //서버에서 받은 이벤트의 키를 저장 후 다시 가져옴(중복 수령 방지)
        List<string> event_key_list = new List<string>();

        if (File.Exists(Application.persistentDataPath + "/" + accountID + "EventFile.data"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + accountID + "EventFile.data", FileMode.Open);

            if (file != null && file.Length > 0)
            {
                event_data = (EventData)bf.Deserialize(file);

                event_key_list = event_data.server_event_key_list;
            }
            file.Close();
        }
        return event_key_list;
    }

    public void load_my_game_record()
    {
        if (File.Exists(Application.persistentDataPath + "/" + DataManager.instance.accountID + "PlayRecodeData.data"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + accountID + "PlayRecodeData.data", FileMode.Open);

            if (file != null && file.Length > 0)
            {
                recordData = (PlayRecodData)bf.Deserialize(file);

                my_game_record = recordData.game_recorde;
            }
            file.Close();
        }
    }

    public byte check_my_data()
    {
        if (my_name == null || my_name == "")
        {
            return 1;
        }
        else if (my_old == OLD.NONE)
        {
            return 2;
        }
        else if (my_gender == GENDER.NONE)
        {
            return 3;
        }
        //추후 국가 설정 추가
        else if (my_country == COUNTRY.NONE)
        {
            return 4;
        }
        else
        {
            return 0;
        }
    }

    public void account_logout()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/AccountDataFile.data");

        account_data.acountID = "";

        bf.Serialize(file, play_data);
        file.Close();
    }

    public void reset_my_data()
    {
        try
        {
            File.Delete(Application.persistentDataPath + "/" + accountID + "PlayFile.data");
        }
        catch (Exception e)
        {
            Debug.Log("reset_my_data error: " + e);
        }
    }
}
