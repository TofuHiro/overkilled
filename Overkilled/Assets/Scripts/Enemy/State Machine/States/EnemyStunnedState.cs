using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunnedState : EnemyState
{
    float _stunTime, _timer;

    public EnemyStunnedState(EnemyController controller, EnemyStateMachine stateMachine) : base(controller, stateMachine)
    {

    }

    public void SetStunTime(float time)
    {
        _stunTime = time;
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

        if (_timer > _stunTime)
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

    }
}
