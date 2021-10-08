using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Converter
{
    public static byte to_byte(string before)
    {
        byte after = Convert.ToByte(before);
        return after;
    }

    public static int to_int(string before)
    {
        int after = Convert.ToInt32(before);
        return after;
    }

    public static PROTOCOL to_protocol(string before)
    {
        PROTOCOL after = (PROTOCOL)Convert.ToInt32(before);
        return after;
    }

    public static PLAYER_TYPE to_player_type(string before)
    {
        PLAYER_TYPE after = (PLAYER_TYPE)Convert.ToInt32(before);
        return after;
    }

    public static STATE to_state(string before)
    {
        STATE after = (STATE)Convert.ToInt32(before);
        return after;
    }

    public static ILLEGAL_MOVE to_illegal_move(string before)
    {
        ILLEGAL_MOVE after = (ILLEGAL_MOVE)Convert.ToInt32(before);
        return after;
    }

    public static EVENT_ITEM to_item(string item)
    {
        return (EVENT_ITEM)Enum.Parse(typeof(EVENT_ITEM), item);
    }


    public static string rating_to_string(RATING rating)
    {
        switch (rating)
        {
            case RATING.NOMAL:
                {
                    return "�Ϲ�ȸ��";
                }
            case RATING.VIP:
                {
                    return "VIPȸ��";
                }
            default:
                {
                    return "";
                }
        }
    }

    public static RATING to_rating(string rating)
    {
        return (RATING)Enum.Parse(typeof(RATING), rating);
    }

    public static string tier_to_string(TIER tier)
    {
        switch (tier)
        {
            case TIER.GRADE_12TH:
                {
                    return "12��";
                }

            case TIER.GRADE_11TH:
                {
                    return "11��";
                }

            case TIER.GRADE_10TH:
                {
                    return "10��";
                }

            case TIER.GRADE_9TH:
                {
                    return "9��";
                }

            case TIER.GRADE_8TH:
                {
                    return "8��";
                }

            case TIER.GRADE_7TH:
                {
                    return "7��";
                }

            case TIER.GRADE_6TH:
                {
                    return "6��";
                }

            case TIER.GRADE_5TH:
                {
                    return "5��";
                }

            case TIER.GRADE_4TH:
                {
                    return "4��";
                }

            case TIER.GRADE_3TH:
                {
                    return "3��";
                }

            case TIER.GRADE_2TH:
                {
                    return "2��";
                }

            case TIER.GRADE_1TH:
                {
                    return "1��";
                }

            case TIER.SUJOL:
                {
                    return "�ʴ�";
                }

            case TIER.YAKWOO:
                {
                    return "2��";
                }

            case TIER.TULYEOK:
                {
                    return "3��";
                }

            case TIER.SOGYO:
                {
                    return "4��";
                }

            case TIER.YONGJI:
                {
                    return "5��";
                }

            case TIER.TONGYU:
                {
                    return "6��";
                }

            case TIER.GUCHE:
                {
                    return "7��";
                }

            case TIER.JWAJO:
                {
                    return "8��";
                }

            case TIER.RIW:
                {
                    return "9��";
                }

            default:
                {
                    return "����";
                }
        }
    }

    public static TIER to_tier(string tier)
    {
        return (TIER)Enum.Parse(typeof(TIER), tier);
    }

    public static string old_to_string(OLD old)
    {
        switch (old)
        {
            case OLD.TEN:
                {
                    return "10��";
                }

            case OLD.TWENTY:
                {
                    return "20��";
                }

            case OLD.THIRTY:
                {
                    return "30��";
                }

            case OLD.FORTY:
                {
                    return "40��";
                }

            case OLD.FIFTY:
                {
                    return "50��";
                }

            case OLD.SIXTY:
                {
                    return "60��";
                }

            case OLD.SEVENTY:
                {
                    return "70��";
                }

            default:
                {
                    return "";
                }
        }
    }

    public static OLD to_old(string old)
    {
        return (OLD)Enum.Parse(typeof(OLD), old);
    }

    public static string gender_to_string(GENDER old)
    {
        switch (old)
        {
            case GENDER.MALE:
                {
                    return "��";
                }

            case GENDER.FEMALE:
                {
                    return "��";
                }

            default:
                {
                    return "";
                }
        }
    }

    public static GENDER to_gender(string gender)
    {
        return (GENDER)Enum.Parse(typeof(GENDER), gender);
    }

    public static string country_to_string(COUNTRY country)
    {
        switch (country)
        {
            case COUNTRY.KOREA:
                {
                    return "���ѹα�";
                }

            case COUNTRY.JAPAN:
                {
                    return "�Ϻ�";
                }

            case COUNTRY.CHINA:
                {
                    return "�߱�";
                }

            default:
                {
                    return "";
                }
        }
    }

    public static COUNTRY to_country(string gender)
    {
        return (COUNTRY)Enum.Parse(typeof(COUNTRY), gender);
    }

    public static string player_type_to_string(PLAYER_TYPE type)
    {
        switch (type)
        {
            case PLAYER_TYPE.BLACK:
                {
                    return "���";
                }

            case PLAYER_TYPE.WHITE:
                {
                    return "���";
                }

            default:
                {
                    return "";
                }
        }
    }

    public static string emoticon_to_string(EMOTICON emoticon)
    {
        switch (emoticon)
        {
            case EMOTICON.HELLO:
                {
                    return "�ȳ��ϼ���";
                }
            case EMOTICON.HURRY:
                {
                    return "�������ּ���";
                }
            case EMOTICON.ZEALOUSLY:
                {
                    return "�������غ���";
                }
            case EMOTICON.HELP:
                {
                    return "����ּ���";
                }
            case EMOTICON.WELL:
                {
                    return "���Ͻó׿�";
                }
            default:
                {
                    return "";
                }
        }
    }
}
