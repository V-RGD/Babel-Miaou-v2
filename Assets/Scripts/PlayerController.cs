using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed;
    private float speedFactor = 1;
    public Vector2 movementDir;

    private PlayerControls playerControls;
    public InputAction move;
    public InputAction dash;
    public InputAction attack;
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
    public GameManager gameManager;

    [Header("Extra Values")] [HideInInspector]
    //public int groundDetectionLayerMask;
    public float horizontalMovement;

    [Header("Attacks")] public float jabLenght;
    public float smashLenght;
    //time 
    public float jabCooldown;
    public float smashCooldown;
    public float smashWarmup;
    public float smashDamage;
    
    public float jabDamage;
    public float jabReach;
    public float comboCooldown;
    //max time allowed to combo
    public float comboTimer;
    public int comboCounter;
    public bool isAttacking;

    public GameObject SmashHitBox;
    public GameObject JabHitBox;

    public float attackMultiplier;

    public float invincibleCounter;
    public float invincibleTime;
    public bool isInvincible;

    //used to change direction accordingly to sprite direction
    public int dirCoef;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerControls = new PlayerControls();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        canMove = true;
        isAttacking = false;
    }

    void Update()
    {
        JostickDir();
        horizontalMovement = Mathf.Abs(rb.velocity.x);
        //Flip(rb.velocity.x);
        JabHitBox.GetComponent<ObjectDamage>().damage = attackMultiplier * jabDamage;
        SmashHitBox.GetComponent<ObjectDamage>().damage = attackMultiplier * smashDamage;
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
        if (movementDir!= Vector2.zero && !isDashing && speedFactor != 0)
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
                //rb.velocity = new Vector3(rb.velocity.x / frictionAmount, rb.velocity.y, rb.velocity.z / frictionAmount);
                rb.velocity = Vector3.zero;
            }
        }
    }

    void Dash(InputAction.CallbackContext context)
    {
        if (!isDashing && dashAvailable > 0 && dashCooldownTimer <= 0)
        {
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
            rb.AddForce(Vector3.right * dashForce * dirCoef, ForceMode.Impulse);
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
            
            attack = playerControls.Player.Attack;
            attack.Enable();
            attack.performed += Attack;
        }

        private void OnDisable()
        {
            move.Disable();
            dash.Disable();
            attack.Disable();
        }
        #endregion

        #region Timer
        void Timer()
        {
            dashCooldownTimer -= Time.deltaTime;
            comboTimer -= Time.deltaTime;
            invincibleCounter -= Time.deltaTime;

            if (comboTimer <= 0)
            {
                comboCounter = 0;
            }

            if (invincibleCounter > 0)
            {
                isInvincible = true;
            }
            else
            {
                isInvincible = false;
            }
        }
        #endregion
        
        void JostickDir()
        {
            Vector2 inputDir = move.ReadValue<Vector2>();
            movementDir = new Vector2(inputDir.x, inputDir.y);
        }

        void Attack(InputAction.CallbackContext context)
        {
            if (!isAttacking && !isDashing)
            {
                isAttacking = true;
            
                if (comboCounter < 3)
                {
                    //Jab
                    StartCoroutine(JabCooldown());
                }
                else
                {
                    //smash
                    StartCoroutine(SmashCooldown());
                }
            }
        }

        IEnumerator JabCooldown()
        {
            //avant l'attaque
            
            //resets combo and ability to combo
            comboCounter++;
            comboTimer = comboCooldown;

            //pendant l'attaque
            JabHitBox.SetActive(true);
            speedFactor = 0.1f;
            //plays animation

            yield return new WaitForSeconds(jabLenght);
            //après l'attaque
            speedFactor = 1;
            JabHitBox.SetActive(false);
            isAttacking = false;
        }
        
        IEnumerator SmashCooldown()
        {
            //avant l'attaque
            
            //resets combo and ability to combo
            comboCounter = 0;
            comboTimer = 0;
            
            //charging time
            yield return new WaitForSeconds(smashWarmup);

            //pendant l'attaque
            SmashHitBox.SetActive(true);
            speedFactor = 0;
            //plays animation

            yield return new WaitForSeconds(smashLenght);
            //après l'attaque
            speedFactor = 1;
            SmashHitBox.SetActive(false);
            isAttacking = false;
        }
}



    





