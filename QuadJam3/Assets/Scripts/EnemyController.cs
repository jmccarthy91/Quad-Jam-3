using System;
using System.Collections;
using UnityEditor.Build;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private float _moveSpeed = 0.0f;
    [SerializeField] private float _stoppingDistance = 0.0f;
    [SerializeField] private float _knockbackUp = 0.0f;

    [Header("Stats")]
    [SerializeField] private int _health = 0;
    [SerializeField] private float _attackCooldown = 0.0f;

    [Header("Other")]
    [SerializeField] private GameObject _heartObject = null;
    [SerializeField] private GameObject _player = null;

    [Header("Debug")]
    [SerializeField] private bool _enableDebug = false;
    [SerializeField] private float _gizmoDirectionLength = 0.0f;

    // Dependencies
    private Rigidbody2D _rb = null;
    private PlayerController _pController = null;

    private Vector2 _moveDirection = Vector2.zero;

    private float _playerDistance = 0.0f;
    private float _attackTimer = 0.0f;

    private bool _shouldMove = true;
    private bool _beingAttacked = false;

    private void Awake()
    {
        GetDependencies();

        _moveDirection = new Vector2(CalculateMoveDirection().x, 0.0f);
        _playerDistance = _moveDirection.magnitude;
        
        _shouldMove = true;
    }

    private void Update()
    {
        _moveDirection = new Vector2(CalculateMoveDirection().x, 0.0f);
        _playerDistance = CalculatePlayerDistance();
        _attackTimer += Time.deltaTime;

        if (_playerDistance <= _stoppingDistance)
        {
            HandleAttack();
        }
    }

    private void FixedUpdate() => HandleMovement();

    private Vector2 CalculateMoveDirection()
    {
        return (_player.transform.position - transform.position);
    }

    private float CalculatePlayerDistance() => _moveDirection.magnitude;

    private void HandleMovement()
    {
        if (_playerDistance > _stoppingDistance && _shouldMove)
        {
            _rb.velocity = new Vector2(_moveDirection.x * _moveSpeed * Time.fixedDeltaTime, _rb.velocity.y);
        }
        else if (_playerDistance <= _stoppingDistance && !_beingAttacked)
        {
            _rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator AppleKnockback(float amount)    
    {
        _beingAttacked = true;
        _shouldMove = false;

        _rb.velocity = Vector2.zero;
        _rb.AddForce(new Vector2(-Mathf.Sign(_moveDirection.x) * amount, 5.0f),
            ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.25f);

        _shouldMove = true;
        _beingAttacked = false;
    }

    private void HandleAttack()
    {
        if (_attackTimer < _attackCooldown || !_pController.IsGrounded())
            return;

        _pController.TakeDamage(transform.position);
        _attackTimer = 0.0f;
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
        StartCoroutine(AppleKnockback(knockbackAmount));

        _health--;

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
