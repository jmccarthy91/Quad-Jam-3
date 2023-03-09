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
    [SerializeField] private float _attackRange = 3f;
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
    private float _cachedDirection = 0.0f;

    private bool _isGrounded = false;
    private bool _shouldMove = true;
    private bool _isRolling = false;
    private bool _isInvulnerable = false;

    private int currentHearts;

    private string _currentAnimationState = "";

    private void Awake()
    {
        currentHearts = maxHearts;
        
        _rb = GetComponent<Rigidbody2D>();
        _camera = Camera.main;

        _currentState = State.Normal;
        _currentAnimationState = PLAYER_IDLE;

        _cachedDirection = 1;

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
                    _rb.AddForce(Vector2.up * _bootForce * Time.fixedDeltaTime, ForceMode2D.Force);
                }
                break;

            case State.Rolling:
                _rb.velocity = new Vector2(rollDirection.x * rollSpeed * Time.fixedDeltaTime, _rb.velocity.y);
                break;
        }
    }

    private void HandleMovement()
    {
        float xDirection = Input.GetAxisRaw("Horizontal");
        moveDirection = new Vector2(xDirection, 0f).normalized;

        HandlePlayerFlip(xDirection);

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
            // FindObjectOfType<AudioManager>().Play("PickSwing");

            Vector3 attackDir = new(_cachedDirection * _attackRange, 0.0f, 0.0f);

            enemyCol = Physics2D.OverlapBox(transform.position + attackDir, Vector2.one,
                angle: 0.0f, _enemyLayer);

            mineralCol = Physics2D.OverlapBox(transform.position + attackDir, Vector2.one,
                angle: 0.0f, _mineralLayer);
        }

        if (enemyCol)
        {
            enemyCol.GetComponent<EnemyController>().TakeDamage(_attackKnockback);
            // FindObjectOfType<AudioManager>().Play("SlimeHit");
        }

        if (mineralCol)
        {
            mineralCol.GetComponent<MiningNode>().Hit();
            // FindObjectOfType<AudioManager>().Play("MineralHit");
        }
    }

    private void HandleJetpack()
    {
        if (Input.GetKey(KeyCode.Space) && _jetpackTimer < _bootUseLimit)
        {
            if(!engineIsOn)
            {
              HUDEventsManager.EventsHUD.OnJetpackStarted(0.5f);
            }
            engineIsOn = true;
            // FindObjectOfType<AudioManager>().Play("RocketBoots");

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
        if (Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded())
        {
            rollDirection = lastMoveDirection;
            rollSpeed = rollSpeedMax;
            _currentState = State.Rolling;
        }
    }

    private void Roll()
    {
        _isInvulnerable = true;
        // FindObjectOfType<AudioManager>().Play("Roll");

        //degrade roll speed
        rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

        if(rollSpeed < rollSpeedMinimum)
        {
            _isInvulnerable = false;
            _currentState = State.Normal;
        }
    }
    
    public void TakeDamage(Vector2 enemyPos)
    {
        if (_isInvulnerable)
            return;
        
        currentHearts -= 1;
        StartCoroutine(AppleKnockback(enemyPos));
        HUDEventsManager.EventsHUD.OnHealthChange(currentHearts);
        // FindObjectOfType<AudioManager>().Play("PlayerHit1");
        // FindObjectOfType<AudioManager>().Play("PlayerHit2");

        if (currentHearts >= 0)
        {
            StartCoroutine(Invulnerability());
        }
    }

    private IEnumerator Invulnerability() // will want to turn this on when I take damage
    {
        _isInvulnerable = true;

        //flash red a few times
        for (int i = 0; i < numFlashes; i++)
        {
            _spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numFlashes * 2));
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numFlashes * 2));
        }

        _isInvulnerable = false;
    }

    private void Respawn()
    {
        gameObject.SetActive(false);
        transform.position = _spawnPoint.position;
        //  FindObjectOfType<AudioManager>().Play("Respawn");
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
            HUDEventsManager.EventsHUD.OnJetpackEnded(0.5f);
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

    private void HandlePlayerFlip(float dir)
    {
        Vector3 scale = transform.localScale;

        if (dir == -1)
        {
            _cachedDirection = -1;
            scale.x = -1;
        }
        else if (dir == 1)
        {
            _cachedDirection = 1;
            scale.x = 1;
        }

        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, Vector2.down * _groundCheckLength);

        if (Input.GetMouseButton((int)MouseButton.LeftMouse))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + new Vector3(_cachedDirection * _attackRange, 0.0f, 0.0f),
                Vector2.one);
        }
    }
} 
