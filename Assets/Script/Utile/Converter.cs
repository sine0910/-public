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
                    return "일반회원";
                }
            case RATING.VIP:
                {
                    return "VIP회원";
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
                    return "12급";
                }

            case TIER.GRADE_11TH:
                {
                    return "11급";
                }

            case TIER.GRADE_10TH:
                {
                    return "10급";
                }

            case TIER.GRADE_9TH:
                {
                    return "9급";
                }

            case TIER.GRADE_8TH:
                {
                    return "8급";
                }

            case TIER.GRADE_7TH:
                {
                    return "7급";
                }

            case TIER.GRADE_6TH:
                {
                    return "6급";
                }

            case TIER.GRADE_5TH:
                {
                    return "5급";
                }

            case TIER.GRADE_4TH:
                {
                    return "4급";
                }

            case TIER.GRADE_3TH:
                {
                    return "3급";
                }

            case TIER.GRADE_2TH:
                {
                    return "2급";
                }

            case TIER.GRADE_1TH:
                {
                    return "1급";
                }

            case TIER.SUJOL:
                {
                    return "초단";
                }

            case TIER.YAKWOO:
                {
                    return "2단";
                }

            case TIER.TULYEOK:
                {
                    return "3단";
                }

            case TIER.SOGYO:
                {
                    return "4단";
                }

            case TIER.YONGJI:
                {
                    return "5단";
                }

            case TIER.TONGYU:
                {
                    return "6단";
                }

            case TIER.GUCHE:
                {
                    return "7단";
                }

            case TIER.JWAJO:
                {
                    return "8단";
                }

            case TIER.RIW:
                {
                    return "9단";
                }

            default:
                {
                    return "연습";
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
                    return "10대";
                }

            case OLD.TWENTY:
                {
                    return "20대";
                }

            case OLD.THIRTY:
                {
                    return "30대";
                }

            case OLD.FORTY:
                {
                    return "40대";
                }

            case OLD.FIFTY:
                {
                    return "50대";
                }

            case OLD.SIXTY:
                {
                    return "60대";
                }

            case OLD.SEVENTY:
                {
                    return "70대";
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
                    return "남";
                }

            case GENDER.FEMALE:
                {
                    return "여";
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
                    return "대한민국";
                }

            case COUNTRY.JAPAN:
                {
                    return "일본";
                }

            case COUNTRY.CHINA:
                {
                    return "중국";
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
                    return "흑목";
                }

            case PLAYER_TYPE.WHITE:
                {
                    return "백목";
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
                    return "안녕하세요";
                }
            case EMOTICON.HURRY:
                {
                    return "빨리해주세요";
                }
            case EMOTICON.ZEALOUSLY:
                {
                    return "열심히해봐요";
                }
            case EMOTICON.HELP:
                {
                    return "살려주세요";
                }
            case EMOTICON.WELL:
                {
                    return "잘하시네요";
                }
            default:
                {
                    return "";
                }
        }
    }
}
