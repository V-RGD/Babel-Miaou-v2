using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerAttacks : MonoBehaviour
{
    public static PlayerAttacks instance;
    
    public float _comboCooldown = 1f; //max time allowed to combo
    public float _comboTimer;
    private float _attackMultiplier = 1;
    private bool _isMouseHolding;
    private bool _rightMouseHolding;
    public float smashGauge;
    [HideInInspector] public bool isAttacking;
    private int _comboCounter;
    private Vector3 _attackDir;
    private InputAction _mouseHold;
    private InputAction _rightClick;
    private PlayerControls _playerControls;

    #region Attack Values

    [Header("Attacks")] 
    public float attackStat = 1;
    public float spinDamageMultiplier = 1.5f;
    public float smashDamageMultiplier = 5;
    public float dexterity;

    public float slashCooldown = 0.4f;
    public float spinCooldown = 0.7f;
    public float smashCooldown = 2;
    public float smashWarmup = 1;

    public float smashForce = 10;
    public float slashForce = 15;
    public float spinForce = 8;

    private GameObject _attackAnchor;
    private GameObject _smashHitBox;
    private GameObject _slashHitBox;
    private GameObject _spinHitBox;
    public GameObject poisonCloud;

    #endregion
    
    private Animator _animator;
    private Rigidbody _rb;
    private PlayerController _pc;
    public AttackParameters attackParameters;

    private static readonly int Idle = Animator.StringToHash("Idle");
    
    private static readonly int Attack_Side = Animator.StringToHash("Attack_Side");
    private static readonly int Attack_Back = Animator.StringToHash("Attack_Back");
    private static readonly int Attack_Front = Animator.StringToHash("Attack_Front");
    private static readonly int SecondAttack_Side = Animator.StringToHash("SecondAttack_Side");
    private static readonly int SecondAttack_Back = Animator.StringToHash("SecondAttack_Back");
    private static readonly int SecondAttack_Front = Animator.StringToHash("SecondAttack_Front");
    private static readonly int Spin_Attack = Animator.StringToHash("Spin_Attack");
    private static readonly int Smash_Attack = Animator.StringToHash("Smash_Attack");

    public VisualEffect normalSlashFX;
    public VisualEffect reverseSlashFX;
    public VisualEffect spinSlashFX;
    public VisualEffect smashSlashFX;

    public bool canInterruptAnimation;
    public enum AttackState
    {
        Default,
        Startup,
        Active,
        Recovery
    }
    public enum ComboState
    {
        Default,
        SimpleAttack,
        ReverseAttack,
        SpinAttack,
        SmashAttack
    }
    public ComboState comboState;
    public AttackState currentAttackState;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        instance = this;
        
        _attackAnchor = transform.GetChild(0).gameObject;
        _slashHitBox = _attackAnchor.transform.GetChild(0).gameObject;
        _spinHitBox = _attackAnchor.transform.GetChild(1).gameObject;
        _smashHitBox = _attackAnchor.transform.GetChild(2).gameObject;
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _playerControls = new PlayerControls();
    }

    private void Start() => _pc = PlayerController.instance;

    private void Update()
    {
        if (currentAttackState != AttackState.Active && currentAttackState != AttackState.Startup)
        {
            Timer();
            RightClickAttackManagement();
        }
    }

    void PlayAnimation(Vector3 playerDirection)
    {
        int state = GetAttackAnimation(playerDirection);
        if (state == _pc.currentAnimatorState) return;
        _animator.CrossFade(state, 0, 0);
        _pc.currentAnimatorState = state;    
    }
    void SetAttackState(AttackState state)
    {
        currentAttackState = state;
    }
    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        canInterruptAnimation = false;
        _pc.SwitchState(PlayerController.PlayerStates.Attack);;
        
        //values assignation
        GameObject hitbox = null;
        Vector3 attackDir = Vector3.zero;
        float damage = 0;
        float cooldown = 0;
        float force = 0;
        float startUpLength = 0;
        float activeLength = 0;
        float recoverLength = 0;
        
        //-----------startup state
        //defines values, blocks walking
        SetAttackState(AttackState.Startup);
        _pc.SwitchState(PlayerController.PlayerStates.Attack);
        
        //starts timer for combos
        _comboTimer = recoverLength + 0.2f;
        
        if (_pc.movementDir != Vector2.zero)
        {
            attackDir = new Vector3(_pc.movementDir.x, 0, _pc.movementDir.y);
        }
        else
        {
            attackDir = new Vector3(_pc.lastWalkedDir.x, 0, _pc.lastWalkedDir.y);
            force = 0;
        }

        if (comboState == ComboState.SpinAttack)
        {
            comboState = ComboState.Default;
        }
        
        switch (comboState)
        {
            case ComboState.Default : 
                cooldown = slashCooldown * dexterity; 
                damage = Mathf.FloorToInt(attackStat);
                hitbox = _slashHitBox; 
                force = slashForce;
                startUpLength = attackParameters.attackStartupLength;
                activeLength = attackParameters.attackActiveLength;
                recoverLength = attackParameters.attackRecoverLength;
                normalSlashFX.Play();
                //1st anim
                comboState = ComboState.SimpleAttack;
                PlayAnimation(attackDir);
                break;
            case ComboState.SimpleAttack : 
                cooldown = slashCooldown * dexterity; 
                damage = Mathf.FloorToInt(attackStat);
                hitbox = _slashHitBox; 
                force = slashForce;
                startUpLength = attackParameters.attackStartupLength;
                activeLength = attackParameters.attackActiveLength;
                recoverLength = attackParameters.attackRecoverLength;
                //2nd anim
                reverseSlashFX.Play();
                comboState = ComboState.ReverseAttack;
                PlayAnimation(attackDir);
                break;
            case ComboState.ReverseAttack : 
                cooldown = spinCooldown * dexterity; 
                damage = attackStat * spinDamageMultiplier/5;
                hitbox = _spinHitBox; 
                force = spinForce;
                startUpLength = attackParameters.spinStartupLength;
                activeLength = attackParameters.spinActiveLength;
                recoverLength = attackParameters.spinRecoverLength;
                //3rd anim
                StartCoroutine(SpinSlashes());                
                comboState = ComboState.SpinAttack;
                PlayAnimation(attackDir);
                break;
        }
        
        //determine ou l'attaque va se faire
        _attackAnchor.transform.LookAt(transform.position + attackDir);
        //stops movement
        _pc.canMove = false;
        //add current damage stat to weapon
        hitbox.GetComponent<ObjectDamage>().damage = Mathf.CeilToInt(damage);
        //adds force to simulate inertia
        _rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(startUpLength);

        //-----------active state
        //can touch enemies, hitbox active, is invincible
        SetAttackState(AttackState.Active);
        hitbox.SetActive(true);
        _pc.invincibleCounter = activeLength;

        if (_pc.movementDir != Vector2.zero)
        {
            _rb.AddForce(attackDir * force, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(activeLength);

        //------------recovery state
        SetAttackState(AttackState.Recovery);
        //hitbox inactive, not invincible
        hitbox.SetActive(false);
        yield return new WaitForSeconds(recoverLength);

        //-----------can attack again
        SetAttackState(AttackState.Default);
        //comboState = ComboState.Default;
        //can walk again
        _pc.SwitchState(PlayerController.PlayerStates.Run);
        //waits cooldown depending on the attack used
        _rb.velocity = Vector3.zero;
        //restores speed
        _pc.canMove = true;
        isAttacking = false;
    }
    
    IEnumerator SmashCoroutine()
    {
        isAttacking = true;
        canInterruptAnimation = false;
        _pc.SwitchState(PlayerController.PlayerStates.Attack);;
        
        //values assignation
        GameObject hitbox = null;
        Vector3 attackDir = Vector3.zero;
        int damage = 0;
        float cooldown = 0;
        float force = 0;
        float startUpLength = 0;
        float activeLength = 0;
        float recoverLength = 0;
        
        //-----------startup state
        //defines values, blocks walking
        SetAttackState(AttackState.Startup);
        _pc.SwitchState(PlayerController.PlayerStates.Attack);
        
        if (_pc.movementDir != Vector2.zero)
        {
            attackDir = new Vector3(_pc.movementDir.x, 0, _pc.movementDir.y);
        }
        else
        {
            attackDir = new Vector3(_pc.lastWalkedDir.x, 0, _pc.lastWalkedDir.y);
            force = 0;
        }
        
        cooldown = smashCooldown; 
        damage = Mathf.CeilToInt(attackStat * smashDamageMultiplier);
        hitbox = _smashHitBox; 
        force = smashForce;
        startUpLength = attackParameters.smashStartupLength;
        activeLength = attackParameters.smashActiveLength;
        recoverLength = attackParameters.smashRecoverLength;
        //3rd anim
        comboState = ComboState.SimpleAttack;
        PlayAnimation(attackDir);
        comboState = ComboState.Default;
        
        //determine ou l'attaque va se faire
        _attackAnchor.transform.LookAt(transform.position + attackDir);
        //stops movement
        _pc.canMove = false;
        //add current damage stat to weapon
        hitbox.GetComponent<ObjectDamage>().damage = Mathf.CeilToInt(damage);
        //adds force to simulate inertia
        _rb.velocity = Vector3.zero;
        _rb.AddForce(attackDir * force, ForceMode.Impulse);
        yield return new WaitForSeconds(startUpLength);

        //-----------active state
        //can touch enemies, hitbox active, is invincible
        _rb.velocity = Vector3.zero;
        SetAttackState(AttackState.Active);
        hitbox.SetActive(true);

        if (ObjectsManager.instance.stinkyFish)
        {
            Instantiate(poisonCloud, transform.position, Quaternion.identity);
        }
        
        _pc.invincibleCounter = activeLength;
        // _rb.AddForce(attackDir * force, ForceMode.Impulse);
        yield return new WaitForSeconds(activeLength);

        //------------recovery state
        SetAttackState(AttackState.Recovery);
        //hitbox inactive, not invincible
        hitbox.SetActive(false);
        yield return new WaitForSeconds(recoverLength);

        //-----------can attack again
        SetAttackState(AttackState.Default);
        //comboState = ComboState.Default;
        //can walk again
        _pc.SwitchState(PlayerController.PlayerStates.Run);
        //waits cooldown depending on the attack used
        _rb.velocity = Vector3.zero;
        //restores speed
        _pc.canMove = true;
        isAttacking = false;
    }

    IEnumerator SpinSlashes()
    {
        float interval = attackParameters.spinActiveLength/4;
        spinSlashFX.Play();
        _spinHitBox.SetActive(true);
        yield return new WaitForSeconds(interval);
        _spinHitBox.SetActive(false);
        _spinHitBox.SetActive(true);
        spinSlashFX.Stop();
        spinSlashFX.Play();
        yield return new WaitForSeconds(interval);
        _spinHitBox.SetActive(false);
        _spinHitBox.SetActive(true);
        spinSlashFX.Stop();
        spinSlashFX.Play();
        yield return new WaitForSeconds(interval);
        _spinHitBox.SetActive(false);
        _spinHitBox.SetActive(true);
        spinSlashFX.Stop();
        spinSlashFX.Play();
        yield return new WaitForSeconds(interval);
        _spinHitBox.SetActive(false);
        _spinHitBox.SetActive(true);
        spinSlashFX.Stop();
        spinSlashFX.Play();
    }

    void RightMouseHold(InputAction.CallbackContext context)
    {
        if (currentAttackState == AttackState.Default)
        {
            _rightMouseHolding = true;
        }
    }
    void RightMouseReleased(InputAction.CallbackContext context)
    {
        _rightMouseHolding = false;
    }
    #region Timer

    private void Timer()
        {
            _comboTimer -= Time.deltaTime;

            if (_comboTimer <= 0)
            {
                _comboCounter = 0;
                comboState = ComboState.Default;
            }
        }
    #endregion
    private void NormalAttackManagement(InputAction.CallbackContext context)
    {
        if (currentAttackState != AttackState.Active && currentAttackState != AttackState.Startup)
        {
            _pc.canMove = true;
            StopAllCoroutines();
            StartCoroutine(AttackCoroutine());
        }
    }
    
    private void RightClickAttackManagement()
    {
        if (_rightMouseHolding)
        {
            _pc.currentState = PlayerController.PlayerStates.Attack;
            _rb.velocity = Vector3.zero;
            //charges smash gauge
            smashGauge += Time.deltaTime;
        }
        else
        {
            //if initial input
            if (smashGauge > 0)
            {
                
                if (smashGauge >= 1)
                {
                    StartCoroutine(SmashCoroutine());
                }
                else
                {
                    _pc.canMove = true;
                }
            }
            smashGauge = 0;
        }

        if (smashGauge >= 1)
        {
            smashGauge = 1;
        }
    }

    int GetAttackAnimation(Vector3 playerDir)
    {
        //Debug.Log("attack anim" + comboState);
        var state = Idle;
        //checks player speed for orientation
        var xVal = playerDir.x >= 0 ? playerDir.x : -playerDir.x;
        var yVal = playerDir.z >= 0 ? playerDir.z : -playerDir.z;
        //if is attacking, animation plays, then locks current state during x seconds
        //checks the best option depending on the speed
        if (yVal >= xVal)
        {
            //if y value is dominant, plays up or down anims
            state = comboState switch
            {
                ComboState.SimpleAttack => playerDir.z >= 0 ? Attack_Back : Attack_Front,
                ComboState.ReverseAttack => playerDir.z >= 0 ? SecondAttack_Back : SecondAttack_Front,
                ComboState.SpinAttack => Spin_Attack,
                _ => state
            };
        }
        else
        {
            //else, plays side
            state = comboState switch
            {
                ComboState.SimpleAttack => Attack_Side,
                ComboState.ReverseAttack => SecondAttack_Side,
                ComboState.SpinAttack => Spin_Attack,
                _ => state
            };
        }
        return state;
    }
    
    int GetAttackAnimationNoDir()
    {
        //Debug.Log("attack anim" + comboState);
        int state = comboState switch
        {
            ComboState.SimpleAttack => Attack_Side,
            ComboState.ReverseAttack => SecondAttack_Side,
            ComboState.SpinAttack => Spin_Attack,
            _ => Idle
        };
        return state;
    }

    #region InputSystemRequirements
    private void OnEnable()
    {
        _mouseHold = _playerControls.Player.AttackHold;
        _mouseHold.performed += NormalAttackManagement;
        _mouseHold.Enable();
        
        _rightClick = _playerControls.Player.RightClick;
        _rightClick.started += RightMouseHold;
        _rightClick.canceled += RightMouseReleased;
        _rightClick.Enable();
    }
    private void OnDisable()
    {
        _mouseHold.Disable();
        _rightClick.Disable();
    }
    #endregion

}
