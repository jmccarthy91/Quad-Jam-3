using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUpgradeChoice : MonoBehaviour
{
    public bool upgradeAttack;
    public bool upgradeJump;

    public void makeChoice()
    {
        if (upgradeAttack)
        {
            Debug.Log("Upgrade Attack");
        }
        else if (upgradeJump)
        {
            Debug.Log("Upgrade Jump");
        }

        InGameUIManager.Instance.OnModalUpgradeClose();
    }

    
}
