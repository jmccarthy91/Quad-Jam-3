using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMainMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
        ScenesManager.Instance.LoadMainMenu();
    }
}
