using UnityEngine;

public class MiningNode : MonoBehaviour
{
    [Header("MiningNode Data")]
    [SerializeField] private int _health = 0;

    private void Start()
    {
        EventManager.Current.OnMineralHit += Hit;
        EventManager.Current.OnMineralMined += Mined;
    }

    private void Hit()
    {
        _health--;
        Debug.Log("[MiningNode]: Mineral hit\tRemaining health: " + _health);
    }

    private void Mined()
    {
        // ...
    }
}
