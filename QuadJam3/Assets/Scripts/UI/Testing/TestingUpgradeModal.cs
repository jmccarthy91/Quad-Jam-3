using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingUpgradeModal : MonoBehaviour
{
    public void OpenUpgradeModal()
    {
        InGameUIManager.Instance.OnModalUpgradeOpen();
    }
}
