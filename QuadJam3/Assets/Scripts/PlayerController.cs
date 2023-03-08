using System.Collections;
using System.IO;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Used a lot of this guy's tutorials: https://www.youtube.com/watch?v=AXkaqW3E9OI4

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private int maxHearts = 5;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackOffset = 3f;
    [SerializeField] public float _knockbackAmount = 1f;
    private int currentHearts;

    [Header("Roll")]
    [SerializeField] private float rollSpeedMax = 150f;
    private float rollSpeedMinimum = 50f;
    private float rollSpeedDropMultiplier = 5f;
    private float rollSpeed = 0.0f;

    [Header("Boots")]
    [SerializeField] private float _bootForce = 40f;      //jetpack tutorials https://www.youtube.com/watch?v=gJilpepn3gw
    [SerializeField] private float _bootUseForce = 0.0f;
    [SerializeField] private float _bootUseLimit = 0.0f;
    private bool engineIsOn; 

    [Header("iframes")]
    [SerializeField] private float iFramesDuration = 1f;
    [SerializeField] private int numFlashes = 3;
    
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPoint;       //Callum, this is the part I can't figure out
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private LayerMask mineralLayers;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Other")]
    [SerializeField] private float _groundCheckLength = 0.0f;
    [SerializeField] private LayerMask _floorLayer;

    private enum State
    {
        Normal,
        Rolling,
        Attacking,
    }

    private Rigidbody2D _rb = null;

    private Vector2 moveDirection;
    private Vector2 rollDirection;
    private Vector2 lastMoveDirection;
    private State state;

    private float _jetpackTimer = 0.0f;
    private float _cachedDir = 0.0f;

    private bool _isGrounded = false;
    private bool _shouldMove = true;


    private void Awake()
    {
        currentHearts = maxHearts;
        
        _rb = GetComponent<Rigidbody2D>();

        state = State.Normal;

        engineIsOn = false;
    }

    private void Update()
    {
        CheckGrounded();

        switch (state)
        {
            case State.Normal:
                // move WASD
                HandleMovement();
                // attack on mouse
                HandleAttack();
                // jetpack on spacebar
                HandleJetpack();
                // initiate roll on left shift
                InitiateRoll();
                break;
                        
                // roll & end roll
            case State.Rolling:
                Roll();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
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

        if (xDirection == -1f)
            _cachedDir = -1f;
        else if (xDirection == 1)
            _cachedDir = 1f;

        _spriteRenderer.flipX = moveDirection.x < 0;

        //set a last move direction so we roll even when no directions are pressed
        if (xDirection != 0)
        {
            lastMoveDirection = moveDirection;
        }
    }

    private void Move()
    {
        if (_shouldMove)
            _rb.velocity = new Vector2(moveDirection.x * moveSpeed * Time.fixedDeltaTime, _rb.velocity.y);
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // play attack animation and set state to attacking until animation finishes
            // animator.SetTrigger("Attack");
            //state = State.Attacking;
            //characterBase.PlayAttackAnimation(attackDirection, () => state = State.Normal);
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
            state = State.Rolling;
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
            state = State.Normal;
        }
    }
    
    public void TakeDamage(Vector2 enemyPos)
    {
        currentHearts -= 1;

        // Debug.Log("[PlayerController]: Took damage.\nRemaining health: " + currentHearts);

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
        }
        else
        {
            _jetpackTimer = 0.0f;
        }
    }

    public bool IsGrounded() => _isGrounded;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, Vector2.down * _groundCheckLength);
    }
} 
