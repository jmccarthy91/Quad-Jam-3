using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Data")]
    [SerializeField] private Vector3 _offset;
    [SerializeField] private int _maxFOV;
    [SerializeField] private bool _enableZoom;
     
    [Header("Other")]
    [SerializeField] private Transform _player;

    private Camera _camera;

    private void Awake()
    {
        if (!_player)
        {
            Debug.LogError("[CameraController]: Failed to fetch Player Transform");
        }

        _camera = Camera.main;
    }

    private void Update()
    {
        if (_enableZoom)
        {
            Zoom();
        }
    }

    private void LateUpdate() => OffsetPosition();

    private void OffsetPosition() => transform.position = _player.position + _offset;

    private void Zoom()
    {
        _camera.fieldOfView += MouseSingleton.GetMouseWheelValue().y;
        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, 60, _maxFOV);
    }
}
