using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private float _moveSpeed = 0.0f;
    [SerializeField] private float _stoppingDistance = 0.0f;

    [Header("Stats")]
    [SerializeField] private int _health = 0;

    [Header("Debug")]
    [SerializeField] private bool _enableDebug = false;
    [SerializeField] private float _gizmoDirectionLength = 0.0f;

    // Dependencies
    private Rigidbody2D _rb = null;
    private GameObject _player = null;

    private Vector2 _moveDirection = Vector2.zero;

    private State _currentState;

    private enum State
    {
        Moving,
        Attacking
    }

    private void Awake()
    {
        GetDependencies();
    }

    private void Update()
    {
        _moveDirection = CalculateMoveDirection();
        HandleStateChanges();
    }

    private void FixedUpdate() => HandleMovement();

    private Vector2 CalculateMoveDirection()
    {
        return _player.transform.position - transform.position;
    }

    private void HandleMovement()
    {
        if (_moveDirection.magnitude > _stoppingDistance)
        {
            _rb.velocity = _moveDirection.normalized * _moveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
    }

    private void HandleStateChanges()
    {
        if (_rb.velocity != Vector2.zero)
        {
            _currentState = State.Moving;
        }
        else
        {
            _currentState = State.Attacking;
        }
    }

    private void GetDependencies()
    {
        if (!TryGetComponent<Rigidbody2D>(out _rb))
        {
            Debug.LogError("[EnemyController]: Failed to find Rigidbody2D component");
        }

        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("[EnemyController] Failed to find Player gameobject");
        }
    }

    private void OnDrawGizmos()
    {
        if (!_enableDebug)
            return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, _moveDirection.normalized * _gizmoDirectionLength);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _moveDirection.normalized * _stoppingDistance);
    }
}
