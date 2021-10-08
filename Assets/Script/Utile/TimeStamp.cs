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
}
