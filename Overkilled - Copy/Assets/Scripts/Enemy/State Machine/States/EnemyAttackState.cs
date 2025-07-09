using UnityEngine;

public class EnemyAttackState : EnemyState
{
    float _attackDuration;
    float _durationTimer = 0f;

    public EnemyAttackState(EnemyController controller, EnemyStateMachine stateMachine, float attackDuration) : base(controller, stateMachine)
    {
        _attackDuration = attackDuration;
    }

    public override void EnterState()
    {
        _durationTimer = 0f;
        //start attack anim?
        _enemy.StartAttack();
    }

    public override void ExitState()
    {
        _enemy.StopAttack();
    }

    public override void FrameUpdate()
    {
        _durationTimer += Time.deltaTime;

        if (_durationTimer < _attackDuration)
            return;

        if (_enemy.HasTarget)
            _enemy.StateMachine.ChangeState(_enemy.ChaseState);
        else
            _stateMachine.ChangeState(_enemy.IdleState);
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType)
    {
        if (triggerType != EnemyController.AnimationTriggerType.Attack)
            return;

        //_enemy.Attack();
    }
}
