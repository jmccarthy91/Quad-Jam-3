using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Used a lot of this guy's tutorials: https://www.youtube.com/watch?v=AXkaqW3E9OI4

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private int maxHearts = 5;

    [Header("Combat")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackOffset = 3f;
    [SerializeField] private float _knockbackAmount = 1f;
    [SerializeField] private float _attackKnockback = 0.0f;
    
    [Header("Roll")]
    [SerializeField] private float rollSpeedMax = 150f;
    private float rollSpeedMinimum = 50f;
    private float rollSpeedDropMultiplier = 5f;
    private float rollSpeed = 0.0f;

    [Header("Boots")]
    [SerializeField] private float _bootForce = 40f;      //jetpack tutorials https://www.youtube.com/watch?v=gJilpepn3gw
    [SerializeField] private float _bootUseLimit = 0.0f;
    private bool engineIsOn; 
        
    [Header("iframes")]
    [SerializeField] private float iFramesDuration = 1f;
    [SerializeField] private int numFlashes = 3;
    
    [Header("Components")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _pickaxeAnimator;
    [SerializeField] private Transform _spawnPoint;

    [Header("Other")]
    [SerializeField] private float _groundCheckLength = 0.0f;
    [SerializeField] private LayerMask _floorLayer;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private LayerMask _mineralLayer;
    [SerializeField] private float _attackOffset = 0.0f;

    private const string PLAYER_IDLE = "PlayerIdle";
    private const string PLAYER_WALK = "PlayerWalk";
    private const string PLAYER_DASH = "PlayerDash";
    private const string PLAYER_JUMP = "PlayerJump";
    private const string PLAYER_FALL = "PlayerFall";
    private const string PLAYER_ATTACK = "Pickaxe";

    private enum State
    {
        Normal,
        Rolling,
        Attacking,
    }

    private Rigidbody2D _rb = null;
    private Camera _camera = null;

    private Vector2 moveDirection;
    private Vector2 rollDirection;
    private Vector2 lastMoveDirection;
    private State _currentState;

    private float _jetpackTimer = 0.0f;

    private bool _isGrounded = false;
    private bool _shouldMove = true;
    private bool _isRolling = false;

    private int currentHearts;

    private string _currentAnimationState = "";

    private void Awake()
    {
        currentHearts = maxHearts;
        
        _rb = GetComponent<Rigidbody2D>();
        _camera = Camera.main;

        _currentState = State.Normal;
        _currentAnimationState = PLAYER_IDLE;

        engineIsOn = false;
    }

    private void Update()
    {
        CheckGrounded();
        HandleAttack();

        switch (_currentState)
        {
            case State.Normal:
                // move WASD
                HandleMovement();
                // jetpack on spacebar
                HandleJetpack();
                // initiate roll on left shift
                InitiateRoll();
                break;
                        
                // roll & end roll
            case State.Rolling:
                _animator.Play(PLAYER_DASH);
                Roll();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case State.Normal:
                Move();

                if (engineIsOn)
                {
                    _rb.AddForce(new Vector3(0f, _bootForce), ForceMode2D.Force);
                }
                break;

            case State.Rolling:
                _rb.velocity = rollDirection * rollSpeed * Time.fixedDeltaTime;
                break;
        }
    }

    private void HandleMovement()
    {
        float xDirection = Input.GetAxisRaw("Horizontal");
        moveDirection = new Vector2(xDirection, 0f).normalized;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(_camera.ScreenToWorldPoint(Input.mousePosition).x);
        transform.localScale = scale;

        if (xDirection == 0)
            _animator.Play(PLAYER_IDLE);
        else if (xDirection != 0 && !_isRolling)
            _animator.Play(PLAYER_WALK);

        //set a last move direction so we roll even when no directions are pressed
        if (xDirection != 0)
        {
            lastMoveDirection = moveDirection;
        }
    }

    private void Move()
    {
        if (_shouldMove)
        {
            _rb.velocity = new Vector2(moveDirection.x * moveSpeed * Time.fixedDeltaTime, _rb.velocity.y);
        }
    }

    private void HandleAttack()
    {
        Collider2D enemyCol = null;
        Collider2D mineralCol = null;

        if (Input.GetMouseButtonDown(0))
        {
            _pickaxeAnimator.Play(PLAYER_ATTACK);

            float attackDir = Mathf.Sign(_camera.ScreenToWorldPoint(Input.mousePosition).x);
            _attackOffset = attackDir;

            enemyCol = Physics2D.OverlapBox(transform.position + new Vector3(_attackOffset, 0.0f, 0.0f), 
                Vector2.one, 0.0f, _enemyLayer);

            mineralCol = Physics2D.OverlapBox(transform.position + new Vector3(_attackOffset, 0.0f, 0.0f),
                Vector2.one, 0.0f, _mineralLayer);
        }

        if (enemyCol)
        {
            enemyCol.GetComponent<EnemyController>().TakeDamage(_attackKnockback);
        }

        if (mineralCol)
        {
            mineralCol.GetComponent<MiningNode>().Hit();
        }
    }

    private void HandleJetpack()
    {
        if (Input.GetKey(KeyCode.Space) && _jetpackTimer < _bootUseLimit)
        {
            engineIsOn = true;

            _jetpackTimer += Time.deltaTime;

            if (_jetpackTimer >= _bootUseLimit)
            {
                _jetpackTimer = 0.0f;
            }
        }
        else
        {
            engineIsOn = false;
        }
    }

    private IEnumerator AppleKnockback(Vector2 direction)
    {
        if (!_isGrounded)
            yield return null;

        _shouldMove = false;

        _rb.velocity = Vector2.zero;
        _rb.AddForce(new Vector2(Mathf.Sign(transform.position.x - direction.x) * _knockbackAmount, 5.0f),
            ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.25f);

        _shouldMove = true;
    }

    private void InitiateRoll()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            rollDirection = lastMoveDirection;
            rollSpeed = rollSpeedMax;
            _currentState = State.Rolling;
        }
    }

    private void Roll()
    {
        //set invulnerable
        Physics.IgnoreLayerCollision(10, 11, true);
        
        //degrade roll speed
        rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

        if(rollSpeed < rollSpeedMinimum)
        {
            //remove invulnerable
            Physics.IgnoreLayerCollision(10, 11, false);
            _currentState = State.Normal;
        }
    }
    
    public void TakeDamage(Vector2 enemyPos)
    {
        currentHearts -= 1;

        StartCoroutine(AppleKnockback(enemyPos));

        if (currentHearts >= 0)
        {
            StartCoroutine(Invulnerability());
        }
    }

    private IEnumerator Invulnerability() // will want to turn this on when I take damage
    {
        Physics.IgnoreLayerCollision(10, 11, true);
        
        //flash red a few times
        for (int i = 0; i < numFlashes; i++)
        {
            _spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numFlashes * 2));
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numFlashes * 2));
        }
        Physics.IgnoreLayerCollision(10, 11, false);
    }

    private void Respawn()
    {
        gameObject.SetActive(false);
        transform.position = _spawnPoint.position;
        gameObject.SetActive(true);
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckLength, _floorLayer);

        if (!_isGrounded)
        {
            _jetpackTimer += Time.deltaTime;

            if (_rb.velocity.y > 0)
            {

            }
            else
            {

            }
        }
        else
        {

            _jetpackTimer = 0.0f;
        }
    }

    public bool IsGrounded() => _isGrounded;

    private void SetAnimationState(string newState)
    {
        if (_currentAnimationState == newState)
            return;

        _currentAnimationState = newState;

        _animator.Play(newState);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, Vector2.down * _groundCheckLength);

        if (Input.GetMouseButton((int)MouseButton.LeftMouse))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + new Vector3(_attackOffset, 0.0f, 0.0f), Vector2.one);
        }
    }
} 
