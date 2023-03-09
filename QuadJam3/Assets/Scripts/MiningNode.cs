using UnityEngine;

public class MiningNode : MonoBehaviour
{
    [Header("MiningNode Data")]
    [SerializeField] private int _health = 0;

    public void Hit()
    {
        _health--;
        // Debug.Log("[MiningNode]: Mineral hit\tRemaining health: " + _health);

        if (_health < 1)
        {
            Mined();
        }
    }

    private void Mined()
    {
        Destroy(gameObject);
        FindObjectOfType<AudioManager>().Play("MineralFinish");
        Debug.Log("[MineralNode]: Mineral Mined.");
    }
}
