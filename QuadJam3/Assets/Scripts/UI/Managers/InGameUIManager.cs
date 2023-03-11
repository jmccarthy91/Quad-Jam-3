using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance;

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

    public event Action onModalUpgradeOpen;
    public void OnModalUpgradeOpen()
    {
      if (onModalUpgradeOpen != null)
      {
        onModalUpgradeOpen();
      }
    }

    public event Action onModalUpgradeClose;
    public void OnModalUpgradeClose()
    {
      if (onModalUpgradeClose != null)
      {
        onModalUpgradeClose();
      }
    }

    public event Action onInGameSettingsOpen;
    public void OnInGameSettingsOpen()
    {
      if (onInGameSettingsOpen != null)
      {
        onInGameSettingsOpen();
      }
    }

    public event Action onInGameSettingsClose;
    public void OnInGameSettingsClose()
    {
      if (onInGameSettingsClose != null)
      {
        onInGameSettingsClose();
      }
    }
}
