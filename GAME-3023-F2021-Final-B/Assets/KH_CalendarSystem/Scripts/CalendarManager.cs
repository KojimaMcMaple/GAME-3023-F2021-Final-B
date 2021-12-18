using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Light2DE = UnityEngine.Experimental.Rendering.Universal.Light2D; //https://stackoverflow.com/a/66033687/12418806

public class CalendarManager : MonoBehaviour
{
    // STATS
    [SerializeField] private Vector2Int game_start_time_ = new Vector2Int(22,55);
    [SerializeField] private Vector3Int game_start_date_ = new Vector3Int(1, 1, 2022);
    [SerializeField] private int game_tick_speed_ = 60; //how many seconds pass in one frame
    [SerializeField] private float game_speed_ = 1.0f; //speed modifier to speed up calendar
    private Vector2Int game_curr_time_;
    private Vector3Int game_curr_date_;
    Calendar calendar_ = new Calendar();

    //EVENTS
    [SerializeField] private CalendarEventTable calendar_event_table_;
    private List<int> active_events_ = new List<int>();
    private List<int> upcoming_events_ = new List<int>();
    [SerializeField] private Light2DE global_light_; 
    private float default_global_light_intensity_ = -1;
    private Color default_global_light_color_ = Color.black;
    [SerializeField] private List<GameObject> weather_vfx_list_;
    private AudioSource audio_source_;

    // UI
    [SerializeField] private TMP_Text calendar_time_text_;
    [SerializeField] private TMP_Text calendar_date_text_;
    [SerializeField] private GameObject calendar_day_group_; //parent of calendar_day_list
    private List<CalendarDayController> calendar_day_list_ = new List<CalendarDayController>();
    private int curr_calendar_day_idx = -1;
    [SerializeField] private Color curr_day_color_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    [SerializeField] private Color default_day_color_ = new Color(0.8f, 0.8f, 0.8f, 1.0f);
    [SerializeField] private Color day_not_in_month_color_ = new Color(0.4f, 0.4f, 0.4f, 1.0f);

