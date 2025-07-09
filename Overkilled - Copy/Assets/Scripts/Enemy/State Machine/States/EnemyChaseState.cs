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

        if (!_enemy.InAttackRange)
            _enemy.TargetPlayer();
        else 
        {
            if (!_enemy.IsFacingTarget)
                _enemy.FacePlayer();
            else
                _stateMachine.ChangeState(_enemy.StartAttackState);
        }
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }
}
