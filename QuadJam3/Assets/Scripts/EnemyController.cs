using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private float _moveSpeed = 0.0f;
    [SerializeField] private float _stoppingDistance = 0.0f;

    [Header("Stats")]
    [SerializeField] private int _health = 0;
    [SerializeField] private float _attackCooldown = 0.0f;

    [Header("Debug")]
    [SerializeField] private bool _enableDebug = false;
    [SerializeField] private float _gizmoDirectionLength = 0.0f;

    // Dependencies
    private Rigidbody2D _rb = null;
    private GameObject _player = null;
    private PlayerController _pController = null;

    private Vector2 _moveDirection = Vector2.zero;

    private State _currentState;

    private float _attackTimer = 0.0f;

    private enum State
    {
        Moving,
        Attacking,
        Knockback
    }

    private void Awake()
    {
        GetDependencies();

        // This line is needed to avoid a weird bug where the enemy would attack the player once,
        // straight after the enemy is first spawned in. 
        // This could be fixed by adding an additional "Idle" state, but this is just a quick fix for now.
        _moveDirection = CalculateMoveDirection();
        _currentState = State.Moving;
    }

    private void Update()
    {
        _moveDirection = CalculateMoveDirection();
        _attackTimer += Time.deltaTime;

        HandleStates();
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
            HandleAttack();
        }
    }

    private void HandleStates()
    {
        if (_moveDirection.magnitude <= _stoppingDistance)
            _currentState = State.Attacking;
    }

    private void HandleAttack()
    {
        if (_attackTimer < _attackCooldown)
            return;

        _pController.TakeDamage();
        _attackTimer = 0.0f;

        _currentState = State.Moving;

        Debug.Log("[EnemyController]: Attacked player");
    }

    private void GetDependencies()
    {
        if (!TryGetComponent(out _rb))
        {
            Debug.LogError("[EnemyController]: Failed to find Rigidbody2D component");
            return;
        }

        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("[EnemyController] Failed to find Player gameobject");
            return;
        }

        if (!_player.TryGetComponent(out _pController))
        {
            Debug.LogError("[EnemyController]: Failed to fetch PlayerController.cs");
            return;
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

    public void TakeDamage(float knockbackAmount)
    {
        _health--;

        // TODO: Implement knockback

        Debug.Log("[EnemyController]: Damage Taken\nCurrent Health: " + _health);

        if (_health < 1)
        {
            // Just destroying the gameobject for now, will probably add something else later
            Destroy(gameObject);
        }
    }
}
