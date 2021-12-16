using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CalendarEvent", menuName = "CalendarEvent/Create new CalendarEvent")]
public class CalendarEvent : ScriptableObject
{
    [SerializeField] private string event_name_;
    [SerializeField] private Vector2 event_time_ = new Vector2(23, 59);
    [SerializeField] private Vector3 event_date_ = new Vector3(1, 1, 2022);
    [SerializeField] private float global_light_intensity_;
    [SerializeField] private GameObject weather_vfx_;
    [SerializeField] private float weather_vfx_probability_;
    [SerializeField] private AudioClip event_sfx;
    [SerializeField] private AudioClip event_soundtrack;
}
