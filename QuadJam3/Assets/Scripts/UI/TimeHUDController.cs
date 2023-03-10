using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeHUDController : MonoBehaviour
{
    public Text timer;

    private int year = 0; // start year is now?
    private int month = 1;
    private int day = 1;

    private float timeScale = 0.00001f;

    void Start()
    {
        HUDEventsManager.EventsHUD.onTimeUpdate += OnTimeUpdate;
    }

    void OnDestroy()
    {
        HUDEventsManager.EventsHUD.onTimeUpdate -= OnTimeUpdate;
    }

    // void FixedUpdate()
    // {
    //     UpdateHomeDate();
    // }

    void OnTimeUpdate(float newTime)
    { 
        UpdateHomeDate(newTime);
    }

    void UpdateHomeDate( float time )
    {
        //time = Time.fixedDeltaTime * timeScale;
        time = time * timeScale;
        day = day + (int)(time);
        if (day > 30) // skip 30/31 variation
        {
            month++;
            day = 1;
        }
        if (month > 12)
        {
            year++;
            month = 1;
        }

        timer.text = string.Format("{0:0000} {1:00} {2:00}", year, month, day);
    }
}
