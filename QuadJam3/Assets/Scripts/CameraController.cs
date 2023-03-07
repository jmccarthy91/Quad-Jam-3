using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Transform _player;

    private void Awake()
    {
        if (!_player)
        {
            Debug.LogError("[CameraController]: Failed to fetch Player Transform");
        }
    }

    private void LateUpdate() => OffsetPosition();

    private void OffsetPosition() => transform.position = _player.position + _offset;
}
