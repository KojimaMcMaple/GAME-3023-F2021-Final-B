using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarManager : MonoBehaviour
{
    //https://en.wikipedia.org/wiki/Zeller%27s_congruence
    private int day_of_week_; //cal
    private int day_; //input
    private int month_; //input
    private int year_; //input
    private int year_of_century_; //cal
    private int century_; //cal


    void Awake()
    {
        
    }

    void Update()
    {
        
    }

    public void GetDayOfWeek(int day, int month, int year)
    {
        int m = month;
        if (month == 1)
        {
            m = 13;
        }
        else if (month == 2)
        {
            m = 14;
        }
        int K = year % 100;
        int J = (int)(year / 100);
        int h = (day +
            (int)((13 * (m + 1)) / 5) +
            K +
            (int)(K / 4) +
            (int)(J / 4) -
            2 * J) % 7;
    }
}
