using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EndGameUIManager : MonoBehaviour
{
    public static EndGameUIManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public event Action onMainOpen;
    public void OnMainOpen()
    {
      if (onMainOpen != null)
      {
        onMainOpen();
      }
    }

    public event Action onMainClose;
    public void OnMainClose()
    {
      if (onMainClose != null)
      {
        onMainClose();
      }
    }

    public event Action onCreditsOpen;
    public void OnCreditsOpen()
    {
      if (onCreditsOpen != null)
      {
        onCreditsOpen();
      }
    }

    public event Action onCreditsClose;
    public void OnCreditsClose()
    {
      if (onCreditsClose != null)
      {
        onCreditsClose();
      }
    }
}
