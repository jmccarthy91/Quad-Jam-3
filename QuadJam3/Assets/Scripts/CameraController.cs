using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Transform _player;

    private void Update()
    {
        transform.position = _player.position + _offset;
    }
}
