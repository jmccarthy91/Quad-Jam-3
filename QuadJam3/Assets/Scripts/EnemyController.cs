using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private float _moveSpeed = 0.0f;
    [SerializeField] private float _stoppingDistance = 0.0f;

    [Header("Stats")]
    [SerializeField] private int _health = 0;
    [SerializeField] private float _attackCooldown = 0.0f;

    [Header("Other")]
    [SerializeField] private GameObject _heartObject = null;

    [Header("Debug")]
    [SerializeField] private bool _enableDebug = false;
    [SerializeField] private float _gizmoDirectionLength = 0.0f;

    // Dependencies
    private Rigidbody _rb = null;
    private GameObject _player = null;
    private PlayerController _pController = null;

    private Vector3 _moveDirection = Vector3.zero;

    private float _playerDistance = 0.0f;
    private float _attackTimer = 0.0f;

    private bool _shouldMove = true;

    private void Awake()
    {
        GetDependencies();

        // This line is needed to avoid a weird bug where the enemy would attack the player once,
        // straight after the enemy is first spawned in. 
        _moveDirection = new Vector3(CalculateMoveDirection().x, 0.0f, CalculateMoveDirection().z);
        _playerDistance = _moveDirection.magnitude;
        
        _shouldMove = true;
    }

    private void Update()
    {
        _moveDirection = new Vector3(CalculateMoveDirection().x, 0.0f, CalculateMoveDirection().z);
        _playerDistance = CalculatePlayerDistance();
        _attackTimer += Time.deltaTime;

        if (_playerDistance <= _stoppingDistance)
        {
            HandleAttack();
        }

        // This is just for testing
        // Applying knockback should be done from the player script
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10.0f);
        }
    }

    private void FixedUpdate() => HandleMovement();

    private Vector3 CalculateMoveDirection()
    {
        return (_player.transform.position - transform.position);
    }

    private float CalculatePlayerDistance() => _moveDirection.magnitude;

    private void HandleMovement()
    {
        if (_playerDistance > _stoppingDistance && _shouldMove)
        {
            _rb.velocity = _moveDirection.normalized * _moveSpeed * Time.fixedDeltaTime;
        }
        else if (_playerDistance <= _stoppingDistance)
        {
            _rb.velocity = Vector3.zero;
        }
    }

    private IEnumerator AppleKnockback(float amount)
    {
        _shouldMove = false;

        _rb.velocity = Vector3.zero;
        _rb.AddForce(-_moveDirection.normalized * amount, ForceMode.Impulse);

        yield return new WaitForSeconds(0.25f);

        _shouldMove = true;
    }

    private void HandleAttack()
    {
        if (_attackTimer < _attackCooldown)
            return;

        _pController.TakeDamage();
        _attackTimer = 0.0f;

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
        if (!_player)
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
        Debug.Log("[EnemyController]: Damage Taken\nCurrent Health: " + _health);

        StartCoroutine(AppleKnockback(knockbackAmount));

        if (_health < 1)
        {
            DropHeart();
            Destroy(gameObject);
        }
    }

    private void DropHeart()
    {
        Instantiate(_heartObject, transform.position, transform.rotation);
    }
}
