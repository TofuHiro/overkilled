using System.Collections;
using System.Collections.Generic;
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
        base.EnterState();

        _timer = 0f;
        //start attack anim
        _enemy.Attack();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        _timer += Time.deltaTime;

        if (_timer > _attackDuration)
            _enemy.StateMachine.ChangeState(_enemy.ChaseState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);

        if (triggerType != EnemyController.AnimationTriggerType.Attack)
            return;

        _enemy.Attack();
    }
}
