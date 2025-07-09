using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStaggeredState : EnemyState
{
    float _staggerTime, _timer;

    public EnemyStaggeredState(EnemyController controller, EnemyStateMachine stateMachine, float staggerTime) : base(controller, stateMachine)
    {
        _staggerTime = staggerTime;
    }

    public override void EnterState()
    {
        _timer = 0f;

        _enemy.Stop();
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {
        _timer += Time.deltaTime;

        if (_timer > _staggerTime)
        {
            if (_enemy.HasTarget)
                _stateMachine.ChangeState(_enemy.ChaseState);
            else
                _stateMachine.ChangeState(_enemy.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {

    }

    public override void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType)
    {
        //
    }
}
