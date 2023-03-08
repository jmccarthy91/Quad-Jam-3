using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Used a lot of this guy's tutorials: https://www.youtube.com/watch?v=AXkaqW3E9OI

    [Header("Stats")]
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] int maxHearts = 5;
    [SerializeField] float attackRange = 3f;
    [SerializeField] float attackOffset = 3f;
    [SerializeField] public float knockback = 1f;
    [SerializeField] float playerGravity = 5f;
    public int currentHearts;

    [Header("Roll")]
    [SerializeField] float rollSpeedMax = 150f;
    float rollSpeedMinimum = 50f;
    float rollSpeedDropMultiplier = 5f;
    float rollSpeed;

    [Header("Jetpack")]
    [SerializeField] float jetForce = 40f;      //jetpack tutorials https://www.youtube.com/watch?v=gJilpepn3gw
    [SerializeField] private GameObject fire;
    private bool engineIsOn; 

    [Header("iframes")]
    [SerializeField] float iFramesDuration = 1f;
    [SerializeField] int numFlashes = 3;
    SpriteRenderer spriteRenderer;
    
    [Header("Components")]
    public Rigidbody rb;
    public Animator animator;
    public Transform attackPoint;                                       //Callum, this is the part I can't figure out
    public LayerMask enemyLayers;
    public LayerMask mineralLayers;
    public EnemyController enemyController;

    enum State
    {
        Normal,
        Rolling,
        Attacking,
    }

    
    Vector3 moveDirection;
    Vector3 rollDirection;
    Vector3 lastMoveDirection;
    State state;


    private void Awake()
    {
        currentHearts = maxHearts;
        
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyController = GameObject.Find("Enemy").GetComponent<EnemyController>();

        // EventManager.current.onHitPlayer += onHitPlayer;

        state = State.Normal;

        engineIsOn = false;
        fire.SetActive(false);
    }

    void Update()
    {
        //FaceMouse();

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

            //case State.Attacking                                                      //if needed later for animations
            //    HandleAttack();                                                       //lets us spam attacks, may not want! if we remove this line then we attack @ speed of animation
            //    break;
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector3(0, -1f, 0) * playerGravity);

        switch (state)
        {
            case State.Normal:
                Move();

                // Jetpack
                switch (engineIsOn)
                {
                    case true:
                        rb.AddForce(new Vector3(0f, jetForce, 0f), ForceMode.Force);
                        break;

                    case false:
                        rb.AddForce(new Vector3(0f, 0f, 0f), ForceMode.Force);
                        break;
                }

                break;

            //roll
            case State.Rolling:
                rb.velocity = rollDirection * rollSpeed;

                break;
        }
    }

    void HandleMovement()
    {
        // get keyboard inputs
        float xDirection = Input.GetAxisRaw("Horizontal");
        float zDirection = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(xDirection, 0f, zDirection).normalized;
        
        //set a last move direction so we roll even when no directions are pressed
        if (xDirection != 0 || zDirection != 0)
        {
            lastMoveDirection = moveDirection;
        }
    }

    void Move()
    {
        // use inputs to move
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, 0f, moveDirection.z * moveSpeed);
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // play attack animation and set state to attacking until animation finishes
            animator.SetTrigger("Attack");
            //state = State.Attacking;
            //characterBase.PlayAttackAnimation(attackDirection, () => state = State.Normal);

            // detect enemies AND MINERALS in range
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
            Collider[] hitMinerals = Physics.OverlapSphere(attackPoint.position, attackRange, mineralLayers);

            //damage them
            foreach (Collider enemy in hitEnemies)
            {
                Debug.Log("We hit " + enemy.name);
                enemyController.TakeDamage(knockback);

            }

            foreach (Collider mineral in hitMinerals)
            {
                Debug.Log("We hit " + mineral.name);
                EventManager.Current.MineralHit();
            }

            // @Callum, I think the below is close to getting attackPoint in the way I want using attackOffset, but I don't think it's working properly in 3D:

            //Vector3 mousePosition = MouseSingleton.GetMouseWorldPosition();
            //Vector3 mouseDirection = (mousePosition - transform.position).normalized;
            //Vector3 attackPosition = transform.position + mouseDirection * attackOffset;
            //Vector3 attackDirection = mouseDirection;

        }
    }

    void HandleJetpack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            engineIsOn = true;
            fire.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            engineIsOn = false;
            fire.SetActive(false);
        }
    }

    void InitiateRoll()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            rollDirection = lastMoveDirection;
            rollSpeed = rollSpeedMax;
            state = State.Rolling;
        }
    }

    void Roll()
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
    
    public void TakeDamage()
    {
        currentHearts -= 1;

        Debug.Log("[PlayerController]: Took damage.\nRemaining health: " + currentHearts);

        if (currentHearts >= 0)
        {
            Invulnerability();
        }
        else
        {
            FindObjectOfType<GameManager>().EndGame();
        }
    }

    IEnumerator Invulnerability()                                               // will want to turn this on when I take damage
    {
        Physics.IgnoreLayerCollision(10, 11, true);
        
        //flash red a few times
        for (int i = 0; i < numFlashes; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numFlashes * 2));
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numFlashes * 2));
        }
        Physics.IgnoreLayerCollision(10, 11, false);
    }
    
    // if we want the character to face the mouse
    //void FaceMouse()
    //{
    //    Vector3 mousePosition = Input.mousePosition;
    //    mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

    //    Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

    //    transform.up = direction;
    //}

} 
