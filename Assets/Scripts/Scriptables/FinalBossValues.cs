using System;
using UnityEngine;
[CreateAssetMenu(fileName = "FinalBossValues", menuName = "ScriptableObjects/FinalBossValues")]


public class FinalBossValues : ScriptableObject
{
    [Header("Values \n")]
    public float bodyMaxHealth;
    public float handMaxHealth;
    public float handRespawnCooldown;
    public float _attackCooldown;
    public Vector2 roomBounds;

    [Header("Attack Values \n")]

    [Header("Claw")]
    public float clawWarmup;
    
    [Header("Circle")]
    public float circleLength;
    public float circleOriginalSize;
    public float circlePosInterval;

    [Header("EyeLightning")] public float wandererSpawnAmount;
    public float wandererSpawnInterval;
    
    [Header("EyeLightning")]
    public float eyeNumber;
    public float eyeSpawnInterval;
    
    [Header("HugeLaser")]
    public float hugeLaserWarmup;
    public float rockWarmup;

    [Header("M_Laser")]
    public LayerMask wallLayerMask;
    public LayerMask groundLayerMask;
    public LayerMask playerLayerMask;
    public float m_laserLength = 1;
    public float m_laserCooldown = 1;
    public float m_laserWarmup = 2;
    public float _playerPosDelay = 0.0f;
    public Gradient laserGradient;

    [Header("AttackDamages \n")]
    public float clawDamage;
    public float circleDamage;
    public float eyesDamage;
    public float m_laserDamage;
    public float hugeLaserDamage;

    private void Awake()
    {
        wallLayerMask = LayerMask.GetMask("Wall");
        playerLayerMask = LayerMask.GetMask("Player");
        groundLayerMask = LayerMask.GetMask("Ground");
    }
}
