using UnityEngine;

public class PlayerDeathTrigger : MonoBehaviour
{
    public delegate void DeathTrigger();
    public static event DeathTrigger OnDeathTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("[Death Trigger]: Entered.");
            OnDeathTrigger?.Invoke();
        }
    }
}
