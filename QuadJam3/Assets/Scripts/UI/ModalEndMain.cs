using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalEndMain : MonoBehaviour
{
    public class ModalMainMenu : MonoBehaviour
{

    void Start()
    {
        EndGameUIManager.Instance.onMainOpen += OnMainOpen;
        EndGameUIManager.Instance.onMainClose += OnMainClose;
    }

    void OnDestory()
    {
        EndGameUIManager.Instance.onMainOpen -= OnMainOpen;
        EndGameUIManager.Instance.onMainClose -= OnMainClose;
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
}
