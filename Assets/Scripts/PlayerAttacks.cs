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
    public float smashPower;
    private bool _isMouseHolding;
    [SerializeField]private bool _rightMouseHolding;
    public float smashGauge;
    [HideInInspector] public bool isAttacking;
    private int _comboCounter;
    private Vector3 _attackDir;
    private InputAction _mouseHold;
    private InputAction _rightClick;
    private PlayerControls _playerControls;

    #region Attack Values

    [Header("Attacks")] 
    public float bumpForce = 20;
    public float attackStat = 1;
    public float spinDamageMultiplier = 1.5f;
    public float smashDamageMultiplier = 5;
    public float dexterity;

    public float slashCooldown = 0.4f;
    public float spinCooldown = 0.7f;
    public float smashCooldown = 1;
    public float smashWarmup = 0.2f;

    public float smashForce = 15;
    public float slashForce = 15;
    public float spinForce = 8;

    public GameObject _attackAnchor;
    private GameObject _smashHitBox;
    private GameObject _slashHitBox;
    private GameObject _spinHitBox;
    public GameObject poisonCloud;

    #endregion
    
    private Animator _animator;
    private Rigidbody _rb;
    private PlayerController _pc;
    public AttackParameters attackParameters;

    #region Animations
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack_Side = Animator.StringToHash("Attack_Side");
    private static readonly int Attack_Back = Animator.StringToHash("Attack_Back");
    private static readonly int Attack_Front = Animator.StringToHash("Attack_Front");
    private static readonly int SecondAttack_Side = Animator.StringToHash("SecondAttack_Side");
    private static readonly int SecondAttack_Back = Animator.StringToHash("SecondAttack_Back");
    private static readonly int SecondAttack_Front = Animator.StringToHash("SecondAttack_Front");
    private static readonly int Spin_Attack = Animator.StringToHash("Spin_Attack");
    private static readonly int SmashPrepare = Animator.StringToHash("SmashPrepare");
    private static readonly int SmashRelease = Animator.StringToHash("SmashRelease");
    private static readonly int Smash = Animator.StringToHash("Smash");
    #endregion

    #region VFX
    public VisualEffect normalSlashFX;
    public VisualEffect reverseSlashFX;
    public VisualEffect spinSlashFX;
    public VisualEffect smashSlashFX;
    public VfxPulling vfxPulling;
    #endregion

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
        SmashAttack,
        SmashRelease,
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
        vfxPulling = GetComponent<VfxPulling>();
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
        }
        OnSmashHold();
    }
    void PlayAnimation(Vector3 playerDirection)
    {
        int state = GetAttackAnimation(playerDirection);
        if (state == _pc.currentAnimatorState) return;
        _animator.CrossFade(state, 0, 0);
        _pc.currentAnimatorState = state;    
    }
    public void SetAttackState(AttackState state)
    {
        currentAttackState = state;
    }
    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        canInterruptAnimation = false;
        if (_pc.currentState != PlayerController.PlayerStates.Dash)
        {
            _rb.velocity = Vector3.zero;
        }
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
        
        //determine ou l'attaque va se faire
        _attackAnchor.transform.LookAt(transform.position + attackDir);
        vfxPulling.attackDir = attackDir;
        
        switch (comboState)
        {
            case ComboState.Default : 
                cooldown = slashCooldown * dexterity; 
                damage = attackStat;
                hitbox = _slashHitBox; 
                force = slashForce;
                startUpLength = attackParameters.attackStartupLength;
                activeLength = attackParameters.attackActiveLength;
                recoverLength = attackParameters.attackRecoverLength;
                normalSlashFX.Play();
                vfxPulling.PlaceBurnMark(0);
                //1st anim
                comboState = ComboState.SimpleAttack;
                PlayAnimation(attackDir);
                break;
            case ComboState.SimpleAttack : 
                cooldown = slashCooldown * dexterity; 
                damage = attackStat;
                hitbox = _slashHitBox; 
                force = slashForce;
                startUpLength = attackParameters.attackStartupLength;
                activeLength = attackParameters.attackActiveLength;
                recoverLength = attackParameters.attackRecoverLength;
                //2nd anim
                reverseSlashFX.Play();
                vfxPulling.PlaceBurnMark(1);
                comboState = ComboState.ReverseAttack;
                PlayAnimation(attackDir);
                break;
            case ComboState.ReverseAttack : 
                damage = attackStat * spinDamageMultiplier;
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
        
        //stops movement
        _pc.canMove = false;
        //add current damage stat to weapon
        hitbox.GetComponent<ObjectDamage>().damage = (damage);
        //adds force to simulate inertia
        _rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(startUpLength);

        //-----------active state
        //can touch enemies, hitbox active, is invincible
        SetAttackState(AttackState.Active);
        hitbox.SetActive(true);
        //_pc.invincibleCounter = activeLength;

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
        //_rb.velocity = Vector3.zero;
        //restores speed
        _pc.canMove = true;
        isAttacking = false;
    }
    IEnumerator SpinSlashes()
    {
        yield return new WaitForSeconds(attackParameters.spinStartupLength);
        float interval = attackParameters.spinActiveLength/4;
        vfxPulling.PlaceBurnMark(2);
        spinSlashFX.Play();
        _spinHitBox.SetActive(true);
        yield return new WaitForSeconds(interval);
        vfxPulling.PlaceBurnMark(2);
        _spinHitBox.SetActive(false);
        _spinHitBox.SetActive(true);
        spinSlashFX.Stop();
        spinSlashFX.Play();
        yield return new WaitForSeconds(interval);
        vfxPulling.PlaceBurnMark(2);
        _spinHitBox.SetActive(false);
        _spinHitBox.SetActive(true);
        spinSlashFX.Stop();
        spinSlashFX.Play();
        yield return new WaitForSeconds(interval);
        vfxPulling.PlaceBurnMark(2);
        _spinHitBox.SetActive(false);
        _spinHitBox.SetActive(true);
        spinSlashFX.Stop();
        spinSlashFX.Play();
        yield return new WaitForSeconds(interval);
        vfxPulling.PlaceBurnMark(2);
        _spinHitBox.SetActive(false);
        spinSlashFX.Stop();
        spinSlashFX.Play();
    }
    IEnumerator SmashCoroutine()
    {
        isAttacking = true;
        //-----------can attack again
        comboState = ComboState.Default;
        _animator.CrossFade(Idle, 0, 0);
        SetAttackState(AttackState.Default);
        //can walk again
        _pc.SwitchState(PlayerController.PlayerStates.Run);
        canInterruptAnimation = false;
        _pc.SwitchState(PlayerController.PlayerStates.Attack);
        //values assignation
        GameObject hitbox = null;
        Vector3 attackDir = Vector3.zero;
        smashForce = smashGauge / attackParameters.smashChargeLength;
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
        damage = (attackStat * smashDamageMultiplier);
        hitbox = _smashHitBox; 
        force = smashForce;
        startUpLength = attackParameters.smashStartupLength;
        activeLength = attackParameters.smashActiveLength;
        recoverLength = attackParameters.smashRecoverLength;
        //3rd anim
        comboState = ComboState.SmashAttack;
        PlayAnimation(attackDir);
        comboState = ComboState.Default;
        //determine ou l'attaque va se faire
        _attackAnchor.transform.LookAt(transform.position + attackDir);
        //stops movement
        _pc.canMove = false;
        //add current damage stat to weapon
        hitbox.GetComponent<ObjectDamage>().damage = damage;
        _rb.velocity = Vector3.zero;
        yield return new WaitUntil(()=> !_rightMouseHolding);
        //adds force to simulate inertia
        if (_pc.movementDir != Vector2.zero)
        {
            _rb.AddForce(attackDir * force, ForceMode.Impulse);
        }
        comboState = ComboState.SmashRelease;
        PlayAnimation(attackDir);
        comboState = ComboState.Default;
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
        
        Vector3 pos = new Vector3(transform.position.x, 0.03f, transform.position.z);
        vfxPulling.StartCoroutine(vfxPulling.PlaceNewVfx(vfxPulling.vfxList[0]));
        vfxPulling.StartCoroutine(vfxPulling.PlaceNewVfx(vfxPulling.particleList[3]));
        GameManager.instance._cmShake.ShakeCamera(7, .1f);
        yield return new WaitForSeconds(activeLength);

        //------------recovery state
        //hitbox inactive, not invincible
        hitbox.SetActive(false);
        yield return new WaitForSeconds(recoverLength);
        //-----------can attack again
        SetAttackState(AttackState.Default);
        //can walk again
        _pc.SwitchState(PlayerController.PlayerStates.Run);
        //waits cooldown depending on the attack used
        //restores speed
        smashPower = 0;
        smashGauge = 0;
        _pc.canMove = true;
        isAttacking = false;
    }
    void OnSmash(InputAction.CallbackContext context)
    {
        _rightMouseHolding = true;
        if (currentAttackState != AttackState.Active && currentAttackState != AttackState.Startup)
        {
            if (currentAttackState == AttackState.Default)
            {
                _rightMouseHolding = true;
            }
            
            _pc.canMove = true;
            StopAllCoroutines();
            StartCoroutine(SmashCoroutine());
        }
    }
    void OnReleaseSmash(InputAction.CallbackContext context)
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
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (currentAttackState != AttackState.Active && currentAttackState != AttackState.Startup)
        {
            _pc.canMove = true;
            StopAllCoroutines();
            StartCoroutine(AttackCoroutine());
        }
    }
    private void OnSmashHold()
    {
        if (smashGauge >= attackParameters.smashChargeLength)
        {
            smashGauge = attackParameters.smashChargeLength;
        }
        if (_rightMouseHolding)
        {
            _pc.currentState = PlayerController.PlayerStates.Attack;
            _rb.velocity = Vector3.zero;
            smashGauge += Time.deltaTime;
            smashPower = smashGauge / attackParameters.smashChargeLength;
        }
    }
    int GetAttackAnimation(Vector3 playerDir)
    {
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
                ComboState.SmashAttack => SmashPrepare,
                ComboState.SmashRelease => SmashRelease,
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
                ComboState.SmashAttack => SmashPrepare,
                ComboState.SmashRelease => SmashRelease,
                _ => state
            };
        }
        return state;
    }
    public void InterruptAttack()
    {
        //to make sure any attack is disabled
        StopAllCoroutines();
        //-----------can attack again
        SetAttackState(AttackState.Default);
        //can walk again
        _pc.SwitchState(PlayerController.PlayerStates.Run);
        //waits cooldown depending on the attack used
        _rb.velocity = Vector3.zero;
        //restores speed
        _pc.canMove = true;
        _pc._playerAttacks.isAttacking = false;
        
        _slashHitBox.SetActive(false);
        _smashHitBox.SetActive(false);
        _spinHitBox.SetActive(false);
    }
    
    #region InputSystemRequirements
    private void OnEnable()
    {
        _mouseHold = _playerControls.Player.LightAttack;
        _mouseHold.performed += OnAttack;
        _mouseHold.Enable();
        
        _rightClick = _playerControls.Player.HeavyAttack;
        _rightClick.performed += OnSmash;
        _rightClick.canceled += OnReleaseSmash;
        _rightClick.Enable();
    }
    private void OnDisable()
    {
        _mouseHold.Disable();
        _rightClick.Disable();
    }
    #endregion

}
