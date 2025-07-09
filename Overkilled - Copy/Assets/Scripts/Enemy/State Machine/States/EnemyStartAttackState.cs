using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStartAttackState : EnemyState
{
    float _attackDelay;
    float _delayTimer = 0f;

    public EnemyStartAttackState(EnemyController controller, EnemyStateMachine stateMachine, float attackDelay) : base(controller, stateMachine)
    {
        _attackDelay = attackDelay;
    }

    public override void EnterState()
    {
        _enemy.Stop();
        _delayTimer = 0f;
    }

    public override void ExitState()
    {
        
    }

    public override void FrameUpdate()
    {
        _delayTimer += Time.deltaTime;

        if (_delayTimer < _attackDelay)
            return;

        _stateMachine.ChangeState(_enemy.AttackState);
    }

    public override void PhysicsUpdate()
    {

    }

    public override void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType)
    {
        
    }
}
