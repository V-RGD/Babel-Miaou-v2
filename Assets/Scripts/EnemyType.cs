using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ObjectTextData", menuName = "ScriptableObjects/ObjectTextData", order = 1)]

public class EnemyType : ScriptableObject
{
    //tweak values
    [Header("Enemy Stats")]
    public float speed;
    public float max_health;
    public float damage;
    public float attackRange;
    public int eyesDropped;

    //values
    private int dashForce;
    private float _health;
    private float _playerDist;
    private float _speedFactor;
    private float projectileDamage;
    private float projectileRate;
    private float stunLenght;
    private float shootWarmup;
    private float dashWarmUp;
    private bool _isHit;
    private Vector3 dashDir;

    //type related values
    private bool _canShootProjectile;
    private bool _isStunned;
    private bool _isDashing;
    private bool _canDash;
    private bool _isTouchingWall;
    private bool _isVulnerable;
    private int _projectileForce;
    private float _stunCounter;
    private LayerMask wallLayerMask;
    private Vector3 _projectileDir;

    //components
    [Header("*Objects*")]
    public GameObject eyeToken;
    public GameObject mageProjectile;
}
