using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalCredits : MonoBehaviour
{
    void Start()
    {
        MainMenuUIManager.Instance.onCreditsOpen += OnCreditsOpen;
        MainMenuUIManager.Instance.onCreditsClose += OnCreditsClose;
    }

    void OnDestory()
    {
        MainMenuUIManager.Instance.onCreditsOpen -= OnCreditsOpen;
        MainMenuUIManager.Instance.onCreditsClose -= OnCreditsClose;
    }

    void OnCreditsOpen()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void OnCreditsClose()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
