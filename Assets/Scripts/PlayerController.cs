using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed;
    public float acceleration;
    public float dashForce;
    public float frictionAmount;
    public float dashLenght;
    public float dashCooldownLenght;
    [HideInInspector] public bool canMove = true;

    [Header("Attacks")]
    public float smashDamage = 5;
    public float slashDamage = 1;
    public float pickDamage = 2;
    
    public float slashCooldown = 0.4f;
    public float pickCooldown = 0.7f;
    public float smashCooldown = 2;
    public float smashWarmup = 1;

    public float smashForce = 10;
    public float slashForce = 15;
    public float pickForce = 50;

    private GameObject _attackAnchor;
    private GameObject _smashHitBox;
    private GameObject _slashHitBox;
    private GameObject _pickHitBox;

    private float _comboCooldown = 0.7f; //max time allowed to combo
    private float _comboTimer;
    private float _attackMultiplier = 1;
    [HideInInspector] public float invincibleCounter;
    [HideInInspector] public float invincibleTime;
    private float _speedFactor = 1;
    private float _dashCooldownTimer;
    private float smashTimer;
    private int _dashesAvailable = 1;
    private bool _isInvincible;
    [HideInInspector]public bool isAttacking;
    private bool _isDashing;
    private int _comboCounter;
    private int _dirCoef; //sprite direction

    private Vector3 _attackDir;
    private Vector3 _lastWalkedDir;
    public Vector2 movementDir;

    private Rigidbody _rb;
    private SpriteRenderer _spriteRenderer;
    private PlayerControls _playerControls;
    private InputAction _move;
    private InputAction _dash;
    private InputAction _attack;
    private void Awake()
    {
        canMove = true;
        
        _rb = GetComponent<Rigidbody>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerControls = new PlayerControls();

        _attackAnchor = transform.GetChild(0).gameObject;
        _slashHitBox = _attackAnchor.transform.GetChild(0).gameObject;
        _pickHitBox = _attackAnchor.transform.GetChild(1).gameObject;
        _smashHitBox = _attackAnchor.transform.GetChild(2).gameObject;
        
        _slashHitBox.GetComponent<ObjectDamage>().damage = _attackMultiplier * slashDamage;
        _smashHitBox.GetComponent<ObjectDamage>().damage = _attackMultiplier * smashDamage;
        _pickHitBox.GetComponent<ObjectDamage>().damage = _attackMultiplier * pickDamage;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            MovePlayer();
            JostickDir();
        }
        Friction();
        Timer();
    }

    #region Basic Movement

    void MovePlayer()
    {
        if (movementDir!= Vector2.zero && !_isDashing && _speedFactor != 0)
        {
            _rb.AddForce(new Vector3(movementDir.x * acceleration, 0, movementDir.y * acceleration), ForceMode.Impulse);
            _lastWalkedDir = movementDir;
            
            //cap x speed
            if(_rb.velocity.x > maxSpeed)
            {
                _rb.velocity = new Vector3(maxSpeed, _rb.velocity.y, _rb.velocity.z);
            }
            if(_rb.velocity.x < -maxSpeed)
            {
                _rb.velocity = new Vector3(-maxSpeed, _rb.velocity.y, _rb.velocity.z);
            }
            
            //cap x speed
            if(_rb.velocity.z > maxSpeed)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, maxSpeed);
            }
            if(_rb.velocity.z < -maxSpeed)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, -maxSpeed);
            }
        }
    }

    void Friction()
    {
        //deceleration
        if (!_isDashing && movementDir == Vector2.zero)
        {
            if (!isAttacking)
            {
                Debug.Log("friction pas attack");
                _rb.velocity = new Vector3(_rb.velocity.x / frictionAmount, _rb.velocity.y, _rb.velocity.z / frictionAmount);
            }
        }

        if (isAttacking)
        {
            Debug.Log("friction attack");
            _rb.velocity = new Vector3(_rb.velocity.x * 0.95f, _rb.velocity.y, _rb.velocity.z * 0.95f);
        }
    }

    void Dash(InputAction.CallbackContext context)
    {
        if (!_isDashing && _dashesAvailable > 0 && _dashCooldownTimer <= 0 && !isAttacking)
        {
            _dashesAvailable--;
            _isDashing = true;
            StartCoroutine(DashSequence());
        }
    }

    IEnumerator DashSequence()
    {
        _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        canMove = false;
        //applies dash force
        if (movementDir != Vector2.zero)
        {
            _rb.AddForce(new Vector3(dashForce * movementDir.x, 0, dashForce * movementDir.y), ForceMode.Impulse);
        }
        else
        {
            _rb.AddForce(new Vector3(dashForce * _lastWalkedDir.x, 0, dashForce * _lastWalkedDir.y), ForceMode.Impulse);
        }
        yield return new WaitForSeconds(dashLenght);
        _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        _isDashing = false;
        canMove = true;
        _dashesAvailable++;
        _dashCooldownTimer = dashCooldownLenght;
    }
    #endregion

    #region Flip

        void Flip(float velocity)
        {
            //Flips the sprite when turning around
            if (movementDir.x > 0.1f)
            {
                _spriteRenderer.flipX = false;
            }

            else if (movementDir.x < -0.1f)
            {
                _spriteRenderer.flipX = true;
            }

            if (!_spriteRenderer.flipX)
            {
                _dirCoef = 1;
            }
            else
            {
                _dirCoef = -1;
            }
        }

        #endregion
        
    #region InputSystemRequirements
        private void OnEnable()
        {
            _move = _playerControls.Player.Move;
            _move.Enable();

            _dash = _playerControls.Player.Dash;
            _dash.Enable();
            _dash.performed += Dash;
            
            _attack = _playerControls.Player.Attack;
            _attack.Enable();
            _attack.performed += Attack;
        }

        private void OnDisable()
        {
            _move.Disable();
            _dash.Disable();
            _attack.Disable();
        }
        #endregion

    #region Timer
        void Timer()
        {
            _dashCooldownTimer -= Time.deltaTime;
            _comboTimer -= Time.deltaTime;
            invincibleCounter -= Time.deltaTime;

            if (_comboTimer <= 0)
            {
                _comboCounter = 0;
            }

            if (invincibleCounter > 0)
            {
                _isInvincible = true;
            }
            else
            {
                _isInvincible = false;
            }
        }
        #endregion
        
        void JostickDir()
        {
            Vector2 inputDir = _move.ReadValue<Vector2>();
            movementDir = new Vector2(inputDir.x, inputDir.y);
        }

        void Attack(InputAction.CallbackContext context)
        {
            if (!isAttacking && !_isDashing)
            {
                isAttacking = true;
                StartCoroutine(AttackCooldown()); 
            }
        }

        IEnumerator AttackCooldown()
        {
            //determines attack length, damage, hitbox, force to add
            GameObject hitbox = null;
            float damage = 0;
            float cooldown = 0;
            float force = 0;
            
            switch (_comboCounter)
            {
                case 0 : 
                    cooldown = slashCooldown; 
                    damage = slashDamage;
                    hitbox = _slashHitBox; 
                    force = slashForce;
                    //1st anim
                    break;
                case 1 : 
                    cooldown = slashCooldown; 
                    damage = slashDamage;
                    hitbox = _slashHitBox; 
                    force = slashForce;
                    //2nd anim
                    break;
                case 2 : 
                    cooldown = pickCooldown; 
                    damage = pickDamage;
                    hitbox = _pickHitBox; 
                    force = pickForce;
                    break;
                case 3 : 
                    cooldown = smashCooldown; 
                    damage = smashDamage;
                    hitbox = _smashHitBox; 
                    force = smashForce;
                    break;
            }
            
            //determine ou l'attaque va se faire
            _attackAnchor.transform.LookAt(transform.position + new Vector3(movementDir.x, 0, movementDir.y));
            //used to combo if done in a row
            _comboCounter++;
            //starts timer for combos
            _comboTimer = _comboCooldown;
            //stops movement
            canMove = false;
            //add current damage stat to weapon
            hitbox.GetComponent<ObjectDamage>().damage = damage;
            //actives weapon
            hitbox.SetActive(true);
            //adds force to simulate inertia
            _rb.velocity = Vector3.zero;
            Vector3 pushedDir = new Vector3(_lastWalkedDir.x, 0, _lastWalkedDir.y);
                
            _rb.AddForce(pushedDir * force, ForceMode.Impulse);
            //plays animation

            //waits cooldown depending on the attack used
            yield return new WaitForSeconds(cooldown);
            //restores speed
            canMove = true;
            //disables hitbox
            hitbox.SetActive(false);
            isAttacking = false;
        }
}



    





