public abstract class EnemyState 
{
    protected EnemyController _enemy;
    protected EnemyStateMachine _stateMachine;

    public EnemyState(EnemyController controller, EnemyStateMachine stateMachine)
    {
        _enemy = controller;
        _stateMachine = stateMachine;
    }

    /// <summary>
    /// Called when entering a state
    /// </summary>
    public virtual void EnterState() { }
    /// <summary>
    /// Called when exiting a state
    /// </summary>
    public virtual void ExitState() { }
    /// <summary>
    /// Called once per update loop
    /// </summary>
    public virtual void FrameUpdate() { }
    /// <summary>
    /// Called once per fixed update loop
    /// </summary>
    public virtual void PhysicsUpdate() { }
    /// <summary>
    /// Called through animation trigger event that is called on the EnemyController class
    /// </summary>
    /// <param name="triggerType"></param>
    public virtual void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType) { }
}
