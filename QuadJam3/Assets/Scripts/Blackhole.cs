using UnityEngine;

public class Blackhole : MonoBehaviour
{
    [Header("Parallax")]
    [SerializeField] private float m_smoothTime = 0.0f;

    private Vector3 m_velocity = Vector3.zero;

    private Transform m_cameraTransform = null;

    private void Awake() => m_cameraTransform = Camera.main.transform;

    private void Update()
    {
        Vector3 t = new(m_cameraTransform.position.x, m_cameraTransform.position.y, transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, t, ref m_velocity, m_smoothTime);
    }
}
