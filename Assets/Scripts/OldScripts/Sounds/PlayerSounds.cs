using System;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public static PlayerSounds instance;
    //step sounds
    public RandSoundGen stepsGen;
    //arm movement
    public RandSoundGen armGen;
    //sword movement
    public RandSoundGen swordGen;

    public AudioSource dashSource;
    public AudioSource eyeSource;
    
    //burn marks - placer sur le vfx
    public RandSoundGen burnGen;
    //smash impact - placer sur le vfx
    public RandSoundGen impactGen;
    //enemy hit - placer sur les ennemis
    public RandSoundGen enemyHurtGen;
    //enemy hurt
    public RandSoundGen enemyHitGen;

    public float stepInterval = 0.2f;
    public float stepCooldownCounter;
    public bool isPlayerWalkingOnSand;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        stepCooldownCounter = 0;
    }

    private void Update()
    {
        stepCooldownCounter -= Time.deltaTime;
        //if the player is moving
        if (PlayerController__old.instance.movementDir != Vector2.zero && 
            PlayerController__old.instance.currentState == PlayerController__old.PlayerStates.Run)
        {
            //if can play sound
            if (stepCooldownCounter <= 0)
            {
                stepCooldownCounter = stepInterval;
                PlayStepSounds();
            }
        }
    }

    void PlayStepSounds()
    {
        if (isPlayerWalkingOnSand)
        {
            stepsGen.PlayRandomSound(1);
        }
        else
        {
            stepsGen.PlayRandomSound(0);
        }
    }

    public void PlayAttackSound(int type)
    {
        switch (type)
        {
            case 0 : 
                armGen.PlayRandomSound(0);
                swordGen.PlayRandomSound(0);
                break;
            case 1 : 
                armGen.PlayRandomSound(1);
                swordGen.PlayRandomSound(1);
                break;
            case 2 : 
                armGen.PlayRandomSound(2);
                swordGen.PlayRandomSound(2);
                break;
        }
    }
    
    
}
