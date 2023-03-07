using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float maxHearts = 5f;
    [SerializeField] float rollSpeedMax = 150f;

    [SerializeField] float currentHearts;
    float rollSpeed;

    enum State
    {
        Normal,
        Rolling,
    }

    public Rigidbody2D rb;
    
    Vector2 moveDirection;
    Vector2 rollDirection;
    State state;


    private void Awake()
    {
        currentHearts = maxHearts;
        rb = GetComponent<Rigidbody2D>();
        state = State.Normal;
    }

    void Update()
    {
        //FaceMouse();

        switch (state)
        {
            case State.Normal:

                ProcessInputs();

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    rollDirection = moveDirection;
                    rollSpeed = rollSpeedMax;
                    state = State.Rolling;
                }

                break;
            case State.Rolling:
                float rollSpeedDropMultiplier = 5f;
                rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

                float rollSpeedMinimum = 50f;
                if (rollSpeed < rollSpeedMinimum)
                {
                    state = State.Normal;
                }
                break;
        }
    }

    void FixedUpdate()
    {
            switch (state)
            {
                case State.Normal:
                    Move();
                    break;
                case State.Rolling:
                    rb.velocity = rollDirection * rollSpeed;
                    break;
            }


    }

    void ProcessInputs()
    {
        // get keyboard inputs
        float xDirection = Input.GetAxisRaw("Horizontal");
        float yDirection = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(xDirection, yDirection).normalized;
    }

    void Move()
    {
        // use inputs to move
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
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
