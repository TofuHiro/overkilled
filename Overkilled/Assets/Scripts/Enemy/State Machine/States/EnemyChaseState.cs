public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyController controller, EnemyStateMachine stateMachine) : base(controller, stateMachine)
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
        if (!_enemy.HasTarget)
            _stateMachine.ChangeState(_enemy.IdleState);

        _enemy.TargetPlayer();

        if (_enemy.CheckCanAttack())
            _stateMachine.ChangeState(_enemy.AttackState);
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }
}
