using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CalendarEventTable", menuName = "CalendarEvent/Create new CalendarEventTable")]
public class CalendarEventTable : ScriptableObject
{
    [SerializeField] private List<CalendarEvent> table_;  //The index of each item in the table is its ID

    public void Setup()
    {
        foreach (var item in table_)
        {
            item.SetupDatesAsInt();
        }
    }

    public CalendarEvent GetEvent(int id)
    {
        return table_[id];
    }

    public void AssignItemIDs() // Give each item an ID based on its location in the list
    {
        for (int i = 0; i < table_.Count; i++)
        {
            table_[i].SetId(i);
        }
    }

    public List<CalendarEvent> GetTable()
    {
        return table_;
    }

    public int GetEventCount()
    {
        return table_.Count;
    }
}