    void Start()
    {
        audio_source_ = GetComponent<AudioSource>();

        calendar_event_table_.Setup();

        default_global_light_intensity_ = global_light_.intensity;
        default_global_light_color_ = global_light_.color;

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

    public void PopulateCalendarDayList(Vector3Int date)
    {
        int first_idx = CalendarUtilities.GetFirstDayOfWeekFromDate(date);
        Debug.Log(">>> first_idx = " + first_idx);
        int days_in_month = System.DateTime.DaysInMonth((int)date.z, (int)date.y);
        int day = 1;

        Vector3Int last_date_prev_month = CalendarUtilities.GetPrevDate(new Vector3Int(1, date.y, date.z));
        int furthest_visible_day_prev_month = (int)last_date_prev_month.x - first_idx + 1;
        Debug.Log(">>> furthest_visible_day_prev_month = " + furthest_visible_day_prev_month);
        int day_prev_month = furthest_visible_day_prev_month;

        Vector3Int first_date_next_month = CalendarUtilities.GetNextDate(new Vector3Int(days_in_month, date.y, date.z));
        int day_next_month = 1;

        for (int i = 0; i < calendar_day_list_.Count; i++)
        {
            calendar_day_list_[i].SetDayColor(default_day_color_);
            if (i < first_idx) //set last month
            {
                //calendar_day_list_[i].SetDayText(furthest_visible_day_prev_month.ToString());
                calendar_day_list_[i].SetDate(new Vector3Int(day_prev_month, last_date_prev_month.y, last_date_prev_month.z));
                calendar_day_list_[i].SetDayColor(day_not_in_month_color_);
                day_prev_month++;
            }
            else if (i > days_in_month + first_idx -1) //set next month
            {
                //calendar_day_list_[i].SetDayText(first_day_next_month.ToString());
                calendar_day_list_[i].SetDate(new Vector3Int(day_next_month, first_date_next_month.y, first_date_next_month.z));
                calendar_day_list_[i].SetDayColor(day_not_in_month_color_);
                day_next_month++;
            }
            else 
            {
                //calendar_day_list_[i].SetDayText(day.ToString());
                calendar_day_list_[i].SetDate(new Vector3Int(day, date.y, date.z));
                if ((day == (int)game_curr_date_.x) && (date.y == game_curr_date_.y))
                {
                    calendar_day_list_[i].SetDayColor(curr_day_color_);
                    curr_calendar_day_idx = i;
                }
                day++;
            }
        }

        // Events
        foreach (var cal_day in calendar_day_list_)
        {
            cal_day.ClearEvents();
        }
        Vector3Int earliest_date_on_calendar = new Vector3Int(calendar_day_list_[0].GetDate().x, 
                                                        calendar_day_list_[0].GetDate().y, 
                                                        calendar_day_list_[0].GetDate().z);
        Vector3Int latest_date_on_calendar = new Vector3Int(calendar_day_list_[calendar_day_list_.Count-1].GetDate().x, 
                                                        calendar_day_list_[calendar_day_list_.Count - 1].GetDate().y, 
                                                        calendar_day_list_[calendar_day_list_.Count - 1].GetDate().z);
        //foreach (var cal_event in calendar_event_table_.GetTable()) //can be improved
        for (int i = 0; i < calendar_event_table_.GetEventCount(); i++)
        {
            foreach (var cal_day in calendar_day_list_) //iterate calendar_event_table_ 1st, and calendar_day_list_ 2nd to add one event to multiple days
            {
                int cal_date_int = CalendarUtilities.GetDateAsInt(cal_day.GetDate());
                if (cal_date_int >= calendar_event_table_.GetEvent(i).GetEventStartDateAsInt()  && cal_date_int <= calendar_event_table_.GetEvent(i).GetEventEndDateAsInt())
                {
                    //Debug.Log(">>> " + calendar_event_table_.GetEvent(i).GetEventName() + " = " + calendar_event_table_.GetEvent(i).GetEventStartDateAsInt());
                    //Debug.Log(">>> cal_day = " + cal_date_int);
                    cal_day.AddEvent(i);
                }
            }
        }
        foreach (var cal_day in calendar_day_list_)
        {
            cal_day.BuildEventText(calendar_event_table_);
        }
    }

    public void CheckCalendarEvents()
    {
        System.DateTime curr_datetime = new System.DateTime(game_curr_date_.z, game_curr_date_.y, game_curr_date_.x, 
                                                            game_curr_time_.x, game_curr_time_.y, 0);

        // END EVENTS
        if (active_events_.Count > 0)
        {
            for (int i = 0; i < active_events_.Count; i++)
            {
                if (curr_datetime >= calendar_event_table_.GetEvent(active_events_[i]).GetEventEndDateTime()) //if game_curr_date_ out of range
                {
                    EndCalendarEvent(calendar_event_table_.GetEvent(active_events_[i]));
                    active_events_.RemoveAt(i);
                }
            }
        }

        // START EVENTS
        if (calendar_day_list_[curr_calendar_day_idx].GetEventIdxList().Count > 0)
        {
            foreach (int event_idx in calendar_day_list_[curr_calendar_day_idx].GetEventIdxList())
            {
                if (active_events_.Contains(event_idx)) //skip
                {
                    continue;
                }
                if (curr_datetime >= calendar_event_table_.GetEvent(event_idx).GetEventStartDateTime() &&
                    curr_datetime < calendar_event_table_.GetEvent(event_idx).GetEventEndDateTime()) //if game_curr_date_ within range
                {
                    active_events_.Add(event_idx);
                    TriggerCalendarEvent(calendar_event_table_.GetEvent(event_idx));
                }
            }
        }
    }

    public void CheckCalendarWeatherEvents()
    {
        if (active_events_.Count == 0)
        {
            return;
        }
        for (int i = 0; i < active_events_.Count; i++)
        {
            TriggerCalendarEventWeatherVfx(calendar_event_table_.GetEvent(active_events_[i]));
        }
    }

    public void TriggerCalendarEvent(CalendarEvent cal_event)
    {
        Debug.Log("> Triggering " + cal_event.GetEventName());
        if (cal_event.IsTriggered())
        {
            return;
        }
        cal_event.SetTriggered(true);
        cal_event.SetActive(true);
        if (cal_event.GetGlobalLightIntensity() != -1)
        {
            global_light_.intensity = cal_event.GetGlobalLightIntensity();
        }
        if (cal_event.GetGlobalLightColor() != Color.black)
        {
            global_light_.color = cal_event.GetGlobalLightColor();
        }
        TriggerCalendarEventWeatherVfx(cal_event);
        if (cal_event.GetEventStartSfx() != null)
        {
            audio_source_.PlayOneShot(cal_event.GetEventStartSfx());
        }
    }

    public void TriggerCalendarEventWeatherVfx(CalendarEvent cal_event)
    {
        if (cal_event.GetWeatherVfx() != Weather.kNone)
        {
            int rand = Random.Range(1, 101);
            //Debug.Log(">>> rand = " + rand);
            if (rand < cal_event.GetWeatherVfxProbability())
            {
                weather_vfx_list_[(int)cal_event.GetWeatherVfx()].SetActive(true);
            }
            else
            {
                weather_vfx_list_[(int)cal_event.GetWeatherVfx()].SetActive(false);
            }
        }
    }

    public void EndCalendarEvent(CalendarEvent cal_event)
    {
        Debug.Log("> Ending " + cal_event.GetEventName());
        if (!cal_event.IsActive())
        {
            return;
        }
        cal_event.SetTriggered(false);
        cal_event.SetActive(false);
        if (cal_event.GetEventEndSfx() != null)
        {
            audio_source_.PlayOneShot(cal_event.GetEventEndSfx());
        }
        global_light_.intensity = default_global_light_intensity_;
        global_light_.color = default_global_light_color_;
        if (cal_event.GetWeatherVfx() != Weather.kNone)
        {
            weather_vfx_list_[(int)cal_event.GetWeatherVfx()].SetActive(false);
        }
    }

    public void UpdateCurrTime()
    {
        game_curr_time_ = new Vector2Int(calendar_.GetHour(), calendar_.GetMinute());
        SetTimeText(game_curr_time_);
        CheckCalendarEvents();
    }

    public void DoUpdateNewDay()
    {
        Debug.Log(">>> DoUpdateNewDay() ");
        game_curr_date_ = new Vector3Int(calendar_.GetDay(), calendar_.GetMonth(), calendar_.GetYear());
        Debug.Log("> game_curr_date_ = " + game_curr_date_.x + "/" + game_curr_date_.y + "/" + game_curr_date_.z);
        calendar_day_list_[curr_calendar_day_idx].SetDayColor(default_day_color_);
        curr_calendar_day_idx++;
        if (curr_calendar_day_idx >= calendar_day_list_.Count)
        {
            curr_calendar_day_idx = 0;
        }
        calendar_day_list_[curr_calendar_day_idx].SetDayColor(curr_day_color_);

        CheckCalendarWeatherEvents();
    }

    public void DoUpdateNewMonth()
    {
        Debug.Log(">>> DoUpdateNewMonth() ");
        game_curr_date_ = new Vector3Int(calendar_.GetDay(), calendar_.GetMonth(), calendar_.GetYear());
        Debug.Log(">> game_curr_date_ = " + game_curr_date_.x + "/" + game_curr_date_.y + "/" + game_curr_date_.z);
        SetDateText(game_curr_date_);
        PopulateCalendarDayList(game_curr_date_);
    }

    public void DoUpdateNewYear()
    {
        Debug.Log(">>> DoUpdateNewYear() ");
        game_curr_date_ = new Vector3Int(calendar_.GetDay(), calendar_.GetMonth(), calendar_.GetYear());
        Debug.Log(">>> game_curr_date_ = " + game_curr_date_.x + "/" + game_curr_date_.y + "/" + game_curr_date_.z);
        SetDateText(game_curr_date_);
        PopulateCalendarDayList(game_curr_date_);
    }

    //public void UpdateCurrDate()
    //{
    //    game_curr_date_ = new Vector3Int(calendar_.GetDay(), calendar_.GetMonth(), calendar_.GetYear());
    //    SetDateText(game_curr_date_);
    //    PopulateCalendarDayList(game_curr_date_);
    //}

    public void SetTimeText(Vector2Int time)
    {
        calendar_time_text_.text = time.x + ":" + time.y;
    }

    public void SetDateText(Vector3Int date)
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
