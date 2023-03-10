using System;
using UnityEngine;

public class DebugUpgradeUI : MonoBehaviour
{
    public event Action OnUpgradeBoots;
    public void UpgradeBoots() => OnUpgradeBoots?.Invoke();

    public event Action OnUpgradePickaxe;
    public void UpgradePickaxe() => OnUpgradePickaxe?.Invoke();
}
