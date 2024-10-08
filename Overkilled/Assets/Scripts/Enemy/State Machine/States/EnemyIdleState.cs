public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController controller, EnemyStateMachine stateMachine) : base(controller, stateMachine)
    {

    }

    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void FrameUpdate()
    {
        if (_enemy.HasTarget)
            _stateMachine.ChangeState(_enemy.ChaseState);
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType)
    {
        
    }
}
