using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState 
{
    protected EnemyController _enemy;
    protected EnemyStateMachine _stateMachine;

    public EnemyState(EnemyController controller, EnemyStateMachine stateMachine)
    {
        _enemy = controller;
        _stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType) { }
}
