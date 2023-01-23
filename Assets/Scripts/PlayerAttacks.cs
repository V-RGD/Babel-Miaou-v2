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
        ThirdAttack,
        SpinAttack,
        SmashAttack,
        SmashRelease,
    }
    public enum SmashState
    {
        None,
        Blue,
        Orange,
        Purple
    }
    public SmashState smashState;
    public ComboState comboState;
    public AttackState currentAttackState;
    #endregion
    
    public float comboCooldown = 1f; //max time allowed to combo
    public float smashPowerTimer;
    private float smashPower;
    private float comboTimer;
    private float _attackMultiplier = 1;
    private float rocksPlacementInterval = 0.1f;
    private float smashGauge;
    private int _comboCounter;
    private Vector3 _attackDir;
    
    private bool _isMouseHolding;
    [SerializeField] private bool rightMouseHolding;
    [HideInInspector] public bool isAttacking;
    
    #region Attack Values
    [Header("Attacks")] 
    private float spinDamageMultiplier = 0.5f;
    public float smashDamageMultiplier = 2;
    [HideInInspector] public float bumpForce = 20;
    [HideInInspector] public float attackStat = 1;
    [HideInInspector] public float dexterity;

    private float slashCooldown = 0.4f;
    private float spinCooldown = 0.7f;
    private float smashCooldown = 2;
    private float smashWarmup = 1;

    private float smashForce = 4;
    private float slashForce = 15;
    private float spinForce = 8;

    private GameObject _attackAnchor;
    private GameObject _slashHitBox;
    private GameObject _spinHitBox;
    private GameObject _smashHitBox1;
    private GameObject _smashHitBox2;
    private GameObject _smashHitBox3;
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
    public VfxPulling burnVfxPulling;
    public VfxPulling smashVfxPulling;
    public VfxPulling poisonVfxPulling;
    public VfxPulling rocksVfxPulling;
    public ParticleSystem chargeFx1;
    public ParticleSystem chargeFx2;
    public ParticleSystem chargeFx3;
    public ParticleSystem smashPowerFx1;
    public ParticleSystem smashPowerFx2;
    public ParticleSystem smashPowerFx3;
    private bool canActiveFx1 = true;
    private bool canActiveFx2 = true;
    private bool canActiveFx3 = true;
    #endregion
    #endregion
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        
        _attackAnchor = transform.GetChild(0).gameObject;
        _slashHitBox = _attackAnchor.transform.GetChild(0).gameObject;
        _spinHitBox = _attackAnchor.transform.GetChild(1).gameObject;
        _smashHitBox1 = _attackAnchor.transform.GetChild(2).gameObject;
        _smashHitBox2= _attackAnchor.transform.GetChild(3).gameObject;
        _smashHitBox3 = _attackAnchor.transform.GetChild(4).gameObject;
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
        comboTimer = recoverLength + 0.3f;
        
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
        burnVfxPulling.attackDir = attackDir;
        //StopCoroutine(PlayerController.instance.IdleAnimations());

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
                burnVfxPulling.PlaceBurnMark(0);
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
                burnVfxPulling.PlaceBurnMark(1);
                comboState = ComboState.ReverseAttack;
                PlayAnimation(attackDir);
                break;
            case ComboState.ReverseAttack : 
                cooldown = slashCooldown * dexterity; 
                damage = attackStat;
                hitbox = _slashHitBox; 
                force = slashForce;
                startUpLength = attackParameters.attackStartupLength;
                activeLength = attackParameters.attackActiveLength;
                recoverLength = attackParameters.attackRecoverLength;
                normalSlashFX.Play();
                burnVfxPulling.PlaceBurnMark(0);
                //1st anim
                comboState = ComboState.ThirdAttack;
                PlayAnimation(attackDir);
                break;
            case ComboState.ThirdAttack : 
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

        if (_pc.movementDir != Vector2.zero)
        {
            _rb.AddForce(attackDir * force, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(activeLength);

        //------------recovery state
        SetAttackState(AttackState.Recovery);
        hitbox.SetActive(false);
        yield return new WaitForSeconds(recoverLength);

        //-----------can attack again
        SetAttackState(AttackState.Default);
        comboState = ComboState.Default;
        _pc.SwitchState(PlayerController.PlayerStates.Run);
        _pc.canMove = true;
        isAttacking = false;
    }
    IEnumerator SpinSlashes()
    {
        yield return new WaitForSeconds(attackParameters.spinStartupLength);
        float interval = attackParameters.spinActiveLength/4;
        burnVfxPulling.PlaceBurnMark(2);
        spinSlashFX.Play();
        _spinHitBox.SetActive(true);
        yield return new WaitForSeconds(interval);
        burnVfxPulling.PlaceBurnMark(2);
        _spinHitBox.SetActive(false);
        _spinHitBox.SetActive(true);
        spinSlashFX.Stop();
        spinSlashFX.Play();
        yield return new WaitForSeconds(interval);
        burnVfxPulling.PlaceBurnMark(2);
        _spinHitBox.SetActive(false);
        _spinHitBox.SetActive(true);
        spinSlashFX.Stop();
        spinSlashFX.Play();
        yield return new WaitForSeconds(interval);
        burnVfxPulling.PlaceBurnMark(2);
        _spinHitBox.SetActive(false);
        _spinHitBox.SetActive(true);
        spinSlashFX.Stop();
        spinSlashFX.Play();
        yield return new WaitForSeconds(interval);
        burnVfxPulling.PlaceBurnMark(2);
        _spinHitBox.SetActive(false);
        spinSlashFX.Stop();
        spinSlashFX.Play();
    }
    IEnumerator EarthquakeRocks(Vector3 initialPos, Vector3 direction, int power)
    {
        int rocksAmount = 0;
        float damage = 0;
        float rocksOffset = 0;
        int shakePower = 0;
        switch (power)
        {
            case 0 :
                rocksAmount = 3;
                rocksOffset = 2;
                shakePower = 1;
                break;
            case 1 :
                rocksAmount = 4;
                rocksOffset = 3;
                shakePower = 2;
                break;
            case 2 :
                rocksAmount = 5;
                rocksOffset = 4;
                shakePower = 3;
                break;
        }
        
        for (int i = 0; i < rocksAmount; i++)
        {
            //instanciates a new vfx pulled from the vfx pulling script
            Vector3 rocksOffsetPos = initialPos + direction * (rocksOffset * (i + 1));
            rocksVfxPulling.StartCoroutine(rocksVfxPulling.PlaceNewVfx(rocksVfxPulling.particleList[power], rocksOffsetPos, true));
            StartCoroutine(DelayRockShake(shakePower, 0.15f));
            //waits a bit before spawning another one
            yield return new WaitForSeconds(rocksPlacementInterval);
        }
    }
    IEnumerator DelayRockShake(int power, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.instance.cmShake.ShakeCamera(power, 0.1f);
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
        var damage = 0;
        GameObject hitbox = _smashHitBox1; 
        var force = smashForce;
        var startUpLength = attackParameters.smashStartupLength;
        var activeLength = attackParameters.smashActiveLength;
        var recoverLength = attackParameters.smashRecoverLength;
        var shakeStrengh = 0;

        //-----------startup state
        //defines values, blocks walking
        SetAttackState(AttackState.Startup);
        attackDir = _pc.movementDir != Vector2.zero ? new Vector3(_pc.movementDir.x, 0, _pc.movementDir.y) : new Vector3(_pc.lastWalkedDir.x, 0, _pc.lastWalkedDir.y);
        comboState = ComboState.SmashAttack;
        //StopCoroutine(PlayerController.instance.IdleAnimations());
        PlayAnimation(attackDir);
        comboState = ComboState.Default;
        _attackAnchor.transform.LookAt(transform.position + attackDir);
        _rb.velocity = Vector3.zero;
        yield return new WaitUntil(()=> !rightMouseHolding);
        float smashPowerMultiplier = 0;
        if (smashPowerTimer < 0.66f)
        {
            smashPower = 0;
            shakeStrengh = 3;
            smashPowerMultiplier = 0.4f;
        }
        else if (smashPowerTimer < 0.95f)
        {
            smashPower = 1;
            shakeStrengh = 4;
            smashPowerMultiplier = 0.6f;
        }
        else
        {
            smashPower = 2;
            shakeStrengh = 5;
            smashPowerMultiplier = 1f;
        }
        
        switch (smashPower)
        {
            case 0 : 
                hitbox = _smashHitBox1;
                break;
            case 1 : 
                hitbox = _smashHitBox2;
                break;
            case 2 : 
                hitbox = _smashHitBox3;
                break;
        }
        //sets hitbox damage
        hitbox.GetComponent<ObjectDamage>().damage = attackStat * smashDamageMultiplier * smashPowerMultiplier;

        if (smashPowerTimer < 0.33f)
        {
            AbortSmash();
            yield break;
        }
        
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
            ObjectsManager.instance.PlayActivationVfx(2);
            switch (smashPower)
            {
                case 0 : 
                    poisonVfxPulling.StartCoroutine(poisonVfxPulling.PlaceNewVfx(poisonVfxPulling.particleList[0]));
                    break;
                case 1 : 
                    poisonVfxPulling.StartCoroutine(poisonVfxPulling.PlaceNewVfx(poisonVfxPulling.particleList[1]));
                    break;
                case 2 : 
                    poisonVfxPulling.StartCoroutine(poisonVfxPulling.PlaceNewVfx(poisonVfxPulling.particleList[2]));
                    break;
            }
        }

        if (ObjectsManager.instance.earthQuake)
        {
            ObjectsManager.instance.PlayActivationVfx(5);
            switch (smashPower)
            {
                case 0 : 
                    StartCoroutine(EarthquakeRocks(transform.position, attackDir, 0));
                    break;
                case 1 : 
                    StartCoroutine(EarthquakeRocks(transform.position, attackDir, 1));
                    break;
                case 2 : 
                    StartCoroutine(EarthquakeRocks(transform.position, attackDir, 2));
                    break;
            }
        }
        
        switch (smashPower)
        {
            case 0 : 
                smashVfxPulling.StartCoroutine(smashVfxPulling.PlaceNewVfx(smashVfxPulling.vfxList[0]));
                smashVfxPulling.StartCoroutine(smashVfxPulling.PlaceNewVfx(smashVfxPulling.particleList[0]));
                break;
            case 1 : 
                smashVfxPulling.StartCoroutine(smashVfxPulling.PlaceNewVfx(smashVfxPulling.vfxList[1]));
                smashVfxPulling.StartCoroutine(smashVfxPulling.PlaceNewVfx(smashVfxPulling.particleList[1]));
                break;
            case 2 : 
                smashVfxPulling.StartCoroutine(smashVfxPulling.PlaceNewVfx(smashVfxPulling.vfxList[2]));
                smashVfxPulling.StartCoroutine(smashVfxPulling.PlaceNewVfx(smashVfxPulling.particleList[2]));
                break;
        }
        #endregion
        StartCoroutine(GameManager.instance.ShakeCam(shakeStrengh));
        yield return new WaitForSeconds(activeLength);
        //------------recovery state
        //hitbox inactive, not invincible
        hitbox.SetActive(false);
        smashState = SmashState.None;
        yield return new WaitForSeconds(recoverLength);
        //-----------can attack again
        //can walk again
        SetAttackState(AttackState.Default);
        _pc.SwitchState(PlayerController.PlayerStates.Run);
        smashGauge = 0;
        smashPower = 0;
        smashPowerTimer = 0;
        _pc.canMove = true;
        isAttacking = false;
    }

    void AbortSmash()
    {
        SetAttackState(AttackState.Default);
        _pc.SwitchState(PlayerController.PlayerStates.Run);
        smashGauge = 0;
        smashPower = 0;
        smashPowerTimer = 0;
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
            smashPowerTimer = smashGauge / attackParameters.smashChargeLength;
            //varies smash preparation fx depending on the power needed
            if (smashPowerTimer < 0.33f)
            {
                smashState = SmashState.None;
                chargeFx1.gameObject.transform.localPosition = Vector3.zero;
                chargeFx2.gameObject.transform.localPosition = Vector3.right * 1000;
                chargeFx3.gameObject.transform.localPosition = Vector3.right * 1000;
            }
            else if (smashPowerTimer < 0.66f)
            {
                smashState = SmashState.Blue;
                chargeFx1.gameObject.transform.localPosition = Vector3.right * 1000;
                chargeFx2.gameObject.transform.localPosition = Vector3.zero;
                chargeFx3.gameObject.transform.localPosition = Vector3.right * 1000;
            }
            else if (smashPowerTimer < 0.95f)
            {
                smashState = SmashState.Orange;
                chargeFx1.gameObject.transform.localPosition = Vector3.right * 1000;
                chargeFx2.gameObject.transform.localPosition = Vector3.right * 1000;
                chargeFx3.gameObject.transform.localPosition = Vector3.zero;
            }
            else
            {
                smashState = SmashState.Purple;
            }
            
            //pour les passages entre chaque état de chargement
            float delay = 0.33f;
            
            if (smashPowerTimer > 0.33f - delay)
            {
                if (canActiveFx1)
                {
                    smashPowerFx1.Play();
                    canActiveFx1 = false;
                }
            }
            if (smashPowerTimer > 0.66f - delay)
            {
                if (canActiveFx2)
                {
                    smashPowerFx2.Play();
                    canActiveFx2 = false;
                }
            }
            if (smashPowerTimer > 0.99f - delay)
            {
                if (canActiveFx3)
                {
                    smashPowerFx3.Play();
                    canActiveFx3 = false;
                }
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
        smashPowerFx1.Stop();
        smashPowerFx2.Stop();
        smashPowerFx3.Stop();
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
                ComboState.ThirdAttack => playerDir.z >= 0 ? AttackBack : AttackFront,
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
                ComboState.ThirdAttack => AttackSide,
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
        StopCoroutine(nameof(AttackCoroutine));
        StopCoroutine(nameof(SpinSlashes));
        StopCoroutine(nameof(SmashCoroutine));
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
        _smashHitBox1.SetActive(false);
        _smashHitBox2.SetActive(false);
        _smashHitBox3.SetActive(false);
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
        if (_mouseHold == null)
        {
            return;
        }
        
        _mouseHold.Disable();
        _rightClick.Disable();
    }
    #endregion
}
