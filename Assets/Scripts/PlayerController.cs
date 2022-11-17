using System.Collections;
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

    #region Intern Values
    private float _frictionMultiplier = 1;
    private float _speedFactor = 1;
    private int _dashesAvailable = 1;
    private int _dirCoef; //sprite direction
    [HideInInspector] public float invincibleCounter;
    [HideInInspector] public float invincibleTime;
    [HideInInspector] public float dashCooldownTimer;
    [HideInInspector] public float stunCounter;
    [HideInInspector] public Vector3 lastWalkedDir;
    [HideInInspector] public Vector2 movementDir;
    #endregion

    #region Components
    private Rigidbody _rb;
    private SpriteRenderer _spriteRenderer;
    private PlayerControls _playerControls;
    private InputAction _move;
    private InputAction _dash;
    private Animator _animator;
    private float _lockedTill;
    [HideInInspector] public PlayerAttacks _playerAttacks;
    [HideInInspector] public int currentAnimatorState;
    
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Dash = Animator.StringToHash("Dash");
    private static readonly int Run_Side = Animator.StringToHash("Run_Side");
    private static readonly int Run_Back = Animator.StringToHash("Run_Back");
    private static readonly int Run_Front = Animator.StringToHash("Run_Front");

    #endregion

    public PlayerStates currentState;
    void Behaviour(PlayerStates state)
    {
        switch (state)
        {
            case PlayerStates.Run: 
                MovePlayer();
                MovingAnimations();
                Flip();
                break;
            case PlayerStates.Attack:
                break;
            case PlayerStates.Dash:
                break;
        }
    }
    void MovingAnimations()
    {
        var state = GetMovingAnimation();
        if (state == currentAnimatorState) return;
        _animator.CrossFade(state, 0, 0);
        currentAnimatorState = state;
    }
    public void SwitchState(PlayerStates nextState)
    {
        currentState = nextState;
    }
    void Timer()
    {
        dashCooldownTimer -= Time.deltaTime;
        invincibleCounter -= Time.deltaTime;
        stunCounter -= Time.deltaTime;
    }
    public enum PlayerStates
    {
        Run, 
        Attack,
        Dash,
    }
    private void Awake()
    {
        canMove = true;
        
        _rb = GetComponent<Rigidbody>();
        _spriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        _playerControls = new PlayerControls();
        
        _animator = GetComponent<Animator>();
        _playerAttacks = GetComponent<PlayerAttacks>();
        currentState = PlayerStates.Run;
    }
    void FixedUpdate()
    {
        Behaviour(currentState);
        JostickDir();
        Timer();
        Friction();
    }
    void PlayAnimation(int state)
    {
        //if is dashing, lock current state during x seconds
        if (currentState == PlayerStates.Dash)
        {
            //state = _currentAnimatorState;
        }
        
        if (state == currentAnimatorState) return;
        _animator.CrossFade(state, 0, 0);
        currentAnimatorState = state;
    }
    private int GetMovingAnimation()
    {
        if (Time.time < _lockedTill) return currentAnimatorState;
        //checks player speed for orientation
        float xVal = movementDir.x >= 0 ? movementDir.x : -movementDir.x;
        float yVal = movementDir.y >= 0 ? movementDir.y : -movementDir.y;
        
        //if running
        if (movementDir != Vector2.zero)
        {
            //checks the best option depending on the speed
            if (yVal > xVal && movementDir.y != 0)
            {
                //plays back and forward anims instead of side
                return movementDir.y >= 0 ? Run_Back : Run_Front;
            }
            else
            {
                //plays side anim
                return Run_Side;
            }
        }
        else
        {
            //if the player isn't moving, simply plays idle anim
            return Idle;
        }

        int LockState(int s, float t)
        {
            _lockedTill = Time.time + t;
            return s;
        }
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
        if (movementDir!= Vector2.zero && _speedFactor != 0)
        {
            _rb.AddForce(new Vector3(movementDir.x * acceleration, 0, movementDir.y * acceleration), ForceMode.Impulse);
            lastWalkedDir = movementDir;
            
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
        if (currentState == PlayerStates.Attack || stunCounter > 0 || currentState == PlayerStates.Dash)
        {
            _frictionMultiplier = 0.95f;
            return;
        }
        
        if (currentState != PlayerStates.Dash && currentState != PlayerStates.Attack && movementDir == Vector2.zero)
        {
            _frictionMultiplier = 1/frictionAmount;
        }

        _rb.velocity = new Vector3(_rb.velocity.x * _frictionMultiplier, _rb.velocity.y, _rb.velocity.z * _frictionMultiplier);
    }

    void InputDash(InputAction.CallbackContext context)
    {
        if (currentState == PlayerStates.Run && _dashesAvailable > 0 && dashCooldownTimer <= 0)
        {
            _dashesAvailable--;
            StartCoroutine(DashSequence());
        }
    }

    IEnumerator DashSequence()
    {
        SwitchState(PlayerStates.Dash);
        _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        canMove = false;
        //applies dash force
        if (movementDir != Vector2.zero)
        {
            _rb.AddForce(new Vector3(dashForce * movementDir.x, 0, dashForce * movementDir.y), ForceMode.Impulse);
        }
        else
        {
            _rb.AddForce(new Vector3(dashForce * lastWalkedDir.x, 0, dashForce * lastWalkedDir.y), ForceMode.Impulse);
        }
        yield return new WaitForSeconds(dashLenght);
        _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        canMove = true;
        _dashesAvailable++;
        dashCooldownTimer = dashCooldownLenght;
        SwitchState(PlayerStates.Run);
    }
    #endregion
    void JostickDir()
        {
            Vector2 inputDir = _move.ReadValue<Vector2>();
            movementDir = new Vector2(inputDir.x, inputDir.y);
        }
    
    #region InputSystemRequirements
    private void OnEnable()
        {
            _move = _playerControls.Player.Move;
            _move.Enable();

            _dash = _playerControls.Player.Dash;
            _dash.Enable();
            _dash.performed += InputDash;
        }
    private void OnDisable()
    {
        _move.Disable();
        _dash.Disable();
    }
    #endregion
    /*
    IEnumerator Smash()
        {
            SwitchState(PlayerStates.Attack);
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
            //_rb.AddForce(pushedDir * force, ForceMode.Impulse);
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
            SwitchState(PlayerStates.Run);
        }*/
}



    





