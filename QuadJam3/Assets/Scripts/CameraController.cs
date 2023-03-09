using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Data")]
    [SerializeField] private Vector2 _offset;
     
    [Header("Other")]
    [SerializeField] private Transform _player;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;

        if (!_player)
        {
            Debug.LogError("[CameraController]: Failed to fetch Player Transform");
        }
    }

    private void LateUpdate() => OffsetPosition();

    private void OffsetPosition() =>
        transform.position = _player.position + new Vector3(_offset.x, _offset.y, _camera.transform.position.z);
}
