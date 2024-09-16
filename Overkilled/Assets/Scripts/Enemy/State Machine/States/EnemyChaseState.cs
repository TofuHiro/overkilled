public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyController controller, EnemyStateMachine stateMachine) : base(controller, stateMachine)
    {

    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (!_enemy.HasTarget)
            _stateMachine.ChangeState(_enemy.IdleState);

        _enemy.TargetPlayer();

        if (_enemy.CheckCanAttack())
            _stateMachine.ChangeState(_enemy.AttackState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }
}
