using UnityEngine;

public class EnemyAttackState : EnemyState
{
    float _attackDuration;
    float _timer = 0f;

    public EnemyAttackState(EnemyController controller, EnemyStateMachine stateMachine, float attackDuration) : base(controller, stateMachine)
    {
        _attackDuration = attackDuration;
    }

    public override void EnterState()
    {
        _timer = 0f;
        //start attack anim?
        _enemy.Attack();
    }

    public override void ExitState()
    {
        
    }

    public override void FrameUpdate()
    {
        if (!_enemy.HasTarget)
            _stateMachine.ChangeState(_enemy.IdleState);

        _timer += Time.deltaTime;

        if (_timer > _attackDuration)
            _enemy.StateMachine.ChangeState(_enemy.ChaseState);
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType)
    {
        if (triggerType != EnemyController.AnimationTriggerType.Attack)
            return;

        _enemy.Attack();
    }
}
