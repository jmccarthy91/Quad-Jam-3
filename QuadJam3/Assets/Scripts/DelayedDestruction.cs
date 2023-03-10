using System.Collections;
using UnityEngine;

public class DelayedDestruction : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float _animationLength = 0.0f;

    private void OnEnable()
    {
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(_animationLength);

        Destroy(gameObject);
    }
}
