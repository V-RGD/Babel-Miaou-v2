public class PlayerStateMachine : GenericSingletonClass<PlayerStateMachine>
{
    public PlayerState currentState;
    public enum PlayerState
    {
        Running,
        Attacking,
        Dashing,
        Hurt
    }

    void Update()
    {
        Behaviour();
    }

    /// <summary>
    /// Manages Player Behaviour depending on the current State
    /// </summary>
    void Behaviour()
    {
        switch (currentState)
        {
            case PlayerState.Running:
                // _speedFactor = 1;
                // if (canMove) MovePlayer();
                // MovingAnimations();
                break;
            case PlayerState.Attacking:
                // MovePlayer();
                // _speedFactor = attackSpeedFactor;
                break;
        }
    }
    
    public void SwitchState(PlayerState nextState)=> currentState = nextState;
}
