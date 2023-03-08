using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDEventsManager : MonoBehaviour
{
    public static HUDEventsManager EventsHUD;

    void Awake()
    {
        if (EventsHUD != null && EventsHUD != this)
        {
            Destroy(this);
        }
        else
        {
            EventsHUD = this;
        }
    }

    public event Action<int> onHealthChange;
    public void OnHealthChange(int newHealth)
    {
      if (onHealthChange != null)
      {
        onHealthChange(newHealth);
      }
    }


}
