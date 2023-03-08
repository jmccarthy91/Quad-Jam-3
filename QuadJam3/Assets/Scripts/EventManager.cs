using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //used this tutorial: https://www.youtube.com/watch?v=gx0Lt4tCDE0&t=346s

    public static EventManager Current;

    void Awake()
    {
        if (Current != null && Current != this)
        {
            Destroy(this);
        }
        else
        {
            Current = this;
        }
    }

    //when an enemy hits the player
    public event Action OnHitPlayer;
    public void HitPlayer() => OnHitPlayer?.Invoke();

    //when the player hits a mineral deposit
    public event Action OnMineralHit;
    public void MineralHit() => OnMineralHit?.Invoke();

    //when a mineral deposit is mined out
    public event Action OnMineralMined;
    public void MineralMined() => OnMineralMined?.Invoke();

    //when boots get upgraded
    public event Action OnBootsLevel;
    public void BootsLevel() => OnBootsLevel?.Invoke();

    //when mining pick gets upgraded
    public event Action OnPickLevel;
    public void PickLevel() => OnPickLevel?.Invoke();
}
