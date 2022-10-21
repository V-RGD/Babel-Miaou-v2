using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Movement Values
    [Header("Movement")]
    public float maxSpeed;
    public float acceleration;
    public float dashForce;
    public float frictionAmount;
    public float dashLenght;
    public float dashCooldownLenght;
    [HideInInspector] public bool canMove = true;
    #endregion

    #region Attack Values

    [Header("Attacks")] 
    public float attackStat = 1;
    public float pickDamageMultiplier = 1.5f;
    public float smashDamageMultiplier = 5;
    public float dexterity;

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
    public GameObject masterSwordProjo;

    #endregion

    #region Intern Values
    public float _comboCooldown = 1f; //max time allowed to combo
    public float _comboTimer;
    private float _attackMultiplier = 1;
    private float _frictionMultiplier = 1;
    [HideInInspector] public float invincibleCounter;
    [HideInInspector] public float invincibleTime;
    private float _speedFactor = 1;
    [HideInInspector] public float dashCooldownTimer;
    public float smashGauge;
    private bool _isInvincible;
    [HideInInspector]public bool isAttacking;
    private bool _isDashing;
    private bool _isMouseHolding;
    [HideInInspector] public bool isMasterSword;
    [HideInInspector] public bool canRepel;
    [HideInInspector] public bool noPet;
    [HideInInspector] public float stunCounter;
    private int _dashesAvailable = 1;
    private int _comboCounter;
    private int _dirCoef; //sprite direction

    private Vector3 _attackDir;
    private Vector3 _lastWalkedDir;
    public Vector2 movementDir;
    #endregion

    #region Components
    private Rigidbody _rb;
    private SpriteRenderer _spriteRenderer;
    private PlayerControls _playerControls;
    private InputAction _move;
    private InputAction _dash;
    private InputAction _mouseHold;
    private GameManager _gameManager;
    #endregion
    private void Awake()
    {
        canMove = true;
        
        _rb = GetComponent<Rigidbody>();
        _spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        _playerControls = new PlayerControls();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        _attackAnchor = transform.GetChild(0).gameObject;
        _slashHitBox = _attackAnchor.transform.GetChild(0).gameObject;
        _pickHitBox = _attackAnchor.transform.GetChild(1).gameObject;
        _smashHitBox = _attackAnchor.transform.GetChild(2).gameObject;
    }

    void FixedUpdate()
    {
        JostickDir();
        Timer();
        Friction();

        if (stunCounter <= 0)
        {
            if (canMove)
            {
                MovePlayer();
            }
            AttackManagement();
        }
        
        Flip();
    }


    #region Flip

    void Flip()
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
        if (isAttacking || stunCounter > 0 || _isDashing)
        {
            _frictionMultiplier = 0.95f;
            return;
        }
        
        if (!_isDashing && movementDir == Vector2.zero)
        {
            if (!isAttacking)
            {
                _frictionMultiplier = 1/frictionAmount;
            }
        }

        _rb.velocity = new Vector3(_rb.velocity.x * _frictionMultiplier, _rb.velocity.y, _rb.velocity.z * _frictionMultiplier);
    }

    void Dash(InputAction.CallbackContext context)
    {
        if (!_isDashing && _dashesAvailable > 0 && dashCooldownTimer <= 0 && !isAttacking)
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
        dashCooldownTimer = dashCooldownLenght;
    }
    #endregion
    void JostickDir()
        {
            Vector2 inputDir = _move.ReadValue<Vector2>();
            movementDir = new Vector2(inputDir.x, inputDir.y);
        }

        void Attack()
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
            int damage = 0;
            float cooldown = 0;
            float force = 0;
            Vector3 attackDir = Vector3.zero;

            if (movementDir != Vector2.zero)
            {
                attackDir = new Vector3(movementDir.x, 0, movementDir.y);
            }
            else
            {
                attackDir = new Vector3(_lastWalkedDir.x, 0, _lastWalkedDir.y);
            }
            
            switch (_comboCounter)
            {
                case 0 : 
                    cooldown = slashCooldown * dexterity; 
                    damage = Mathf.CeilToInt(attackStat);
                    hitbox = _slashHitBox; 
                    force = slashForce;
                    _comboCounter++;  //used to combo if done in a row
                    //1st anim
                    break;
                case 1 : 
                    cooldown = slashCooldown * dexterity; 
                    damage = Mathf.CeilToInt(attackStat);
                    hitbox = _slashHitBox; 
                    force = slashForce;
                    _comboCounter++;  //used to combo if done in a row
                    //2nd anim
                    break;
                case 2 : 
                    cooldown = pickCooldown * dexterity; 
                    damage = Mathf.CeilToInt(attackStat * pickDamageMultiplier);
                    hitbox = _pickHitBox; 
                    force = pickForce;
                    _comboCounter = 0; //resets combo if last attack
                    break;
            }
            
            //determine ou l'attaque va se faire
            _attackAnchor.transform.LookAt(transform.position + attackDir);
            //starts timer for combos
            _comboTimer = _comboCooldown;
            //stops movement
            canMove = false;
            //add current damage stat to weapon
            hitbox.GetComponent<ObjectDamage>().damage = Mathf.CeilToInt(damage);
            //actives weapon
            hitbox.SetActive(true);
            //adds force to simulate inertia
            _rb.velocity = Vector3.zero;
            _rb.AddForce(attackDir * force, ForceMode.Impulse);
            //plays animation
            
            //if master sword, launches projectile
            if (isMasterSword && _gameManager.health == _gameManager.maxHealth)
            {
                GameObject projo = Instantiate(masterSwordProjo, transform.position, quaternion.identity);
                projo.GetComponent<Rigidbody>().AddForce(attackDir * 100);
            }
            //waits cooldown depending on the attack used
            yield return new WaitForSeconds(cooldown);
            _rb.velocity = Vector3.zero;
            //restores speed
            canMove = true;
            //disables hitbox
            hitbox.SetActive(false);
            isAttacking = false;
        }

        void MouseHold(InputAction.CallbackContext context)
        {
            _isMouseHolding = true;
        }
        void MouseReleased(InputAction.CallbackContext context)
        {
            _isMouseHolding = false;
        }
        
        #region InputSystemRequirements
        private void OnEnable()
        {
            _move = _playerControls.Player.Move;
            _move.Enable();

            _dash = _playerControls.Player.Dash;
            _dash.Enable();
            _dash.performed += Dash;

            _mouseHold = _playerControls.Player.AttackHold;
            _mouseHold.started += MouseHold;
            _mouseHold.canceled += MouseReleased;
            _mouseHold.Enable();
        }

        private void OnDisable()
        {
            _move.Disable();
            _dash.Disable();
            _mouseHold.Disable();
        }
        #endregion

        #region Timer
        void Timer()
        {
            dashCooldownTimer -= Time.deltaTime;
            _comboTimer -= Time.deltaTime;
            invincibleCounter -= Time.deltaTime;
            stunCounter -= Time.deltaTime;

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

         IEnumerator Smash()
        {
            //determines attack length, damage, hitbox, force to add
            GameObject hitbox = _smashHitBox;
            float damage = attackStat * smashDamageMultiplier;
            float cooldown = smashCooldown;
            float force = smashForce;

            //determine ou l'attaque va se faire
            _attackAnchor.transform.LookAt(transform.position + new Vector3(movementDir.x, 0, movementDir.y));
            //stops combo
            _comboCounter = 0;
            //stops movement
            canMove = false;
            //add current damage stat to weapon
            hitbox.GetComponent<ObjectDamage>().damage = Mathf.CeilToInt(damage);
            //adds force to simulate inertia
            _rb.velocity = Vector3.zero;
            Vector3 pushedDir = new Vector3(movementDir.x, 0, movementDir.y);
            _rb.AddForce(pushedDir * force, ForceMode.Impulse);
            //warmup
            yield return new WaitForSeconds(smashWarmup);
            //actives weapon
            hitbox.SetActive(true);
            //stops player
            _rb.velocity = Vector3.zero;
            //plays animation
            
            //waits cooldown depending on the attack used
            yield return new WaitForSeconds(cooldown);
            //restores speed
            canMove = true;
            //disables hitbox
            hitbox.SetActive(false);
            isAttacking = false;
        }

         void AttackManagement()
         {
             if (!isAttacking && !_isDashing)
             {
                 if (_isMouseHolding)
                 {
                     canMove = false;
                     _rb.velocity = Vector3.zero;
                     //charges smash gauge
                     smashGauge += Time.deltaTime;
                 }
                 else
                 {
                     //if initial input
                     if (smashGauge > 0)
                     {
                         //on release, if a little bit hold, attacks
                         if (smashGauge <= 0.3)
                         {
                             canMove = true;
                             Attack();
                         }
                         //if too long, smash or pass (lol)
                         else 
                         {
                             if (smashGauge == 1)
                             {
                                 isAttacking = true;
                                 StartCoroutine(Smash());
                             }
                             else
                             {
                                 canMove = true;
                             }
                         }
                     }
                     smashGauge = 0;
                 }

                 if (smashGauge >= 1)
                 {
                     smashGauge = 1;
                 }
             }
         }
}



    





