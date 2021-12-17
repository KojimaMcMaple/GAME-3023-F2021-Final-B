using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar
{
    // STATS
    private int sec_;
    private int min_;
    private int hour_;
    private int day_;
    private int month_;
    private int year_;

    // EVENTS - https://learn.unity.com/tutorial/events-uh#5c894782edbc2a1410355442
    public delegate void CalendarEventAction();
    public static event CalendarEventAction OnNewMinute;
    public static event CalendarEventAction OnNewHour;
    public static event CalendarEventAction OnNewDay;
    public static event CalendarEventAction OnNewMonth;
    public static event CalendarEventAction OnNewYear;

    public int GetSec()
    {
        return sec_;
    }

    public int GetMinute()
    {
        return min_;
    }

    public int GetHour()
    {
        return hour_;
    }

    public int GetDay()
    {
        return day_;
    }

    public int GetMonth()
    {
        return month_;
    }

    public int GetYear()
    {
        return year_;
    }

    public void Setup(Vector3Int game_curr_date, Vector2Int game_curr_time)
    {
        sec_ = 0;
        min_ = (int)game_curr_time.y;
        hour_ = (int)game_curr_time.x;
        day_ = (int)game_curr_date.x;
        month_ = (int)game_curr_date.y;
        year_ = (int)game_curr_date.z;
    }

    public void DoUpdateGameTime(int tick_speed = 60, float speed = 1.0f)
    {
        sec_ += (int)(tick_speed * speed);
        int min_add = (int)(sec_ / 60);
        if (min_add == 0) { return; } //Return Early Pattern to skip redundant calculations
        sec_ %= 60; //Reset var

        min_ += min_add;
        int hour_add = (int)(min_ / 60);
        min_ %= 60; //Reset before broadcast, to have accurate display
        if (OnNewMinute != null) { OnNewMinute(); } //Broadcast event
        if (hour_add == 0) { return; } //Return Early Pattern to skip redundant calculations
        

        hour_ += hour_add;
        int day_add = (int)(hour_ / 24);
        hour_ %= 24; //Reset before broadcast, to have accurate display
        if (OnNewHour != null) { OnNewHour(); } //Broadcast event
        if (day_add == 0) { return; } //Return Early Pattern to skip redundant calculations
        

        int days_in_month = System.DateTime.DaysInMonth(year_, month_);
        day_ += day_add;
        int mon_add = 0;
        if (day_!= days_in_month) //Prevent day_ = 0
        {
            mon_add = (int)(day_ / days_in_month);
            day_ %= days_in_month; //Reset before broadcast, to have accurate display
        }
        if (OnNewDay != null) { OnNewDay(); } //Broadcast event
        if (mon_add == 0) { return; } //Return Early Pattern to skip redundant calculations
        

        month_ += mon_add;
        int year_add = 0;
        if (month_!=12) //Prevent month_ = 0
        {
            year_add = (int)(month_ / 12);
            month_ %= 12; //Reset before broadcast, to have accurate display
        }
        if (OnNewMonth != null) { OnNewMonth(); } //Broadcast event
        if (year_add == 0) { return; } //Return Early Pattern to skip redundant calculations
        

        year_ += year_add;
        if (OnNewYear != null) { OnNewYear(); } //Broadcast event
    }
}

public enum DayOfWeek
{
    kSaturday = 0,
    kSunday = 1,
    kMonday = 2,
    kTuesday,
    kWednesday,
    kThursday,
    kFriday
}