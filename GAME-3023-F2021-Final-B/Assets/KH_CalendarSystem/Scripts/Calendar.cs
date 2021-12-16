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

    public void Setup(Vector3 game_curr_date, Vector2 game_curr_time)
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
        int year_add = (int)(month_ / 12);
        if (month_!=12) //Prevent month_ = 0
        {
            month_ %= 12; //Reset before broadcast, to have accurate display
        }
        if (OnNewMonth != null) { OnNewMonth(); } //Broadcast event
        if (year_add == 0) { return; } //Return Early Pattern to skip redundant calculations
        

        year_ += year_add;
        if (OnNewYear != null) { OnNewYear(); } //Broadcast event
    }

    public int GetDayOfWeek(int day, int month, int year)
    {
        //For the Gregorian calendar, Zeller's congruence - https://en.wikipedia.org/wiki/Zeller%27s_congruence
        /*  h is the day of the week (0 = Saturday, 1 = Sunday, 2 = Monday, ..., 6 = Friday)
            q is the day of the month
            m is the month (3 = March, 4 = April, 5 = May, ..., 14 = February)
            K the year of the century ( year mod 100 ).
            J is the zero-based century (actually [ year / 100 ] ) For example, the zero-based centuries for 1995 and 2000 are 19 and 20 respectively (not to be confused with the common ordinal century enumeration which indicates 20th for both cases).
            [ . . . ] is the floor function or integer part
            mod is the modulo operation or remainder after division

        In this algorithm January and February are counted as months 13 and 14 of the previous year. 
        E.g. if it is 2 February 2010, the algorithm counts the date as the second day of the fourteenth month of 2009 (02/14/2009 in DD/MM/YYYY format)
        
        The formulas rely on the mathematician's definition of modulo division, which means that -2 mod 7 is equal to positive 5. 
        Unfortunately, in the truncating way most computer languages implement the remainder function, -2 mod 7 returns a result of -2. 
        So, to implement Zeller's congruence on a computer, the formulas should be altered slightly to ensure a positive numerator. 
        The simplest way to do this is to replace - 2J by + 5J
        */

        int m = month; //(3 = March, 4 = April, 5 = May, ..., 14 = February)
        int y = year;
        if (month == 1) //In this algorithm January and February are counted as months 13 and 14 of the previous year
        {
            m = 13;
            y = year - 1;
        }
        else if (month == 2)
        {
            m = 14;
            y = year - 1;
        }
        int K = y % 100;
        int J = (int)(y / 100);
        int h = (day +
            (int)((13 * (m + 1)) / 5) +
            K +
            (int)(K / 4) +
            (int)(J / 4) +
            5 * J) % 7; //(0 = Saturday, 1 = Sunday, 2 = Monday, ..., 6 = Friday)

        return h;
    }

    public int GetFirstDayOfWeekFromDate(Vector3 date)
    {
        return GetDayOfWeek(1, (int)date.y, (int)date.z);
    }

    public Vector3 GetPrevDate(Vector3 date)
    {
        int day = (int)(date.x - 1);
        int month = (int)(date.y);
        int year = (int)(date.z);
        if (day < 1)
        {
            month--;
            if (month < 1)
            {
                year--;
                month = 12;
            }
            day = System.DateTime.DaysInMonth(year, month);
        }
        return new Vector3(day, month, year);
    }

    public Vector3 GetNextDate(Vector3 date)
    {
        int day = (int)(date.x + 1);
        int month = (int)(date.y);
        int year = (int)(date.z);
        if (day > System.DateTime.DaysInMonth(year, month))
        {
            month++;
            if (month > 12)
            {
                year++;
                month = 1;
            }
            day = 1;
        }
        return new Vector3(day, month, year);
    }

    public int GetLastDayPrevMonth(int month, int year)
    {
        return (int)GetPrevDate(new Vector3(1, month, year)).x;
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