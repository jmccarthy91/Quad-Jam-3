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

    void OnDestroy()
    {
        if (EventsHUD == this)
        {
            EventsHUD = null;
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

    public event Action<int,int> onExpereinceChange;
    public void OnExpereinceChange(int newExp, int maxExpLvl)
    {
      if (onExpereinceChange != null)
      {
        onExpereinceChange(newExp, maxExpLvl);
      }
    }

    public event Action<float> onJetpackStarted;
    public void OnJetpackStarted(float maxFlightTime)
    {
      if (onJetpackStarted != null)
      {
        onJetpackStarted(maxFlightTime);
      }
    }

    public event Action<float> onJetpackEnded;
    public void OnJetpackEnded(float refillTime)
    {
      if (onJetpackEnded != null)
      {
        onJetpackEnded(refillTime);
      }
    }

    public event Action<float> onTimeUpdate;
    public void OnTimeUpdate(float currentTime)
    {
      if (onTimeUpdate != null)
      {
        onTimeUpdate(currentTime);
      }
    }


}
