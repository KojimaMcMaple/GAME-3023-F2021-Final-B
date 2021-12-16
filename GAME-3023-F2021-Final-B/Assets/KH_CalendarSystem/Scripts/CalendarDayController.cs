using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalendarDayController : MonoBehaviour
{
    [SerializeField] private TMP_Text day_text_;
    [SerializeField] private TMP_Text event_text_;
    private Image image_;
    private Vector3 date_;

    private void Awake()
    {
        image_ = GetComponent<Image>();
    }

    public void SetEventText(string text)
    {
        event_text_.text = text;
    }

    public void SetDayText(string text)
    {
        day_text_.text = text;
    }

    public void SetDayColor(Color color)
    {
        image_.color = color;
    }

    public void SetDate(Vector3 date)
    {
        date_ = date;
        SetDayText(date.x.ToString());
    }
}
