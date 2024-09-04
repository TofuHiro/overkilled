using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(PlayerRotation))]
[RequireComponent(typeof(EnemyTargetFinder))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemyController : NetworkBehaviour
{
    public enum AnimationTriggerType
    {
        Attack,
        Hurt,
        Stunned,
    }

    [Tooltip("Distance from a player to start attacking")]
    [SerializeField] float _attackDistance = 1.5f;
    [Tooltip("The duration of an attack")]
    [SerializeField] float _attackDuration = 1f;
    [Tooltip("The weapon this enemy starts with")]
    [SerializeField] Weapon _weapon;

    public bool HasTarget { get { return _targetFinder.ClosestPlayer != null; } }

    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }

    EnemyHealth _health;
    PlayerRotation _rotation;
    EnemyTargetFinder _targetFinder;
    EnemyMovement _movement;
    NetworkObject _networkObject;

    void Awake()
    {
        _health = GetComponent<EnemyHealth>();
        _rotation = GetComponent<PlayerRotation>();
        _targetFinder = GetComponent<EnemyTargetFinder>();
        _movement = GetComponent<EnemyMovement>();
        _networkObject = GetComponent<NetworkObject>();

        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine, _attackDuration);

        StateMachine.Initialize(IdleState);
    }

    void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();    
    }

    public void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }

    public bool CheckCanAttack()
    {
        float dist = Vector3.Distance(_targetFinder.ClosestPlayer.transform.position, transform.position);
        if (dist <= _attackDistance)
            return true;
        else
            return false;
    }

    public void TargetPlayer()
    {
        _movement.SetTarget(_targetFinder.ClosestPlayer.transform);
        _rotation.SetLookDirection(_movement.GetDirection());
    }

    public void Attack()
    {
        _movement.Stop();
        _weapon.Attack();
    }

    public NetworkObject GetNetworkObject()
    {
        return _networkObject;
    }
}
