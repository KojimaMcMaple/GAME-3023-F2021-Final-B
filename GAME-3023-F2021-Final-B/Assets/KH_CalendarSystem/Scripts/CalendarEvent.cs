using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CalendarEvent", menuName = "CalendarEvent/Create new CalendarEvent")]
public class CalendarEvent : ScriptableObject
{
    [SerializeField] private string event_name_;
    [SerializeField] private string event_description_;
    [SerializeField] private Vector3Int event_start_date_ = new Vector3Int(1, 1, 2022);
    [SerializeField] private Vector2Int event_start_time_ = new Vector2Int(0, 0);
    [SerializeField] private Vector3Int event_end_date_ = new Vector3Int(1, 1, 2022);
    [SerializeField] private Vector2Int event_end_time_ = new Vector2Int(23, 59);
    [SerializeField] private float global_light_intensity_ = -1;
    [SerializeField] private Color global_light_color_ = Color.black;
    [SerializeField] private Weather weather_vfx_ = Weather.kNone;
    [SerializeField] private float weather_vfx_probability_ = 100;
    [SerializeField] private AudioClip event_start_sfx_;
    [SerializeField] private AudioClip event_end_sfx_;
    [SerializeField] private AudioClip event_soundtrack_;
    private System.DateTime event_start_datetime_;
    private int event_start_date_int_;
    private int event_start_time_int_;
    private System.DateTime event_end_datetime_;
    private int event_end_date_int_;
    private int event_end_time_int_;
    private int id_;
    private bool is_active_;
    private bool is_triggered_;

    public void SetupDatesAsInt()
    {
        event_start_date_int_ = CalendarUtilities.GetDateAsInt(event_start_date_);
        event_start_time_int_ = CalendarUtilities.GetTimeAsInt(event_start_time_);
        event_start_datetime_ = new System.DateTime(event_start_date_.z, event_start_date_.y, event_start_date_.x, 
            event_start_time_.x, event_start_time_.y, 0); ;
        event_end_date_int_ = CalendarUtilities.GetDateAsInt(event_end_date_);
        event_end_time_int_ = CalendarUtilities.GetTimeAsInt(event_end_time_);
        event_end_datetime_ = new System.DateTime(event_end_date_.z, event_end_date_.y, event_end_date_.x,
            event_end_time_.x, event_end_time_.y, 0); ;
    }

    public string GetEventName()
    {
        return event_name_;
    }

    public string GetEventDescription()
    {
        return event_description_;
    }

    public Vector3Int GetEventStartDate()
    {
        return event_start_date_;
    }

    public Vector2Int GetEventStartTime() 
    {
        return event_start_time_;
    }

    public System.DateTime GetEventStartDateTime()
    {
        return event_start_datetime_;
    }
    
    public int GetEventStartDateAsInt()
    {
        return event_start_date_int_;
    }
    
    public int GetEventStartTimeAsInt()
    {
        return event_start_time_int_;
    }

    public Vector3Int GetEventEndDate()
    {
        return event_end_date_;
    }

    public Vector2Int GetEventEndTime()
    {
        return event_end_time_;
    }

    public System.DateTime GetEventEndDateTime()
    {
        return event_end_datetime_;
    }

    public int GetEventEndDateAsInt()
    {
        return event_end_date_int_;
    }
    
    public int GetEventEndTimeAsInt()
    {
        return event_end_time_int_;
    }

    public float GetGlobalLightIntensity()
    {
        return global_light_intensity_;
    }
    
    public Color GetGlobalLightColor()
    {
        return global_light_color_;
    }

    public Weather GetWeatherVfx()
    {
        return weather_vfx_;
    }
        
    public float GetWeatherVfxProbability()
    {
        return weather_vfx_probability_;
    }
        
    public AudioClip GetEventStartSfx()
    {
        return event_start_sfx_;
    }

    public AudioClip GetEventEndSfx()
    {
        return event_end_sfx_;
    }

    public AudioClip GetEventSoundtrack()
    {
        return event_soundtrack_;
    }

    public int GetId()
    {
        return id_;
    }

    public void SetId(int value)
    {
        id_ = value;
    }

    public bool IsActive()
    {
        return is_active_;
    }

    public void SetActive(bool value)
    {
        is_active_ = value;
    }

    public bool IsTriggered()
    {
        return is_triggered_;
    }

    public void SetTriggered(bool value)
    {
        is_triggered_ = value;
    }
}

public enum Weather
{
    kNone = -1,
    kRain = 0,
    kSnow
}