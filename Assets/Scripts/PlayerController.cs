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

    [Header("Movement")] 
    public float acceleration;
    public bool canMove;
    public float dashForce;
    public float frictionAmount;
    private bool isDashing;
    public float dashLenght;

    public int dashAvailable;
    public float dashCooldownLenght;
    public float dashCooldownTimer;

    [Header("Components")] 
    private Rigidbody rb;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    [Header("Extra Values")] [HideInInspector]
    //public int groundDetectionLayerMask;

    [Header("Attacks")] public float jabLenght;
    public float smashLenght;
    //time 
    public float smashWarmup;
    public float smashDamage;
    
    public float jabDamage;
    private float jabReach;
    public float comboCooldown;
    //max time allowed to combo
    private float comboTimer;
    private int comboCounter;
    
    public bool isAttacking;

    public GameObject SmashHitBox;
    public GameObject JabHitBox;

    private float attackMultiplier = 1;

    public float invincibleCounter;
    public float invincibleTime;
    private bool isInvincible;

    //used to change direction accordingly to sprite direction
    public int dirCoef;

    public GameObject attackAnchor;
    private Vector3 attackDir;
    private Vector3 lastWalkedDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        canMove = true;
        isAttacking = false;
    }

    void Update()
    {
        //Flip(rb.velocity.x);
        JabHitBox.GetComponent<ObjectDamage>().damage = attackMultiplier * jabDamage;
        SmashHitBox.GetComponent<ObjectDamage>().damage = attackMultiplier * smashDamage;
    }

    void FixedUpdate()
    {
        MovePlayer();

        if (canMove)
        {
            JostickDir();
        }
        Timer();
    }

    #region Basic Movement

    void MovePlayer()
    {
        if (movementDir!= Vector2.zero && !isDashing && speedFactor != 0)
        {
            rb.AddForce(new Vector3(movementDir.x * acceleration, 0, movementDir.y * acceleration));
            lastWalkedDir = movementDir;
            
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
            rb.AddForce(new Vector3(dashForce * lastWalkedDir.x, 0, dashForce * lastWalkedDir.y), ForceMode.Impulse);
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
                StartCoroutine(JabCooldown());

                /*
                if (comboCounter < 3)
                {
                    //Jab
                }
                else
                {
                    //smash
                    StartCoroutine(SmashCooldown());
                }*/
            }
        }

        IEnumerator JabCooldown()
        {
            //avant l'attaque
            attackAnchor.transform.LookAt(transform.position + new Vector3(movementDir.x, 0, movementDir.y));
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

        public IEnumerator MoveTowardsPoint(Vector3 destination)
        {
            //movementDir = destination;
            yield return new WaitForSeconds(1);
        }
}



    





