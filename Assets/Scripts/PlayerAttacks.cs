using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour
{
    public float _comboCooldown = 1f; //max time allowed to combo
    public float _comboTimer;
    private float _attackMultiplier = 1;
    private bool _isMouseHolding;
    public float smashGauge;
    [HideInInspector] public bool isMasterSword;
    [HideInInspector] public bool canRepel;
    [HideInInspector] public bool noPet;
    private int _comboCounter;
    private Vector3 _attackDir;
    private InputAction _mouseHold;
    private PlayerControls _playerControls;

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
        _attackAnchor = transform.GetChild(0).gameObject;
        _slashHitBox = _attackAnchor.transform.GetChild(0).gameObject;
        _pickHitBox = _attackAnchor.transform.GetChild(1).gameObject;
        _smashHitBox = _attackAnchor.transform.GetChild(2).gameObject;
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _playerControls = new PlayerControls();
        _pc = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (currentAttackState != AttackState.Active && currentAttackState != AttackState.Startup)
        {
            AttackManagement();
            Timer();
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
        
        /*
        //if master sword, launches projectile
        if (isMasterSword && _gameManager.health == _gameManager.maxHealth)
        {
            GameObject projo = Instantiate(masterSwordProjo, transform.position, quaternion.identity);
            projo.GetComponent<Rigidbody>().AddForce(attackDir * 100);
        }*/
        
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
                damage = Mathf.CeilToInt(attackStat);
                hitbox = _slashHitBox; 
                force = slashForce;
                startUpLength = attackParameters.attackStartupLength;
                activeLength = attackParameters.attackActiveLength;
                recoverLength = attackParameters.attackRecoverLength;
                //1st anim
                comboState = ComboState.SimpleAttack;
                PlayAnimation(attackDir);
                break;
            case ComboState.SimpleAttack : 
                cooldown = slashCooldown * dexterity; 
                damage = Mathf.CeilToInt(attackStat);
                hitbox = _slashHitBox; 
                force = slashForce;
                startUpLength = attackParameters.attackStartupLength;
                activeLength = attackParameters.attackActiveLength;
                recoverLength = attackParameters.attackRecoverLength;
                //2nd anim
                comboState = ComboState.ReverseAttack;
                PlayAnimation(attackDir);
                break;
            case ComboState.ReverseAttack : 
                cooldown = pickCooldown * dexterity; 
                damage = Mathf.CeilToInt(attackStat * pickDamageMultiplier);
                hitbox = _pickHitBox; 
                force = pickForce;
                startUpLength = attackParameters.pickStartupLength;
                activeLength = attackParameters.pickActiveLength;
                recoverLength = attackParameters.pickRecoverLength;
                //3rd anim
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
        _rb.AddForce(attackDir * force, ForceMode.Impulse);
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
    }
    void MouseHold(InputAction.CallbackContext context)
        {
            _isMouseHolding = true;
        }
    void MouseReleased(InputAction.CallbackContext context)
        {
            _isMouseHolding = false;
        }
    #region Timer
    void Timer()
        {
            _comboTimer -= Time.deltaTime;

            if (_comboTimer <= 0)
            {
                _comboCounter = 0;
                comboState = ComboState.Default;
            }
        }
    #endregion
    /*
    IEnumerator Smash()
        {
            SwitchState(PlayerController.PlayerStates.Attack);
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
            SwitchState(PlayerController.PlayerStates.Run);
        }*/
    void AttackManagement()
         {
             if (_isMouseHolding)
             {
                 _pc.canMove = false;
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
                         _pc.canMove = true;
                         StopAllCoroutines();
                         StartCoroutine(AttackCoroutine());
                     }
                     //if too long, smash or pass (lol)
                     else 
                     {
                         if (smashGauge == 1)
                         {
                             //StartCoroutine(Smash());
                         }
                         else
                         {
                             _pc.canMove = true;
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

    int GetAttackAnimation(Vector3 playerDir)
    {
        //Debug.Log("attack anim" + comboState);
        int state = Idle;
        //checks player speed for orientation
        float xVal = playerDir.x >= 0 ? playerDir.x : -playerDir.x;
        float yVal = playerDir.z >= 0 ? playerDir.z : -playerDir.z;
        //if is attacking, animation plays, then locks current state during x seconds
        //checks the best option depending on the speed
        if (yVal >= xVal)
        {
            //if y value is dominant, plays up or down anims
            switch (comboState)
            {
                case ComboState.SimpleAttack:
                    state = playerDir.z >= 0 ? Attack_Back : Attack_Front;
                    break;
                case ComboState.ReverseAttack:
                    state = playerDir.z >= 0 ? SecondAttack_Back : SecondAttack_Front;
                    break;
                case ComboState.SpinAttack:
                    state = Spin_Attack;
                    break;
            }
        }
        else
        {
            //else, plays side
            switch (comboState)
            {
                case ComboState.SimpleAttack:
                    state = Attack_Side;
                    break;
                case ComboState.ReverseAttack:
                    state = SecondAttack_Side;
                    break;
                case ComboState.SpinAttack:
                    state = Spin_Attack;
                    break;
            }
        }
        return state;
    }
    
    int GetAttackAnimationNoDir()
    {
        //Debug.Log("attack anim" + comboState);
        int state = Idle;
        switch (comboState)
        {
            case ComboState.SimpleAttack:
                state = Attack_Side;
                break;
            case ComboState.ReverseAttack:
                state = SecondAttack_Side;
                break;
            case ComboState.SpinAttack:
                state = Spin_Attack;
                break;
        }
        Debug.Log(comboState);
        return state;
    }

    #region InputSystemRequirements
    private void OnEnable()
    {
        _mouseHold = _playerControls.Player.AttackHold;
        _mouseHold.started += MouseHold;
        _mouseHold.canceled += MouseReleased;
        _mouseHold.Enable();
    }
    private void OnDisable()
    {
        _mouseHold.Disable();
    }
    #endregion

}
