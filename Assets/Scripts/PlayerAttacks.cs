using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
public class PlayerAttacks : MonoBehaviour
{
    public static PlayerAttacks instance;

    #region Variables
    #region StatesDeclaration
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
    #endregion
    
    public float comboCooldown = 1f; //max time allowed to combo
    public float smashPower;
    public float comboTimer;
    private float _attackMultiplier = 1;
    public float rocksAmount = 5;
    public float rocksOffsetAmount = 4;
    public float rocksPlacementInterval = 0.1f;
    public float smashGauge;
    private int _comboCounter;
    private Vector3 _attackDir;
    
    private bool _isMouseHolding;
    [SerializeField] private bool rightMouseHolding;
    [HideInInspector] public bool isAttacking;
    
    #region Attack Values
    [Header("Attacks")] 
    public float bumpForce = 20;
    public float attackStat = 1;
    public float spinDamageMultiplier = 0.5f;
    public float smashDamageMultiplier = 5;
    public float dexterity;

    public float slashCooldown = 0.4f;
    public float spinCooldown = 0.7f;
    public float smashCooldown = 2;
    public float smashWarmup = 1;

    public float smashForce = 4;
    public float slashForce = 15;
    public float spinForce = 8;

    private GameObject _attackAnchor;
    private GameObject _smashHitBox;
    private GameObject _slashHitBox;
    private GameObject _spinHitBox;
    #endregion
    
    private InputAction _mouseHold;
    private InputAction _rightClick;
    private PlayerControls _playerControls;
    private Animator _animator;
    private Rigidbody _rb;
    private PlayerController _pc;
    public AttackParameters attackParameters;

    #region Animations
    private static readonly int Idle_Breath = Animator.StringToHash("Idle_Breath");
    private static readonly int AttackSide = Animator.StringToHash("Attack_Side");
    private static readonly int AttackBack = Animator.StringToHash("Attack_Back");
    private static readonly int AttackFront = Animator.StringToHash("Attack_Front");
    private static readonly int SecondAttackSide = Animator.StringToHash("SecondAttack_Side");
    private static readonly int SecondAttackBack = Animator.StringToHash("SecondAttack_Back");
    private static readonly int SecondAttackFront = Animator.StringToHash("SecondAttack_Front");
    private static readonly int SpinAttack = Animator.StringToHash("Spin_Attack");
    private static readonly int SmashPrepare = Animator.StringToHash("SmashPrepare");
    private static readonly int SmashRelease = Animator.StringToHash("SmashRelease");
    // public bool canInterruptAnimation;
    #endregion

