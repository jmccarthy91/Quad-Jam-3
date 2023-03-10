using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Used a lot of this guy's tutorials: https://www.youtube.com/watch?v=AXkaqW3E9OI4

    [Header("Stats")]
    [SerializeField] private float _moveSpeed = 15f;
    [SerializeField] private int _maxHearts = 5;

    [Header("Combat")]
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _knockbackAmount = 1f;
    [SerializeField] private float _attackKnockback = 0.0f;
    
    [Header("Roll")]
    [SerializeField] private float rollSpeedMax = 150f;
    
    [Header("Boots")]
    [SerializeField] private float _bootForce = 40f;      //jetpack tutorials https://www.youtube.com/watch?v=gJilpepn3gw
    [SerializeField] private float _bootUseLimit = 0.0f;

    [Header("Upgrades")]
    [SerializeField] private float _bootForceUpgrade = 0.0f;
    [SerializeField] private float _knockbackUpgrade = 0.0f;
        
    [Header("iframes")]
    [SerializeField] private float _iFramesDuration = 1f;
    [SerializeField] private int _numFlashes = 3;
    
    [Header("Components")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _pickaxeAnimator;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _upgradeCanvas;

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
    private AudioManager _audioManager = null;

    private Vector2 moveDirection;
    private Vector2 rollDirection;
    private Vector2 lastMoveDirection;
    private State _currentState;

    private float _jetpackTimer = 0.0f;
    private float _cachedDirection = 0.0f;
    private float _rollSpeedMinimum = 50f;
    private float _rollSpeedDropMultiplier = 5f;
    private float _rollSpeed = 0.0f;

    private bool _isGrounded = false;
    private bool _shouldMove = true;
    private bool _isRolling = false;
    private bool _isInvulnerable = false;
    private bool _engineIsOn = false;
    private bool _canUseBoots = false;
    private bool _inAir;

    private int _currentHearts;

    private string _currentAnimationState = "";

    private void Awake()
    {
        _currentHearts = _maxHearts;
        
        _rb = GetComponent<Rigidbody2D>();
        _audioManager = FindObjectOfType<AudioManager>();

        _currentState = State.Normal;
        _currentAnimationState = PLAYER_IDLE;

        _cachedDirection = 1;

        _engineIsOn = false;
    }

    private void Start()
    {
        EventManager.Current.OnMineralMined += ProvideUpgrades;

        _upgradeCanvas.GetComponent<DebugUpgradeUI>().OnUpgradeBoots += UpgradeBoots;
        _upgradeCanvas.GetComponent<DebugUpgradeUI>().OnUpgradePickaxe += UpgradePickaxe;
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

                if (_canUseBoots && _jetpackTimer <= _bootUseLimit)
                {
                    _jetpackTimer += Time.deltaTime;

                    //_rb.velocity += new Vector2(0.0f, 1.0f * _bootForce * Time.fixedDeltaTime);
                    _rb.AddForce(Vector2.up * _bootForce, ForceMode2D.Force);
                }
                break;

            case State.Rolling:
                _rb.velocity = new Vector2(rollDirection.x * _rollSpeed * Time.fixedDeltaTime, _rb.velocity.y);
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
            _rb.velocity = new Vector2(moveDirection.x * _moveSpeed * Time.fixedDeltaTime, _rb.velocity.y);
        }
    }

    private void HandleAttack()
    {
        Collider2D enemyCol = null;
        Collider2D mineralCol = null;

        if (Input.GetMouseButtonDown(0))
        {
            _pickaxeAnimator.Play(PLAYER_ATTACK);
            _audioManager.Play("PickSwing");

            Vector3 attackDir = new(_cachedDirection * _attackRange, 0.0f, 0.0f);

            enemyCol = Physics2D.OverlapBox(transform.position + attackDir, Vector2.one,
                angle: 0.0f, _enemyLayer);

            mineralCol = Physics2D.OverlapBox(transform.position + attackDir, Vector2.one,
                angle: 0.0f, _mineralLayer);
        }

        if (enemyCol)
        {
            enemyCol.GetComponent<EnemyController>().TakeDamage(_attackKnockback);
            _audioManager.Play("SlimeHit");
        }

        if (mineralCol)
        {
            _audioManager.Play("MineralHit");
            mineralCol.GetComponent<MiningNode>().Hit();
        }
    }

    private void HandleJetpack()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _canUseBoots = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _canUseBoots = false;
        }

        if (_jetpackTimer >= _bootUseLimit && !_inAir)
        {
            if (!_isGrounded)
            {
                _inAir = true;
            }

            _jetpackTimer = 0.0f;
        }

        if (_isGrounded)
            _inAir = false;
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
            _rollSpeed = rollSpeedMax;
            _currentState = State.Rolling;
        }
    }

    private void Roll()
    {
        _isInvulnerable = true;
        _audioManager.Play("Roll");

        //degrade roll speed
        _rollSpeed -= _rollSpeed * _rollSpeedDropMultiplier * Time.deltaTime;

        if(_rollSpeed < _rollSpeedMinimum)
        {
            _isInvulnerable = false;
            _currentState = State.Normal;
        }
    }
    
    public void TakeDamage(Vector2 enemyPos)
    {
        if (_isInvulnerable)
            return;
        
        _currentHearts -= 1;
        StartCoroutine(AppleKnockback(enemyPos));
        // HUDEventsManager.EventsHUD.OnHealthChange(currentHearts);
        _audioManager.Play("PlayerHit1");
        _audioManager.Play("PlayerHit2");

        if (_currentHearts >= 0)
        {
            StartCoroutine(Invulnerability());
        }
    }

    private IEnumerator Invulnerability() // will want to turn this on when I take damage
    {
        _isInvulnerable = true;

        //flash red a few times
        for (int i = 0; i < _numFlashes; i++)
        {
            _spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(_iFramesDuration / (_numFlashes * 2));
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(_iFramesDuration / (_numFlashes * 2));
        }

        _isInvulnerable = false;
    }

    private void Respawn()
    {
        gameObject.SetActive(false);
        transform.position = _spawnPoint.position;
        _audioManager.Play("Respawn");
        gameObject.SetActive(true);
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckLength, _floorLayer);

        if (!_isGrounded)
        {
            _jetpackTimer += Time.deltaTime;
        }
        else
        {
            _jetpackTimer = 0.0f;
            //HUDEventsManager.EventsHUD.OnJetpackEnded(0.5f);
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

    private void ProvideUpgrades()
    {
        // Need UI to implement the upgrades
        _upgradeCanvas.SetActive(true);
        Debug.Log("Can upgrade");
    }

    private void UpgradeBoots()
    {
        Debug.Log("Boots Upgraded!");
        _bootForce += _bootForceUpgrade;
        _upgradeCanvas.SetActive(false);
    }

    private void UpgradePickaxe()
    {
        Debug.Log("Pickaxe Upgraded!");
        _knockbackAmount += _knockbackUpgrade;
        _upgradeCanvas.SetActive(false);
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
