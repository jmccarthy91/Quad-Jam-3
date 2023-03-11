using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalMainMenu : MonoBehaviour
{

    void Start()
    {
        MainMenuUIManager.Instance.onMainOpen += OnMainOpen;
        MainMenuUIManager.Instance.onMainClose += OnMainClose;
    }

    void OnDestory()
    {
        MainMenuUIManager.Instance.onMainOpen -= OnMainOpen;
        MainMenuUIManager.Instance.onMainClose -= OnMainClose;
    }

    void OnMainOpen()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void OnMainClose()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
