using UnityEngine;

/// <summary>
/// Used to animate and add VFX to the player when needed
/// </summary>
public class PlayerVisuals : MonoBehaviour
{
    
    // [Header("Animation")] [SerializeField] public SpriteRenderer spriteRenderer;
    // [Header("VFX")] float _lockedTill;
    // [HideInInspector] public int currentAnimatorState;
    //
    // #region AnimatorStates
    
    Animator _animator;
    static readonly int IdleLick = Animator.StringToHash("Idle_Lick");
    static readonly int IdleHeadFlip = Animator.StringToHash("Idle_HeadFlip");
    static readonly int IdleBreath = Animator.StringToHash("Idle_Breath");
    static readonly int Dash = Animator.StringToHash("Dash");
    static readonly int Run_Up = Animator.StringToHash("Run_Up");
    static readonly int Run_Down = Animator.StringToHash("Run_Down");
    static readonly int Run_Left = Animator.StringToHash("Run_Left");
    static readonly int Run_Right = Animator.StringToHash("Run_Right");
    static readonly int Run_DiagonalUp = Animator.StringToHash("Run_DiagonalUp");
    static readonly int Run_DiagonalDown = Animator.StringToHash("Run_DiagonalDown");
    
    void Update()
    {
        if(PlayerStateMachine.Instance.currentState is not PlayerStateMachine.PlayerState.Running) return;
        
        Vector3 vel = PlayerController.Instance.rb.velocity;
        //either animates movement or idles if static
        if (vel != Vector3.zero)AnimateMovement();
        else AnimateIdle();
    }

    void AnimateMovement()
    {
        
    }
    
    int GetMovingAnimation()
    {
        
        Debug.LogError("could find animation");
        return 0;
    }
    

    void AnimateAttacks()
    {

    }

    void AnimateIdle()
    {
        // public IEnumerator IdleAnimations()
        // {
        //     spriteRenderer.flipX = false;
        //     //plays two breathing then a random idle
        //     isIdle = true;
        //     float variantLenght = 1;
        //     var randAnim = Random.Range(0, 2);
        //     switch (randAnim)
        //     {
        //         case 0:
        //             _animator.CrossFade(IdleLick, 0, 0);
        //             currentAnimatorState = IdleLick;
        //             variantLenght = 0.55f;
        //             break;
        //         case 1:
        //             _animator.CrossFade(IdleHeadFlip, 0, 0);
        //             currentAnimatorState = IdleHeadFlip;
        //             variantLenght = 1.35f;
        //             break;
        //     }
        //
        //     yield return new WaitForSeconds(variantLenght);
        //     idleTimer = 0;
        //     isIdle = false;
        // }
    }

    void SwitchAnimation(int anim)
    {
        _animator.CrossFade(anim, 0, 0);
    }

    
    // PlayerRemnants _remnants;
    // public bool isIdle = false;
    // public float idleTimer;
    // public ParticleSystem dashTrail;
    
    
    
    //
    // #endregion
    //
    // void Timer()
    // {
    //     if (rb.velocity.magnitude < 01f)
    //         idleTimer += Time.deltaTime;
    //     else
    //         idleTimer = 0;
    //     //StopCoroutine(IdleAnimations());
    // }
}