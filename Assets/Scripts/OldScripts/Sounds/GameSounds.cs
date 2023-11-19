using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSounds : MonoBehaviour
{
    public static GameSounds instance;
    
    [Header("Enemies")]
    public AudioClip[] enemyInvocation;
    public AudioClip[] enemyDeath;
    public AudioClip[] wandererClaw;
    public AudioClip[] bullSteps;
    public AudioClip[] bullDash;
    public AudioClip[] bullKnock;
    public AudioClip[] bullHurt;
    public AudioClip[] shooterSteps;
    public AudioClip[] shooterShoot;
    public AudioClip[] shooterProjectile;
    public AudioClip[] shooterRecharge;
    public AudioClip[] marksmanCharge;
    public AudioClip[] marksmanShoot;
    public AudioClip[] bossClaw;
    public AudioClip[] bossInvocation;
    public AudioClip[] bossRock;
    public AudioClip[] bossLaser;
    public AudioClip[] bossLaugh;
    public AudioClip[] bossHurt;
    public AudioClip[] bossAttack;
    [Header("Player")]
    public AudioClip[] playerHeal;
    public AudioClip[] playerSteps;
    public AudioClip[] playerAttacks;
    public AudioClip[] playerHeartbeat;
    public AudioClip[] playerHurt;
    [Header("UI")]
    public AudioClip[] itemAcquired;
    public AudioClip[] itemPurchased;
    public AudioClip[] uiClick;
    public AudioClip[] menuChange;
    public AudioClip[] coinEarned;
    [Header("Environment")]
    public AudioClip[] braseroFireplace;

    public AudioClip invocation;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
