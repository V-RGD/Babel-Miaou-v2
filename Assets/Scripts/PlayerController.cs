using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    #region Movement Values
    [Header("Movement")]
    public float maxSpeed;
    public float attackSpeedFactor;
    public float acceleration;
    public float dashForce;
    public float frictionAmount;
    public float dashLenght;
    public float dashCooldownLenght;
    public bool canMove = true;
    [HideInInspector] public bool isDashing;
    public bool isDashingOverHole;
    
    #endregion
    #region Intern Values
    private float _frictionMultiplier = 1;
    private float _speedFactor = 1;
    private int _dashesAvailable = 1;
    private int _dirCoef; //sprite direction
    public float invincibleCounter;
    [HideInInspector] public float invincibleTime;
    [HideInInspector] public float dashCooldownTimer;
    [HideInInspector] public float stunCounter;
    [HideInInspector] public Vector3 lastWalkedDir;
    [HideInInspector] public Vector2 movementDir;
    public Vector3 dashOverDir;
    public PlayerStates currentState;
    #endregion
    #region Components
    private Rigidbody _rb;
    private RandSoundGen _stepSounds;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    private PlayerControls _playerControls;
    private InputAction _move;
    private InputAction _dash;
    private Animator _animator;
    private PlayerRemnants _remnants;
    private float _lockedTill;
    [HideInInspector] public int currentAnimatorState;
    private BoxCollider _boxCollider;
    
    private static readonly int IdleLick = Animator.StringToHash("Idle_Lick");
    private static readonly int IdleHeadFlip = Animator.StringToHash("Idle_HeadFlip");
    private static readonly int IdleBreath = Animator.StringToHash("Idle_Breath");
    private static readonly int Dash = Animator.StringToHash("Dash");
    private static readonly int Run_Up = Animator.StringToHash("Run_Up");
    private static readonly int Run_Down = Animator.StringToHash("Run_Down");
    private static readonly int Run_Left = Animator.StringToHash("Run_Left");
    private static readonly int Run_Right = Animator.StringToHash("Run_Right");
    private static readonly int Run_DiagonalUp = Animator.StringToHash("Run_DiagonalUp");
    private static readonly int Run_DiagonalDown = Animator.StringToHash("Run_DiagonalDown");
    public bool isIdle = false;
    public float idleTimer;
    public ParticleSystem dashTrail;
    #endregion
    public enum PlayerStates
    {
        Run, 
        Attack,
        Dash,
        Hurt
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        
        canMove = true;
        
        _rb = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        _stepSounds = GetComponent<RandSoundGen>();
        spriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        _playerControls = new PlayerControls();
        
        _animator = GetComponent<Animator>();
        _remnants = GetComponent<PlayerRemnants>();
        currentState = PlayerStates.Run;
        lastWalkedDir = Vector3.right;
        dashTrail.Stop();
    }
    void Behaviour(PlayerStates state)
    {
        switch (state)
        {
            case PlayerStates.Run:
                _speedFactor = 1;
                if (canMove)
                {
                    MovePlayer();
                }
                MovingAnimations();
                break;
            case PlayerStates.Attack:
                MovePlayer();
                _speedFactor = attackSpeedFactor;
                break;
        }
    }
    void FixedUpdate()
    {
        if (PlayerAttacks.instance.currentAttackState == PlayerAttacks.AttackState.Default)
        {
            Behaviour(currentState);
        }
        JoystickDir();
        Timer();
        Friction();
        
        if (isDashing)
        {
            HoleDashCheck();
        }

        if (isDashingOverHole)
        {
            HoleDashForce();
        }
    }

    void HoleDashCheck()
    {
        Vector3 dashDir = new Vector3(movementDir.x, 0,  movementDir.y);
        //if finds wall and ground on the other side
        if (movementDir == Vector2.zero)
        {
            dashDir = new Vector3(lastWalkedDir.x, 0,  lastWalkedDir.y);
        }
            
        //checks for a hole
        float characterSize = 2.4f;
        if (Physics.Raycast(transform.position, dashDir, out var wallHit, 6, LayerMask.GetMask("Wall")) && 
            Physics.Raycast(transform.position + Vector3.down * characterSize + dashDir.normalized, dashDir,
                out var groundHit, 20, LayerMask.GetMask("Ground", "Pont")))
        {
            StopCoroutine(DashSequence());
            StartCoroutine(DashOverHole(dashDir, groundHit.point));
        }
    }
    void HoleDashForce()
    {
        if (_rb.velocity.magnitude < 70)
        {
            Vector3 dir = -(dashDestination + transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(dashDestination.x, transform.position.y, dashDestination.z), 1f);
            // _rb.AddForce(new Vector3(dir.x, 0, dir.y) * 50, ForceMode.VelocityChange);
        }
        else
        {
            _rb.velocity = _rb.velocity.normalized * 70;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(dashDestination.x, transform.position.y, dashDestination.z), 1f);
        }
    }

    #region Basic Movement
    void MovePlayer()
    {
        if (movementDir!= Vector2.zero && _speedFactor != 0)
        {
            _rb.AddForce(new Vector3(movementDir.x * acceleration * _speedFactor, 0, movementDir.y * acceleration * _speedFactor), ForceMode.Impulse);
            lastWalkedDir = movementDir;
            
            //cap x speed
            if(_rb.velocity.x > maxSpeed * _speedFactor)
            {
                _rb.velocity = new Vector3(maxSpeed, _rb.velocity.y, _rb.velocity.z);
            }
            if(_rb.velocity.x < -maxSpeed * _speedFactor)
            {
                _rb.velocity = new Vector3(-maxSpeed, _rb.velocity.y, _rb.velocity.z);
            }
            
            //cap x speed
            if(_rb.velocity.z > maxSpeed * _speedFactor)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, maxSpeed);
            }
            if(_rb.velocity.z < -maxSpeed * _speedFactor)
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
        
        if (currentState == PlayerStates.Run &&  movementDir == Vector2.zero)
        {
            _rb.velocity = Vector3.zero;
        }
    }
    void InputDash(InputAction.CallbackContext context)
    {
        if ((currentState == PlayerStates.Run || PlayerAttacks.instance.currentAttackState == PlayerAttacks.AttackState.Recovery) && _dashesAvailable > 0 && dashCooldownTimer <= 0)
        {
            _dashesAvailable--;
            StartCoroutine(DashSequence());
        }
        else if (ObjectsManager.instance.killingSpreeTimer > 0) //uses killing spree timer
        {
            ObjectsManager.instance.killingSpreeTimer = 0;
            StartCoroutine(DashSequence());
        }
    }
    IEnumerator DashSequence()
    {
        PlayerAttacks.instance.InterruptAttack();
        SwitchState(PlayerStates.Dash);
        isDashing = true;
        //plays fx and sfx
        PlayerAttacks.instance.burnVfxPulling.PlaceDashFx();
        dashTrail.Play();
        PlayerSounds.instance.dashSource.PlayOneShot(PlayerSounds.instance.dashSource.clip);
        _remnants.StartCoroutine(_remnants.DashRemnants());
        _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        canMove = false;
        
        Vector3 dashDir = new Vector3(movementDir.x, 0,  movementDir.y);
        //if finds wall and ground on the other side
        if (movementDir == Vector2.zero)
        {
            dashDir = new Vector3(lastWalkedDir.x, 0,  lastWalkedDir.y);
        }
        
        //else
        //{
            //if normal dash (doesn't find a wall AND there isn't anywhere to dash to
            //applies dash force
            if (movementDir != Vector2.zero)
            {
                _rb.AddForce(dashDir * dashForce, ForceMode.Impulse);
            }
            else
            {
                _rb.AddForce(new Vector3(dashForce * lastWalkedDir.x, 0, dashForce * lastWalkedDir.y) * 1.15f, ForceMode.Impulse);
            }
            //stop dash
            yield return new WaitForSeconds(dashLenght);
        //}
        dashTrail.Stop();
        _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        canMove = true;
        isDashing = false;
        _dashesAvailable++;
        dashCooldownTimer = dashCooldownLenght;

        if (invincibleCounter < dashLenght + 0.25f)
        {
            invincibleCounter = dashLenght + 0.25f;
        }

        if (!isDashingOverHole)
        {
            SwitchState(PlayerStates.Run);
        }
    }

    public float dashCheatCoef;
    public Vector3 dashDestination;
    IEnumerator DashOverHole(Vector3 dashDir, Vector3 groundHit)
    {
        isDashingOverHole = true;
        Debug.Log("tried to ashed over");
        PlayerAttacks.instance.InterruptAttack();
        SwitchState(PlayerStates.Dash);
        isDashing = true;
        //plays fx and sfx
        PlayerSounds.instance.dashSource.PlayOneShot(PlayerSounds.instance.dashSource.clip);
        _remnants.StartCoroutine(_remnants.DashRemnants());
        canMove = false;

        Vector3 originalPos = transform.position;
        Vector3 groundPos = groundHit;
        Vector3 destination = groundPos + dashDir.normalized * 2;
        dashOverDir = dashDir;
        dashDestination = destination;
            
        //starts dash while disabling collider
        _boxCollider.enabled = false;
        _rb.AddForce(dashForce * dashDir, ForceMode.Impulse);
        //waits until reached ground
        //StartCoroutine(DashSecurity(destination, destination - originalPos));
        
        yield return new WaitUntil(() => (destination - transform.position).magnitude <= 3f);
        
        dashOverDir = Vector3.zero;
        isDashingOverHole = false;
        Debug.Log("found ground");
        _boxCollider.enabled = true;
        
        dashTrail.Stop();
        _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        canMove = true;
        isDashing = false;
        _dashesAvailable++;
        dashCooldownTimer = dashCooldownLenght;

        if (invincibleCounter < dashLenght + 0.25f)
        {
            invincibleCounter = dashLenght + 0.25f;
        }
        SwitchState(PlayerStates.Run);
        Debug.Log("dashed over hole");
    }

    IEnumerator DashSecurity(Vector3 securePoint, Vector3 gap)
    {
        yield return new WaitForSeconds(gap.magnitude * dashCheatCoef);
        if (isDashingOverHole)
        {
            Debug.Log("dashSecurity");
            transform.position = new Vector3(securePoint.x, transform.position.y, securePoint.z);
        }
    }
    #endregion
    void JoystickDir()
    {
        Vector2 inputDir = _move.ReadValue<Vector2>();
        movementDir = new Vector2(inputDir.x, inputDir.y);
    }

    #region Animator
    void MovingAnimations()
    {
        //if moving
        if (movementDir != Vector2.zero && _rb.velocity != Vector3.zero)
        {
            var state = GetMovingAnimation();
            if (state == currentAnimatorState) return;
            _animator.CrossFade(state, 0, 0);
            currentAnimatorState = state;
        }
        else
        {
            //idle
            if (PlayerAttacks.instance.currentAttackState != PlayerAttacks.AttackState.Active 
                        && PlayerAttacks.instance.currentAttackState != PlayerAttacks.AttackState.Startup 
                        && PlayerAttacks.instance.currentAttackState != PlayerAttacks.AttackState.Recovery
                        && movementDir == Vector2.zero)
            {
                if (idleTimer > 0 && currentAnimatorState != IdleBreath && !isIdle)
                {
                    //if is not already breathing, plays anim
                    _animator.CrossFade(IdleBreath, 0, 0);
                    currentAnimatorState = IdleBreath;
                    spriteRenderer.flipX = false;
                }

                if (idleTimer > 3 && currentAnimatorState == IdleBreath && !isIdle)
                {
                    //if player waits longer, plays a random animation
                    isIdle = true;
                    StartCoroutine(IdleAnimations());
                }
            }
        }
    }
    private int GetMovingAnimation()
    {
        isIdle = false;
        //StopCoroutine(IdleAnimations());
        if (Time.time < _lockedTill) return currentAnimatorState;
        //checks player speed for orientation
        float xVal = movementDir.x >= 0 ? movementDir.x : -movementDir.x;
        float yVal = movementDir.y;

        //checks the best option depending on the dir
        //up
        if (yVal > 0.9f)
        {
            spriteRenderer.flipX = false;
            return Run_Up;
        }
        //diagonal up
        if (yVal > 0.2f && yVal < 0.8f)
        {
            spriteRenderer.flipX = movementDir.x >= 0 ? true : false;
            return Run_DiagonalUp;
        }
        //side
        if (yVal > -0.2f && yVal < 0.2f)
        {
            spriteRenderer.flipX = false;
            return movementDir.x <= 0 ? Run_Left : Run_Right;
        }
        //diagonal down
        if (yVal > -0.8f && yVal < -0.2f)
        {
            spriteRenderer.flipX = false;
            spriteRenderer.flipX = movementDir.x >= 0 ? true : false;
            return Run_DiagonalDown;
        }
        //down
        else
        {
            spriteRenderer.flipX = false;
            return Run_Down;
        }
    }
    public IEnumerator IdleAnimations()
    {
         spriteRenderer.flipX = false;
        //plays two breathing then a random idle
        isIdle = true;
        float variantLenght = 1;
        int randAnim = Random.Range(0, 2);
        switch (randAnim)
        {
            case 0 : _animator.CrossFade(IdleLick, 0, 0);
                currentAnimatorState = IdleLick;
                variantLenght = 0.55f;
                break;
            case 1 : _animator.CrossFade(IdleHeadFlip, 0, 0);
                currentAnimatorState = IdleHeadFlip;
                variantLenght = 1.35f;
                break;
        }
        yield return new WaitForSeconds(variantLenght);
        idleTimer = 0;
        isIdle = false;
    }
    #endregion
    public void SwitchState(PlayerStates nextState)
    {
        currentState = nextState;
    }
    void Timer()
    {
        dashCooldownTimer -= Time.deltaTime;
        invincibleCounter -= Time.deltaTime;
        stunCounter -= Time.deltaTime;
        if (_rb.velocity.magnitude < 01f)
        {
            idleTimer += Time.deltaTime;
        }
        else
        {
            idleTimer = 0;
            //StopCoroutine(IdleAnimations());
        }
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
        if (_move == null)
        {
            return;
        }
        
        _move.Disable();
        _dash.Disable();
    }
    #endregion
}



    