    #region VFX
    public VisualEffect normalSlashFX;
    public VisualEffect reverseSlashFX;
    public VisualEffect spinSlashFX;
    public VisualEffect smashSlashFX;
    [HideInInspector] public VfxPulling vfxPulling;
    public ParticleSystem chargeFx1;
    public ParticleSystem chargeFx2;
    public ParticleSystem chargeFx3;
    public ParticleSystem smashPowerFx1;
    public ParticleSystem smashPowerFx2;
    public ParticleSystem smashPowerFx3;
    public bool canActiveFx1 = true;
    public bool canActiveFx2 = true;
    public bool canActiveFx3 = true;
    #endregion
    #endregion
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
    private void Timer()
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0)
            {
                _comboCounter = 0;
                comboState = ComboState.Default;
            }
        }
    
    #region Attacks
    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        //canInterruptAnimation = false;
        if (_pc.currentState != PlayerController.PlayerStates.Dash)
        {
            _rb.velocity = Vector3.zero;
        }
        _pc.SwitchState(PlayerController.PlayerStates.Attack);
        
        //values assignation
        GameObject hitbox = null;
        Vector3 attackDir;
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
        comboTimer = recoverLength + 0.2f;
        
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
    IEnumerator EarthquakeRocks(Vector3 initialPos, Vector3 direction)
    {
        for (int i = 0; i < rocksAmount; i++)
        {
            //instanciates a new vfx pulled from the vfx pulling script
            Vector3 rocksOffsetPos = initialPos + (direction * (rocksOffsetAmount * (i + 1)));
            vfxPulling.StartCoroutine(vfxPulling.PlaceNewVfx(vfxPulling.particleList[5], rocksOffsetPos, true));
            //waits a bit before spawning another one
            yield return new WaitForSeconds(rocksPlacementInterval);
        }
    }
    #endregion
    
    #region Smash
    IEnumerator SmashCoroutine()
    {
        isAttacking = true;
        _animator.CrossFade(SmashPrepare, 0, 0);
        _pc.SwitchState(PlayerController.PlayerStates.Attack); //stops walking
        _pc.canMove = false;

        //values
        Vector3 attackDir = Vector3.zero;
        var damage = (attackStat * smashDamageMultiplier);
        GameObject hitbox = _smashHitBox; 
        var force = smashForce;
        var startUpLength = attackParameters.smashStartupLength;
        var activeLength = attackParameters.smashActiveLength;
        var recoverLength = attackParameters.smashRecoverLength;
        
        //-----------startup state
        //defines values, blocks walking
        SetAttackState(AttackState.Startup);
        attackDir = _pc.movementDir != Vector2.zero ? new Vector3(_pc.movementDir.x, 0, _pc.movementDir.y) : new Vector3(_pc.lastWalkedDir.x, 0, _pc.lastWalkedDir.y);
        comboState = ComboState.SmashAttack;
        PlayAnimation(attackDir);
        comboState = ComboState.Default;
        _attackAnchor.transform.LookAt(transform.position + attackDir);
        //add current damage stat to weapon
        hitbox.GetComponent<ObjectDamage>().damage = damage;
        _rb.velocity = Vector3.zero;
        yield return new WaitUntil(()=> !rightMouseHolding);
        
        //adds force to simulate inertia
        if (_pc.movementDir != Vector2.zero)
        {
            _rb.AddForce(attackDir * force, ForceMode.Impulse);
        }
        comboState = ComboState.SmashRelease;
        PlayAnimation(attackDir);
        comboState = ComboState.Default;
        PlayerSounds.instance.PlayAttackSound(2);
        yield return new WaitForSeconds(startUpLength);
        
        //-----------active state
        //can touch enemies, hitbox active, is invincible
        _rb.velocity = Vector3.zero;
        SetAttackState(AttackState.Active);
        hitbox.SetActive(true);
        #region VFX
        //place vfx for burn marks, slash, and eventual objects
        if (ObjectsManager.instance.stinkyFish)
        {
            vfxPulling.StartCoroutine(vfxPulling.PlaceNewVfx(vfxPulling.particleList[6]));
        }

        if (ObjectsManager.instance.earthQuake)
        {
            StartCoroutine(EarthquakeRocks(transform.position, attackDir));
        }
        
        vfxPulling.StartCoroutine(vfxPulling.PlaceNewVfx(vfxPulling.vfxList[0]));
        vfxPulling.StartCoroutine(vfxPulling.PlaceNewVfx(vfxPulling.particleList[3]));
        #endregion
        GameManager.instance.cmShake.ShakeCamera(10, 0.1f);
        yield return new WaitForSeconds(activeLength);
        //------------recovery state
        //hitbox inactive, not invincible
        hitbox.SetActive(false);
        yield return new WaitForSeconds(recoverLength);
        //-----------can attack again
        //can walk again
        //waits cooldown depending on the attack used
        //restores speed
        SetAttackState(AttackState.Default);
        _pc.SwitchState(PlayerController.PlayerStates.Run);
        smashPower = 0;
        smashGauge = 0;
        _pc.canMove = true;
        isAttacking = false;
    }
    private void OnSmashHold()
    {
        smashForce = smashGauge / attackParameters.smashChargeLength;
        if (smashGauge >= attackParameters.smashChargeLength)
        {
            smashGauge = attackParameters.smashChargeLength;
        }
        
        if (rightMouseHolding)
        {
            _pc.currentState = PlayerController.PlayerStates.Attack;
            _rb.velocity = Vector3.zero;
            smashGauge += Time.deltaTime;
            smashPower = smashGauge / attackParameters.smashChargeLength;
            //varies smash preparation fx depending on the power needed
            if (smashPower < 0.33f)
            {
                if (canActiveFx1)
                {
                    Debug.Log("played fx 1");
                    smashPowerFx1.Play();
                    canActiveFx1 = false;
                }
                
                chargeFx1.gameObject.transform.localPosition = Vector3.zero;
                chargeFx2.gameObject.transform.localPosition = Vector3.right * 1000;
                chargeFx3.gameObject.transform.localPosition = Vector3.right * 1000;
            }
            else if (smashPower < 0.66f)
            {
                if (canActiveFx2)
                {
                    Debug.Log("played fx 2");
                    canActiveFx2 = false;
                    smashPowerFx2.Play();
                }
                
                chargeFx1.gameObject.transform.localPosition = Vector3.right * 1000;
                chargeFx2.gameObject.transform.localPosition = Vector3.zero;
                chargeFx3.gameObject.transform.localPosition = Vector3.right * 1000;
            }
            else
            {
                if (canActiveFx3)
                {
                    Debug.Log("played fx 3");
                    canActiveFx3 = false;
                    smashPowerFx3.Play();
                }
                
                chargeFx1.gameObject.transform.localPosition = Vector3.right * 1000;
                chargeFx2.gameObject.transform.localPosition = Vector3.right * 1000;
                chargeFx3.gameObject.transform.localPosition = Vector3.zero;
            }
        }
    }
    void OnReleaseSmash(InputAction.CallbackContext context)
    {
        rightMouseHolding = false;
        chargeFx1.gameObject.transform.localPosition = Vector3.right * 1000;
        chargeFx2.gameObject.transform.localPosition = Vector3.right * 1000;
        chargeFx3.gameObject.transform.localPosition = Vector3.right * 1000;
        canActiveFx1 = true;
        canActiveFx2 = true;
        canActiveFx3 = true;
    }
    void OnSmash(InputAction.CallbackContext context)
    {
        rightMouseHolding = true;
        if (currentAttackState != AttackState.Active && currentAttackState != AttackState.Startup)
        {
            if (currentAttackState == AttackState.Default)
            {
                rightMouseHolding = true;
            }
            
            _pc.canMove = true;
            StopAllCoroutines();
            StartCoroutine(SmashCoroutine());
        }
    }
    #endregion
    
    #region Animations
    int GetAttackAnimation(Vector3 playerDir)
    {
        var state = Idle_Breath;
        //checks player speed for orientation
        var xVal = playerDir.x;
        var yVal = playerDir.z >= 0 ? playerDir.z : -playerDir.z;

        PlayerController.instance.spriteRenderer.flipX = xVal < 0;
        //if is attacking, animation plays, then locks current state during x seconds
        //checks the best option depending on the speed
        if (yVal >= xVal)
        {
            //if y value is dominant, plays up or down anims
            state = comboState switch
            {
                ComboState.SimpleAttack => playerDir.z >= 0 ? AttackBack : AttackFront,
                ComboState.ReverseAttack => playerDir.z >= 0 ? SecondAttackBack : SecondAttackFront,
                ComboState.SpinAttack => SpinAttack,
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
                ComboState.SimpleAttack => AttackSide,
                ComboState.ReverseAttack => SecondAttackSide,
                ComboState.SpinAttack => SpinAttack,
                ComboState.SmashAttack => SmashPrepare,
                ComboState.SmashRelease => SmashRelease,
                _ => state
            };
        }
        return state;
    }
    void PlayAnimation(Vector3 playerDirection)
    {
        StopCoroutine(PlayerController.instance.IdleAnimations());
        int state = GetAttackAnimation(playerDirection);
        if (state == _pc.currentAnimatorState) return;
        _animator.CrossFade(state, 0, 0);
        _pc.currentAnimatorState = state;    
    }
    #endregion

    #region StateManagement
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (currentAttackState is AttackState.Default or AttackState.Recovery && PlayerController.instance.currentState != PlayerController.PlayerStates.Dash)
        {
            _pc.canMove = true;
            StopAllCoroutines();
            StartCoroutine(AttackCoroutine());
        }
    }
    private void SetAttackState(AttackState state)
    {
        currentAttackState = state;
    }
    public void InterruptAttack()
    {
        //to make sure any attack is disabled
        StopCoroutine(AttackCoroutine());
        StopCoroutine(SpinSlashes());
        StopCoroutine(SmashCoroutine());
        //-----------can attack again
        SetAttackState(AttackState.Default);
        //can walk again
        _pc.SwitchState(PlayerController.PlayerStates.Run);
        //waits cooldown depending on the attack used
        _rb.velocity = Vector3.zero;
        //restores speed
        _pc.canMove = true;
        isAttacking = false;
        
        _slashHitBox.SetActive(false);
        _smashHitBox.SetActive(false);
        _spinHitBox.SetActive(false);
    }
    #endregion
    
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
