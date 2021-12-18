using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CalendarDayController : MonoBehaviour
{
    [SerializeField] private TMP_Text day_text_;
    [SerializeField] private TMP_Text event_text_;
    private Image image_ = null;
    private Vector3Int date_;
    private List<int> event_idx_list_ = new List<int>(); //idx of CalendarEventTable

    private void Awake()
    {
        image_ = GetComponent<Image>();
        if (image_ == null)
        {
            Debug.Log(">>> Failed to find Image component!");
        }
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

    public Vector3Int GetDate()
    {
        return date_;
    }

    public void SetDate(Vector3Int date)
    {
        date_ = date;
        SetDayText(date.x.ToString());
    }

    public void BuildEventText(CalendarEventTable table)
    {
        event_text_.text = "";
        if (event_idx_list_.Count == 0){return;}
        SortEvents(table);
        for (int i = 0; i < event_idx_list_.Count; i++)
        {
            if (CalendarUtilities.GetDateAsInt(date_) == table.GetEvent(event_idx_list_[i]).GetEventStartDateAsInt()) //should add start time, but no space
            {

            }
            string suffix = ",\n";
            if (i == event_idx_list_.Count-1)
            {
                suffix = "";
            }
            event_text_.text += table.GetEvent(event_idx_list_[i]).GetEventName() + suffix;
        }
    }

    public List<int> GetEventIdxList()
    {
        return event_idx_list_;
    }

    public void AddEvent(int value)
    {
        event_idx_list_.Add(value);
    }

    public void RemoveEvent(int value)
    {
        event_idx_list_.Remove(value);
    }

    public void ClearEvents()
    {
        event_idx_list_.Clear();
    }

    public void SortEvents(CalendarEventTable table)
    {
        if (event_idx_list_.Count > 0)
        {
            event_idx_list_ = event_idx_list_.OrderBy(x => table.GetEvent(x).GetEventStartDateAsInt()).ToList();
        }
    }
}
