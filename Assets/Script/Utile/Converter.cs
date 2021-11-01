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
        try
        { 
            return (EVENT_ITEM)Enum.Parse(typeof(EVENT_ITEM), item);
        }
        catch (Exception e)
        {
            Debug.Log("to_item error: " + e);
            return EVENT_ITEM.HEART;
        }
    }


    public static string rating_to_string(RATING rating)
    {
        switch (rating)
        {
            case RATING.NOMAL:
                {
                    string val = "";

                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "일반";
                            }
                        case 1:
                            {
                                return "一般";
                            }
                        case 2:
                            {
                                return "None";
                            }
                        default:
                            {
                                return "普通的";
                            }
                    }
                }
            case RATING.VIP:
                {
                    return "VIP";
                }
            default:
                {
                    return "";
                }
        }
    }

    public static RATING to_rating(string rating)
    {
        try
        {
            return (RATING)Enum.Parse(typeof(RATING), rating);
        }
        catch (Exception e)
        {
            Debug.Log("to_rating error: " + e);
            return RATING.NOMAL;
        }
    }

    public static string tier_to_string(TIER tier)
    {
        switch (tier)
        {
            case TIER.GRADE_12TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "12급";
                            }
                        case 1:
                            {
                                return "12級";
                            }
                        case 2:
                            {
                                return "Grade 12";
                            }
                        default:
                            {
                                return "12级";
                            }
                    }
                }

            case TIER.GRADE_11TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "11급";
                            }
                        case 1:
                            {
                                return "11級";
                            }
                        case 2:
                            {
                                return "Grade 11";
                            }
                        default:
                            {
                                return "11级";
                            }
                    }
                }

            case TIER.GRADE_10TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "10급";
                            }
                        case 1:
                            {
                                return "10級";
                            }
                        case 2:
                            {
                                return "Grade 10";
                            }
                        default:
                            {
                                return "10级";
                            }
                    }
                }

            case TIER.GRADE_9TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "9급";
                            }
                        case 1:
                            {
                                return "9級";
                            }
                        case 2:
                            {
                                return "Grade 9";
                            }
                        default:
                            {
                                return "9级";
                            }
                    }
                }

            case TIER.GRADE_8TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "8급";
                            }
                        case 1:
                            {
                                return "8級";
                            }
                        case 2:
                            {
                                return "Grade 8";
                            }
                        default:
                            {
                                return "8级";
                            }
                    }
                }

            case TIER.GRADE_7TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "7급";
                            }
                        case 1:
                            {
                                return "7級";
                            }
                        case 2:
                            {
                                return "Grade 7";
                            }
                        default:
                            {
                                return "7级";
                            }
                    }
                }

            case TIER.GRADE_6TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "6급";
                            }
                        case 1:
                            {
                                return "6級";
                            }
                        case 2:
                            {
                                return "Grade 6";
                            }
                        default:
                            {
                                return "6级";
                            }
                    }
                }

            case TIER.GRADE_5TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "5급";
                            }
                        case 1:
                            {
                                return "5級";
                            }
                        case 2:
                            {
                                return "Grade 5";
                            }
                        default:
                            {
                                return "5级";
                            }
                    }
                }

            case TIER.GRADE_4TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "4급";
                            }
                        case 1:
                            {
                                return "4級";
                            }
                        case 2:
                            {
                                return "Grade 4";
                            }
                        default:
                            {
                                return "4级";
                            }
                    }
                }

            case TIER.GRADE_3TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "3급";
                            }
                        case 1:
                            {
                                return "3級";
                            }
                        case 2:
                            {
                                return "Grade 3";
                            }
                        default:
                            {
                                return "3级";
                            }
                    }
                }

            case TIER.GRADE_2TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "2급";
                            }
                        case 1:
                            {
                                return "2級";
                            }
                        case 2:
                            {
                                return "Grade 2";
                            }
                        default:
                            {
                                return "2级";
                            }
                    }
                }

            case TIER.GRADE_1TH:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "1급";
                            }
                        case 1:
                            {
                                return "1級";
                            }
                        case 2:
                            {
                                return "Grade 1";
                            }
                        default:
                            {
                                return "1级";
                            }
                    }
                }

            case TIER.SUJOL:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "초단";
                            }
                        case 1:
                            {
                                return "一段";
                            }
                        case 2:
                            {
                                return "1st Lavel";
                            }
                        default:
                            {
                                return "一段";
                            }
                    }
                }

            case TIER.YAKWOO:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "2단";
                            }
                        case 1:
                            {
                                return "二段";
                            }
                        case 2:
                            {
                                return "2st Lavel";
                            }
                        default:
                            {
                                return "二段";
                            }
                    }
                }

            case TIER.TULYEOK:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "3단";
                            }
                        case 1:
                            {
                                return "三段";
                            }
                        case 2:
                            {
                                return "3st Lavel";
                            }
                        default:
                            {
                                return "三段";
                            }
                    }
                }

            case TIER.SOGYO:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "4단";
                            }
                        case 1:
                            {
                                return "四段";
                            }
                        case 2:
                            {
                                return "4st Lavel";
                            }
                        default:
                            {
                                return "四段";
                            }
                    }
                }

            case TIER.YONGJI:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "5단";
                            }
                        case 1:
                            {
                                return "五段";
                            }
                        case 2:
                            {
                                return "5st Lavel";
                            }
                        default:
                            {
                                return "五段";
                            }
                    }
                }

            case TIER.TONGYU:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "6단";
                            }
                        case 1:
                            {
                                return "六段";
                            }
                        case 2:
                            {
                                return "6st Lavel";
                            }
                        default:
                            {
                                return "六段";
                            }
                    }
                }

            case TIER.GUCHE:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "7단";
                            }
                        case 1:
                            {
                                return "七段";
                            }
                        case 2:
                            {
                                return "7st Lavel";
                            }
                        default:
                            {
                                return "七段";
                            }
                    }
                }

            case TIER.JWAJO:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "8단";
                            }
                        case 1:
                            {
                                return "八段";
                            }
                        case 2:
                            {
                                return "8st Lavel";
                            }
                        default:
                            {
                                return "八段";
                            }
                    }
                }

            case TIER.RIW:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "9단";
                            }
                        case 1:
                            {
                                return "九段";
                            }
                        case 2:
                            {
                                return "9st Lavel";
                            }
                        default:
                            {
                                return "九段";
                            }
                    }
                }

            default:
                {
                    return "";
                }
        }
    }

    public static TIER to_tier(string tier)
    {
        try
        {
            return (TIER)Enum.Parse(typeof(TIER), tier);
        }
        catch (Exception e)
        {
            Debug.Log("to_old error: " + e);
            return TIER.NONE;
        }
    }

    public static string old_to_string(OLD old)
    {
        switch (old)
        {
            case OLD.TEN:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "10대";
                            }
                        case 1:
                            {
                                return "10代";
                            }
                        case 2:
                            {
                                return "10's";
                            }
                        default:
                            {
                                return "10年代";
                            }
                    }
                }

            case OLD.TWENTY:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "20대";
                            }
                        case 1:
                            {
                                return "20代";
                            }
                        case 2:
                            {
                                return "20's";
                            }
                        default:
                            {
                                return "20年代";
                            }
                    }
                }

            case OLD.THIRTY:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "30대";
                            }
                        case 1:
                            {
                                return "30代";
                            }
                        case 2:
                            {
                                return "30's";
                            }
                        default:
                            {
                                return "30年代";
                            }
                    }
                }

            case OLD.FORTY:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "40대";
                            }
                        case 1:
                            {
                                return "40代";
                            }
                        case 2:
                            {
                                return "40's";
                            }
                        default:
                            {
                                return "40年代";
                            }
                    }
                }

            case OLD.FIFTY:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "50대";
                            }
                        case 1:
                            {
                                return "50代";
                            }
                        case 2:
                            {
                                return "50's";
                            }
                        default:
                            {
                                return "50年代";
                            }
                    }
                }

            case OLD.SIXTY:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "60대";
                            }
                        case 1:
                            {
                                return "60代";
                            }
                        case 2:
                            {
                                return "60's";
                            }
                        default:
                            {
                                return "60年代";
                            }
                    }
                }

            case OLD.SEVENTY:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "70대";
                            }
                        case 1:
                            {
                                return "70代";
                            }
                        case 2:
                            {
                                return "70's";
                            }
                        default:
                            {
                                return "70年代";
                            }
                    }
                }

            default:
                {
                    return "";
                }
        }
    }

    public static OLD to_old(string old)
    {
        try
        { 
            return (OLD)Enum.Parse(typeof(OLD), old);
        }
        catch (Exception e)
        {
            Debug.Log("to_old error: " + e);
            return OLD.NONE;
        }
    }

    public static string gender_to_string(GENDER old)
    {
        switch (old)
        {
            case GENDER.MALE:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "남";
                            }
                        case 1:
                            {
                                return "男";
                            }
                        case 2:
                            {
                                return "male";
                            }
                        default:
                            {
                                return "男性";
                            }
                    }
                }

            case GENDER.FEMALE:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "여";
                            }
                        case 1:
                            {
                                return "女";
                            }
                        case 2:
                            {
                                return "female";
                            }
                        default:
                            {
                                return "女性";
                            }
                    }
                }

            default:
                {
                    return "";
                }
        }
    }

    public static GENDER to_gender(string gender)
    {
        try
        {
            return (GENDER)Enum.Parse(typeof(GENDER), gender);
        }
        catch (Exception e)
        {
            Debug.Log("to_gender error: " + e);
            return GENDER.NONE;
        }
    }

    public static string country_to_string(COUNTRY country)
    {
        return CountryManager.country_to_string(country);
        //switch (country)
        //{
        //    case COUNTRY.KOREA:
        //        {
        //            switch (DataManager.instance.language)
        //            {
        //                case 0:
        //                    {
        //                        return "대한민국";
        //                    }
        //                case 1:
        //                    {
        //                        return "韓国";
        //                    }
        //                case 2:
        //                    {
        //                        return "Korea";
        //                    }
        //                default:
        //                    {
        //                        return "朝鲜";
        //                    }
        //            }
        //        }

        //    case COUNTRY.JAPAN:
        //        {
        //            switch (DataManager.instance.language)
        //            {
        //                case 0:
        //                    {
        //                        return "일본";
        //                    }
        //                case 1:
        //                    {
        //                        return "日本";
        //                    }
        //                case 2:
        //                    {
        //                        return "Japan";
        //                    }
        //                default:
        //                    {
        //                        return "日本";
        //                    }
        //            }
        //        }

        //    case COUNTRY.CHINA:
        //        {
        //            switch (DataManager.instance.language)
        //            {
        //                case 0:
        //                    {
        //                        return "중국";
        //                    }
        //                case 1:
        //                    {
        //                        return "中国";
        //                    }
        //                case 2:
        //                    {
        //                        return "China";
        //                    }
        //                default:
        //                    {
        //                        return "中国";
        //                    }
        //            }
        //        }

        //    default:
        //        {
        //            return "";
        //        }
        //}
    }

    public static COUNTRY to_country(string gender)
    {
        try
        {
            return (COUNTRY)Enum.Parse(typeof(COUNTRY), gender);
        }
        catch (Exception e)
        {
            Debug.Log("to_country error: " + e);
            return COUNTRY.NONE;
        }
    }

    public static string player_type_to_string(PLAYER_TYPE type)
    {
        switch (type)
        {
            case PLAYER_TYPE.BLACK:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "흑돌";
                            }
                        case 1:
                            {
                                return "黒石";
                            }
                        case 2:
                            {
                                return "Black";
                            }
                        default:
                            {
                                return "黑石";
                            }
                    }
                }

            case PLAYER_TYPE.WHITE:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "백돌";
                            }
                        case 1:
                            {
                                return "白石";
                            }
                        case 2:
                            {
                                return "White";
                            }
                        default:
                            {
                                return "白石";
                            }
                    }
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
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "안녕하세요";
                            }
                        case 1:
                            {
                                return "ハロー";
                            }
                        case 2:
                            {
                                return "Hello";
                            }
                        default:
                            {
                                return "你好";
                            }
                    }
                }
            case EMOTICON.HURRY:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "빨리해주세요";
                            }
                        case 1:
                            {
                                return "早くしてください";
                            }
                        case 2:
                            {
                                return "Please do it quickly";
                            }
                        default:
                            {
                                return "请快点做";
                            }
                    }
                }
            case EMOTICON.ZEALOUSLY:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "열심히해봐요";
                            }
                        case 1:
                            {
                                return "ハードみ";
                            }
                        case 2:
                            {
                                return "work hard";
                            }
                        default:
                            {
                                return "努力工作";
                            }
                    }
                }
            case EMOTICON.HELP:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "살려주세요";
                            }
                        case 1:
                            {
                                return "助けて下さい";
                            }
                        case 2:
                            {
                                return "Help me";
                            }
                        default:
                            {
                                return "帮我";
                            }
                    }
                }
            case EMOTICON.WELL:
                {
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                return "잘하시네요";
                            }
                        case 1:
                            {
                                return "上手ですね";
                            }
                        case 2:
                            {
                                return "you are doing well";
                            }
                        default:
                            {
                                return "你做得很好";
                            }
                    }
                }
            default:
                {
                    return "";
                }
        }
    }
}
