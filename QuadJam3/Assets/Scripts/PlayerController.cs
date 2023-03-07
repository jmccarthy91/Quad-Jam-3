using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Used a lot of this guy's tutorials: https://www.youtube.com/watch?v=AXkaqW3E9OI

    [Header("Stats")]
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float maxHearts = 5f;
    [SerializeField] float attackOffset = 10f;
    [SerializeField] float attackRange = 30f;
    float currentHearts;

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

    enum State
    {
        Normal,
        Rolling,
        Attacking,
    }

    public Rigidbody rb;
    
    Vector2 moveDirection;
    Vector2 rollDirection;
    Vector2 lastMoveDirection;
    State state;


    private void Awake()
    {
        currentHearts = maxHearts;
        
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
            switch (state)
            {
                case State.Normal:
                    Move();

                    // Jetpack
                    switch (engineIsOn)
                    {
                        case true:
                            rb.AddForce(new Vector3(0f, jetForce, 0.0f), ForceMode.Force);
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
        float yDirection = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(xDirection, yDirection).normalized;
        
        //set a last move direction so we roll even when no directions are pressed
        if (xDirection != 0 || yDirection != 0)
        {
            lastMoveDirection = moveDirection;
        }
    }

    void Move()
    {
        // use inputs to move
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = MouseSingleton.GetMouseWorldPosition();
            Vector3 mouseDirection = (mousePosition - transform.position).normalized;
            Vector3 attackPosition = transform.position + mouseDirection * attackOffset;
            Vector3 attackDirection = mouseDirection;

            //Enemy targetEnemy = Enemy.GetClosestEnemy(attackPosition, attackRange);                   //talk through this w/ Callum. How to pass him knockback vector?
            //if (targetEnemy != null)                                                                  //if we can get a list of enemies AND minerals then I can handle minerals here too
            //{
            //    //have an enemy, attack :)
            //    attackDirection = targetEnemy.GetPosition() - transform.position.normalized;          //switch direction to be enemy direction (for knockback) rather than 
            //    targetEnemy.Knockback(transform.position);                                            //rename this to whatever Callum's damage enemy function 
            //}

            moveDirection = Vector3.zero;
            Debug.Log("" + attackDirection);
            //state = State.Attacking;                                                                  // using attacking state purely for animation 5:30ish here https://www.youtube.com/watch?v=AXkaqW3E9OI
            //characterBase.PlayAttackAnimation(attackDirection, () => state = State.Normal);                 // end attack state with animation
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
    
    private void onHitPlayer()
    {
        currentHearts -= 1;
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
