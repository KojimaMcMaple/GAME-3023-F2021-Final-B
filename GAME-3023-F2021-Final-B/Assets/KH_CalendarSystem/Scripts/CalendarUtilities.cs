using UnityEngine;

public static class CalendarUtilities
{
    public static int GetDayOfWeek(int day, int month, int year)
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

    public static int GetFirstDayOfWeekFromDate(Vector3Int date)
    {
        return GetDayOfWeek(1, (int)date.y, (int)date.z);
    }

    public static Vector3Int GetPrevDate(Vector3Int date)
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
        return new Vector3Int(day, month, year);
    }

    public static Vector3Int GetNextDate(Vector3Int date)
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
        return new Vector3Int(day, month, year);
    }

    public static int GetLastDayPrevMonth(int month, int year)
    {
        return (int)GetPrevDate(new Vector3Int(1, month, year)).x;
    }

    public static int GetDateAsInt(Vector3Int date)
    {
        return ((int)date.z * 10000 +
                (int)date.y * 100 +
                (int)date.x);
    }

    public static int GetTimeAsInt(Vector2Int time)
    {
        return ((int)time.x * 100 +
                (int)time.y);
    }
}
