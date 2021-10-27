using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStamp : MonoBehaviour
{
    public static TimeSpan time_span;

    void Start()
    {
        time_span = GetTimeSpan();
        Debug.Log("TimeStamp time_span: " + time_span);
    }

    public static void check_next_day()
    {
        //DateTime now = DateTime.Now;
        //if (now.ToString("yyyy-MM-dd") != DataManager.instance.login_time.ToString("yyyy-MM-dd"))
        //{
        //    DataManager.instance.reset_day_data();
        //    if (now.ToString("yyyy-MM") != DataManager.instance.login_time.ToString("yyyy-MM"))
        //    {
        //        DataManager.instance.reset_month_data();
        //    }
        //    DataManager.instance.load_my_data();
        //    DataManager.instance.login_time = now;
        //}
    }

    public static TimeSpan GetTimeSpan()
    {
        DateTime nowTime = DateTime.Now;
        DateTime severTime = DateTime.UtcNow;
        return nowTime - severTime;
    }

    public static string GetUnixTimeStamp()
    {
        TimeSpan time = (DateTime.UtcNow - new DateTime(1970, 1, 1));
        return Math.Floor(time.TotalSeconds).ToString();
    }

    public static string GetDayTime()
    {
        string time = DateTime.Now.ToString("yyyy-MM-dd");
        return time;
    }

    public static string GetNowTime()
    {
        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        return time;
    }


    public static void compair_login_date(DateTime before_login_time)
    {
        try
        {
            Debug.Log("before_login_time: " + before_login_time.ToString("yyyy-MM-dd H:mm"));
            if (before_login_time.ToString("yyyy-MM") != DateTime.UtcNow.Add(time_span).ToString("yyyy-MM"))
            {
                Debug.Log("compair_login_date other_month");
                DataManager.instance.other_day = true;
                DataManager.instance.other_month = true;
            }
            else if (before_login_time.ToString("yyyy-MM-dd") != DateTime.UtcNow.Add(time_span).ToString("yyyy-MM-dd"))
            {
                Debug.Log("compair_login_date other_day");
                DataManager.instance.other_day = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("compair_login_date error: " + e);
        }
    }
}
