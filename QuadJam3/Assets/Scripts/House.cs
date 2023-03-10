using UnityEngine;

public class House : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            ScenesManager.LoadGameOver();
        }
    }
}
