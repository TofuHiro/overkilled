public class EnemyStateMachine 
{
    /// <summary>
    /// The current state the enemy is in
    /// </summary>
    public EnemyState CurrentEnemyState { get; private set; }

    /// <summary>
    /// Initialize this state machine to an initial state
    /// </summary>
    /// <param name="startingState"></param>
    public void Initialize(EnemyState startingState)
    {
        CurrentEnemyState = startingState;
        CurrentEnemyState.EnterState();
    }

    /// <summary>
    /// Change the state of this state machine to a given state
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(EnemyState newState)
    {
        CurrentEnemyState.ExitState();
        CurrentEnemyState = newState;
        CurrentEnemyState.EnterState();
    }
}
