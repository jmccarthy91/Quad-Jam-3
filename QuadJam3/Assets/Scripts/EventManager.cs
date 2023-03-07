using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //used this tutorial: https://www.youtube.com/watch?v=gx0Lt4tCDE0&t=346s

    public static EventManager current;

    void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(this);
        }
        else
        {
            current = this;
        }
    }

    //when an enemy hits the player
    public event Action onHitPlayer;
    public void HitPlayer()
    {
        onHitPlayer?.Invoke();
    }

    //when the player hits a mineral deposit
    public event Action onMineralHit;
    public void MineralHit()
    {
        onMineralHit?.Invoke();
    }

    //when a mineral deposit is mined out
    public event Action onMineralMined;
    public void MineralMined()
    {
        onMineralMined?.Invoke();
    }

    //when boots get upgraded
    public event Action onBootsLevel;
    public void BootsLevel()
    {
        onBootsLevel?.Invoke();
    }

    //when mining pick gets upgraded
    public event Action onPickLevel;
    public void PickLevel()
    {
        onPickLevel?.Invoke();
    }
}
