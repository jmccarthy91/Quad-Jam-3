using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalUpgradeChoice : MonoBehaviour
{
    void Start()
    {
        InGameUIManager.Instance.onModalUpgradeOpen += OnModalUpgradeOpen;
        InGameUIManager.Instance.onModalUpgradeClose += OnModalUpgradeClose;
    }

    void OnDestory()
    {
        InGameUIManager.Instance.onModalUpgradeOpen -= OnModalUpgradeOpen;
        InGameUIManager.Instance.onModalUpgradeClose -= OnModalUpgradeClose;
    }

    void OnModalUpgradeOpen()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void OnModalUpgradeClose()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
