using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CalendarManager : MonoBehaviour
{
    // STATS
    [SerializeField] private Vector2 game_start_time_ = new Vector2(22,55);
    [SerializeField] private Vector3 game_start_date_ = new Vector3(1, 1, 2022);
    [SerializeField] private int game_tick_speed_ = 60; //how many seconds pass in one frame
    [SerializeField] private float game_speed_ = 1.0f; //speed modifier to speed up calendar
    private Vector2 game_curr_time_;
    private Vector3 game_curr_date_;
    Calendar calendar_ = new Calendar();

    // UI
    [SerializeField] private TMP_Text calendar_time_text_;
    [SerializeField] private TMP_Text calendar_date_text_;
    [SerializeField] private GameObject calendar_day_group_; //parent of calendar_day_list
    private List<CalendarDayController> calendar_day_list_ = new List<CalendarDayController>();
    private int curr_calendar_day_idx = -1;
    [SerializeField] private Color curr_day_color_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    [SerializeField] private Color default_day_color_ = new Color(0.8f, 0.8f, 0.8f, 1.0f);
    [SerializeField] private Color day_not_in_month_color_ = new Color(0.4f, 0.4f, 0.4f, 1.0f);

    void Awake()
    {
        game_curr_time_ = game_start_time_;
        game_curr_date_ = game_start_date_;
        calendar_.Setup(game_curr_date_, game_curr_time_);
        Calendar.OnNewMinute += UpdateCurrTime;
        Calendar.OnNewHour += UpdateCurrTime;
        Calendar.OnNewDay += DoUpdateNewDay;
        Calendar.OnNewMonth += DoUpdateNewMonth;
        Calendar.OnNewYear += DoUpdateNewYear;

        foreach (Transform child in calendar_day_group_.transform)
        {
            CalendarDayController child_controller = child.GetComponent<CalendarDayController>();
            if (child_controller!=null)
            {
                calendar_day_list_.Add(child_controller);
            }
        }
        SetTimeText(game_curr_time_);
        SetDateText(game_curr_date_);
        PopulateCalendarDayList(game_curr_date_);
    }

    void Update()
    {
        calendar_.DoUpdateGameTime(game_tick_speed_, game_speed_);
    }

    private void OnDisable()
    {
        Calendar.OnNewMinute -= UpdateCurrTime;
        Calendar.OnNewHour -= UpdateCurrTime;
        Calendar.OnNewDay -= DoUpdateNewDay;
        Calendar.OnNewMonth -= DoUpdateNewMonth;
        Calendar.OnNewYear -= DoUpdateNewYear;
    }

    public void PopulateCalendarDayList(Vector3 date)
    {
        int first_idx = calendar_.GetFirstDayOfWeekFromDate(date);
        int days_in_month = System.DateTime.DaysInMonth((int)date.z, (int)date.y);
        int day = 1;

        int last_day_prev_month = (int)calendar_.GetLastDayPrevMonth((int)date.y, (int)date.z);
        int furthest_visible_day_prev_month = last_day_prev_month - first_idx + 1;

        int first_day_next_month = 1;

        for (int i = 0; i < calendar_day_list_.Count; i++)
        {
            calendar_day_list_[i].SetDayColor(default_day_color_);
            if (i < first_idx)
            {
                calendar_day_list_[i].SetDayText(furthest_visible_day_prev_month.ToString());
                calendar_day_list_[i].SetDayColor(day_not_in_month_color_);
                furthest_visible_day_prev_month++;
            }
            else if (i > days_in_month + first_idx -1)
            {
                calendar_day_list_[i].SetDayText(first_day_next_month.ToString());
                calendar_day_list_[i].SetDayColor(day_not_in_month_color_);
                first_day_next_month++;
            }
            else 
            {
                calendar_day_list_[i].SetDayText(day.ToString());
                if ((day == (int)game_curr_date_.x) && (date.y == game_curr_date_.y))
                {
                    calendar_day_list_[i].SetDayColor(curr_day_color_);
                    curr_calendar_day_idx = i;
                }
                day++;
            }
        }
    }

    public void UpdateCurrTime()
    {
        game_curr_time_ = new Vector2(calendar_.GetHour(), calendar_.GetMinute());
        SetTimeText(game_curr_time_);
    }

    public void DoUpdateNewDay()
    {
        Debug.Log(">>> DoUpdateNewDay() ");
        game_curr_date_ = new Vector3(calendar_.GetDay(), calendar_.GetMonth(), calendar_.GetYear());
        Debug.Log("> game_curr_date_ = " + game_curr_date_.x + "/" + game_curr_date_.y + "/" + game_curr_date_.z);
        calendar_day_list_[curr_calendar_day_idx].SetDayColor(default_day_color_);
        curr_calendar_day_idx++;
        if (curr_calendar_day_idx >= calendar_day_list_.Count)
        {
            curr_calendar_day_idx = 0;
        }
        calendar_day_list_[curr_calendar_day_idx].SetDayColor(curr_day_color_);
    }

    public void DoUpdateNewMonth()
    {
        Debug.Log(">>> DoUpdateNewMonth() ");
        game_curr_date_ = new Vector3(calendar_.GetDay(), calendar_.GetMonth(), calendar_.GetYear());
        Debug.Log(">> game_curr_date_ = " + game_curr_date_.x + "/" + game_curr_date_.y + "/" + game_curr_date_.z);
        SetDateText(game_curr_date_);
        PopulateCalendarDayList(game_curr_date_);
    }

    public void DoUpdateNewYear()
    {
        Debug.Log(">>> DoUpdateNewYear() ");
        game_curr_date_ = new Vector3(calendar_.GetDay(), calendar_.GetMonth(), calendar_.GetYear());
        Debug.Log(">>> game_curr_date_ = " + game_curr_date_.x + "/" + game_curr_date_.y + "/" + game_curr_date_.z);
        SetDateText(game_curr_date_);
        PopulateCalendarDayList(game_curr_date_);
    }

    public void UpdateCurrDate()
    {
        game_curr_date_ = new Vector3(calendar_.GetDay(), calendar_.GetMonth(), calendar_.GetYear());
        SetDateText(game_curr_date_);
        PopulateCalendarDayList(game_curr_date_);
    }

    public void SetTimeText(Vector2 time)
    {
        calendar_time_text_.text = time.x + ":" + time.y;
    }

    public void SetDateText(Vector3 date)
    {
        string month_text = "NULL";
        switch (date.y)
        {
            case 1:
                month_text = "January";
                break;
            case 2:
                month_text = "February";
                break;
            case 3:
                month_text = "March";
                break;
            case 4:
                month_text = "April";
                break;
            case 5:
                month_text = "May";
                break;
            case 6:
                month_text = "June";
                break;
            case 7:
                month_text = "July";
                break;
            case 8:
                month_text = "August";
                break;
            case 9:
                month_text = "September";
                break;
            case 10:
                month_text = "October";
                break;
            case 11:
                month_text = "November";
                break;
            case 12:
                month_text = "December";
                break;
        }
        calendar_date_text_.text = month_text + ", " + date.z;
    }
}
