using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed;
    public Vector2 movementDir;

    public PlayerControls playerControls;
    public InputAction move;
    public InputAction dash;
    //public InputAction attack;
    //public InputAction spell;

    [Header("Movement")] 
    public float acceleration;
    public bool canMove;
    public float dashForce;
    public float frictionAmount;
    public bool isDashing;
    public float dashLenght;

    public int dashAvailable;
    public float dashCooldownLenght;
    public float dashCooldownTimer;

    [Header("Components")] [HideInInspector]
    public Rigidbody rb;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Extra Values")] [HideInInspector]
    //public int groundDetectionLayerMask;
    public float horizontalMovement;

    //used to change direction accordingly to sprite direction
    public int dirCoef;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        canMove = true;
    }

    void Update()
    {
        JostickDir();
        horizontalMovement = Mathf.Abs(rb.velocity.x);
        //Flip(rb.velocity.x);
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            MovePlayer();
        }
        Timer();
    }

    #region Basic Movement

    void MovePlayer()
    {
        if (movementDir!= Vector2.zero && !isDashing)
        {
            rb.AddForce(new Vector3(movementDir.x * acceleration, 0, movementDir.y * acceleration));
            
            //cap x speed
            if(rb.velocity.x > maxSpeed)
            {
                rb.velocity = new Vector3(maxSpeed, rb.velocity.y, rb.velocity.z);
            }
            if(rb.velocity.x < -maxSpeed)
            {
                rb.velocity = new Vector3(-maxSpeed, rb.velocity.y, rb.velocity.z);
            }
            
            //cap x speed
            if(rb.velocity.z > maxSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, maxSpeed);
            }
            if(rb.velocity.z < -maxSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -maxSpeed);
            }
        }
        else
        {
            //Friction
            if (!isDashing)
            {
                rb.velocity = new Vector3(rb.velocity.x / frictionAmount, rb.velocity.y, rb.velocity.z / frictionAmount);
            }
        }
    }

    void Dash(InputAction.CallbackContext context)
    {
        if (!isDashing && dashAvailable > 0 && dashCooldownTimer <= 0)
        {
            Debug.Log("Dashed");
            dashAvailable--;
            isDashing = true;
            StartCoroutine(DashSequence());
        }
    }

    IEnumerator DashSequence()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        canMove = false;
        //applies dash force
        if (movementDir != Vector2.zero)
        {
            rb.AddForce(new Vector3(dashForce * movementDir.x, 0, dashForce * movementDir.y), ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(dashForce * Vector3.right * dirCoef, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(dashLenght);
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        isDashing = false;
        canMove = true;
        dashAvailable++;
        dashCooldownTimer = dashCooldownLenght;
    }
    #endregion
    
        
    #region Flip

        void Flip(float velocity)
        {
            //Flips the sprite when turning around
            if (movementDir.x > 0.1f)
            {
                spriteRenderer.flipX = false;
            }

            else if (movementDir.x < -0.1f)
            {
                spriteRenderer.flipX = true;
            }

            if (!spriteRenderer.flipX)
            {
                dirCoef = 1;
            }
            else
            {
                dirCoef = -1;
            }
        }

        #endregion
        

    #region InputSystemRequirements
        private void OnEnable()
        {
            move = playerControls.Player.Move;
            move.Enable();

            dash = playerControls.Player.Dash;
            dash.Enable();
            dash.performed += Dash;
        }

        private void OnDisable()
        {
            move.Disable();
            dash.Disable();
        }
        #endregion

        #region Timer
        void Timer()
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        #endregion
        
        void JostickDir()
        {
            Vector2 inputDir = move.ReadValue<Vector2>();
            movementDir = new Vector2(inputDir.x, inputDir.y);
        }
}



    





