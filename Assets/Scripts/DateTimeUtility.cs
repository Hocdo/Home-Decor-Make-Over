using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static class DateTimeUtility
{
    public static DateTime LastOfWeek(this DateTime dt)
    {
        var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
        var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
        if (diff < 0)
            diff += 7;

        return dt.AddDays(-diff).Date;
    }

    public static DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }

    public static DateTime GetNow()
    {
        return DateTime.Now;
    }

    public static DateTime Get(string s)
    {
        DateTime.TryParse(s, out DateTime dateTime);
        return dateTime;
    }

    public static void ToMinuteSecond(StringBuilder sb, int totalSecond)
    {
        sb.Clear();

        int minute = (int)(totalSecond / 60);
        int second = (int)(totalSecond - minute * 60);

        if (minute != 0)
        {
            if (minute < 10)
                sb.Append('0');

            sb.Append(minute);
            sb.Append(':');
        }
        else
        {
            sb.Append('0');
            sb.Append('0');
            sb.Append(':');
        }

        if (second != 0)
        {
            if (second < 10)
                sb.Append('0');

            sb.Append(second);
        }
        else
        {
            sb.Append('0');
            sb.Append('0');
        }
    }

    public static void ToHourMinuteSecond(StringBuilder sb, int totalSecond)
    {
        sb.Clear();

        int hour = (int)(totalSecond / 3600);
        int minute = (int)((totalSecond - hour * 3600) / 60);
        int second = (int)(totalSecond - hour * 3600 - minute * 60);

        if (hour != 0)
        {
            if (hour < 10)
                sb.Append('0');

            sb.Append(hour);
            sb.Append(':');
        }
        else
        {
            sb.Append('0');
            sb.Append('0');
            sb.Append(':');
        }

        if (minute != 0)
        {
            if (minute < 10)
                sb.Append('0');

            sb.Append(minute);
            sb.Append(':');
        }
        else
        {
            sb.Append('0');
            sb.Append('0');
            sb.Append(':');
        }

        if (second != 0)
        {
            if (second < 10)
                sb.Append('0');

            sb.Append(second);
        }
        else
        {
            sb.Append('0');
            sb.Append('0');
        }
    }

    public static void ToDayHour(StringBuilder sb, float totalSecond)
    {
        sb.Clear();

        int day = (int)(totalSecond / (3600 * 24));
        int hour = (int)((totalSecond - day * (3600 * 24)) / 3600);

        if (day != 0)
        {
            sb.Append(day);
            sb.Append("d");
            sb.Append(':');
        }
        else
        {
            sb.Append('0');
            sb.Append('0');
            sb.Append(':');
            sb.Append("d");
        }

        if (hour != 0)
        {
            if (hour < 10)
                sb.Append('0');

            sb.Append(hour);
            sb.Append("h");
        }
        else
        {
            sb.Append('0');
            sb.Append('0');
            sb.Append("h");
        }
    }
}
