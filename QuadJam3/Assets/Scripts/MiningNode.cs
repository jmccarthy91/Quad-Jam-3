using UnityEngine;

public class MiningNode : MonoBehaviour
{
    [Header("MiningNode Data")]
    [SerializeField] private int _health = 0;

    public void Hit()
    {
        _health--;

        if (_health < 1)
        {
            Mined();
        }
    }

    private void Mined()
    {
        HUDEventsManager.EventsHUD.OnExpereinceChange(25, 100);
        EventManager.Current.MineralMined();
        FindObjectOfType<AudioManager>().Play("MineralFinish");
        Debug.Log("[MineralNode]: Mineral Mined.");
        Destroy(gameObject);
    }
}
