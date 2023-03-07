using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParent : MonoBehaviour
{
    //this script moves the weapon around the player
    private void Update()
    {
        
        Vector3 mousePosition = MouseSingleton.GetMouseWorldPosition();
        transform.right = (mousePosition - (Vector3)transform.position).normalized;
    }
}
