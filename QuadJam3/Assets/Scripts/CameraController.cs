using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Data")]
    [SerializeField] private Vector3 _offset = Vector3.zero;
    [SerializeField] private float _dampSpeed = 0.0f;
     
    [Header("Other")]
    [SerializeField] private Transform _player;

    private void Awake()
    {
        if (!_player)
        {
            Debug.LogError("[CameraController]: Failed to fetch Player Transform");
        }
    }

    private void LateUpdate() => OffsetPosition();

    private void OffsetPosition()
    {
        transform.position = _player.position + _offset;
    }
}
