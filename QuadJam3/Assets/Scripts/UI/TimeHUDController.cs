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
        day = (int)(time) % 30;
        month = ((int)(time) / 30) % 12;
        year = (int)(time) / 365;
        Debug.Log(day);
        

        timer.text = string.Format("{0:0000} {1:00} {2:00}", year, month, day);
    }
}
