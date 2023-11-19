using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : GenericSingletonClass<PlayerController>
{
    //[Header("Parameters")] 
    [SerializeField] float speed = 7;
    [SerializeField] float acceleration = 7;
    [SerializeField] float dashForce = 60;
    [SerializeField] float dashLength = 0.1f;
    [SerializeField] float dashCooldown = 0.3f;
    [SerializeField] InputActionReference moveInput;
    
    Action _onDash;
    Vector2 _moveDir;
    Vector2 _lastMoveDir;
    float _dashCooldownTimer; 
    public Rigidbody rb { get; private set; }
    InputAction _moveInputAction;
    
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        _moveInputAction = moveInput.action;
        _moveInputAction.Enable();
    }

    void OnDisable() => _moveInputAction.Disable();

    void Update()
    {
        RegisterMoveInput();
        Timers();
    }

    void FixedUpdate()
    {
        //enables movement if correct state active
        if (PlayerStateMachine.Instance.currentState is not PlayerStateMachine.PlayerState.Running) return;
        if (_moveDir == Vector2.zero) Friction();
        else Move();
    }

    /// <summary>
    /// Moves Player along the X and Z axis
    /// </summary>
    void Move()
    {
        Vector3 convertedDir = new Vector3(_moveDir.x, 0, _moveDir.y);
        rb.AddForce(convertedDir * acceleration, ForceMode.Impulse);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);
    }

    void Friction() => rb.velocity = Vector3.zero;

    void RegisterMoveInput() => _moveDir = _moveInputAction.ReadValue<Vector2>();

    public void OnDashInput()
    {
        if (_dashCooldownTimer > 0) return;
        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        //activates related events
        _onDash?.Invoke();
        //disables movement
        PlayerStateMachine.Instance.SwitchState(PlayerStateMachine.PlayerState.Dashing);
        //if is attacking, interrupts attack

        //resets speed
        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        //determines the speed vector to apply
        Vector3 dashDir = new Vector3(_moveDir.x, 0, _moveDir.y);
        //if no directional input registered, uses last move direction and slightly boosts it
        if (_moveDir == Vector2.zero)
        {
            dashDir = new Vector3(_lastMoveDir.x, 0, _lastMoveDir.y) * 1.15f;
        }

        //then applies velocity
        rb.AddForce(dashDir * dashForce, ForceMode.Impulse);

        //waits set duration
        yield return new WaitForSeconds(dashLength);

        //re-enables movement
        PlayerStateMachine.Instance.SwitchState(PlayerStateMachine.PlayerState.Running);

        //resets velocity and updates cooldown
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        _dashCooldownTimer = dashCooldown;
    }

    void Timers()
    {
        _dashCooldownTimer -= Time.deltaTime;
    }
}