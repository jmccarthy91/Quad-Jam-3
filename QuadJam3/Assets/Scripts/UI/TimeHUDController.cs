using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeHUDController : MonoBehaviour
{
    public Text timer;

    private int year = 2023; // start year is now?
    private int month = 1;
    private int day = 1;

    public float timeScale = 100f;

    void FixedUpdate()
    {
        UpdateHomeDate();
        
    }

    void UpdateHomeDate()
    {
        float time = Time.fixedDeltaTime * timeScale;
        day = day + (int)time;
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
