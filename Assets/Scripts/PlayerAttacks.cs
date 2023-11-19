using System;
using System.Collections;
using UnityEngine;

public class PlayerAttacks : AttackSystem, IMeleeAttacker
{
    public static PlayerAttacks Instance;

    [SerializeField] float coyoteTime = 0.3f;
    [SerializeField] float cooldown = 0.6f;
    [SerializeField] Attack attack;
    [field: SerializeField] public Transform meleeAnchor { get; set; }
    [SerializeField] public float backStabMultiplier = 1.3f;

    public bool canAttack;
    float _coyoteTimer;
    float _cooldownTimer;
    Camera _cam;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        _cam = Camera.main;
        if (_cam == null) Debug.LogError("Failed reaching cam");
    }

    public void InputAttack(Vector2 dir)
    {
        if (_cooldownTimer > 0) return;
        attackDir = dir;
        _coyoteTimer = coyoteTime;
    }

    void Update()
    {
        _coyoteTimer -= Time.deltaTime;
        _cooldownTimer -= Time.deltaTime;

        //waits till player has stopped and if coyote is active
        // if (_coyoteTimer > 0 && !PlayerController.Instance.isMoving)
        // {
        //     if (canAttack) TriggerAttack();
        // }
    }

    void TriggerAttack()
    {
        //disables coyote
        _coyoteTimer = 0;
        _cooldownTimer = cooldown;

        //disables movement
        PlayerController.Instance.enabled = false;
        StartCoroutine(EnableMovementAfterAttack(attack.warmup + attack.length + attack.endLag));

        //starts attack
        StartCoroutine(DefaultMelee(attack, meleeAnchor, attackDir, Color.black, Color.red));
    }

    IEnumerator EnableMovementAfterAttack(float activeTime)
    {
        yield return new WaitForSeconds(activeTime);
        PlayerController.Instance.enabled = true;
    }
}