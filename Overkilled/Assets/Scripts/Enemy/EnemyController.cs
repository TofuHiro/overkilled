using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using SurvivalGame;

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

    /// <summary>
    /// Whether if this enemy currently has a target or not
    /// </summary>
    public bool HasTarget { get { return _targetFinder.GetClosestPlayer() != null; } }

    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }

    PlayerRotation _rotation;
    EnemyTargetFinder _targetFinder;
    EnemyMovement _movement;

    void Awake()
    {
        _rotation = GetComponent<PlayerRotation>();
        _targetFinder = GetComponent<EnemyTargetFinder>();
        _movement = GetComponent<EnemyMovement>();

        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine, _attackDuration);

        StateMachine.Initialize(IdleState);
    }

    public override void OnNetworkSpawn()
    {
        GameManager.Instance.OnGameStateChange += Stop;    
    }

    public override void OnNetworkDespawn()
    {
        GameManager.Instance.OnGameStateChange -= Stop;
    }

    void Update()
    {
        if (GameManager.Instance.GameEnded)
            return;

        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.GameEnded)
            return;

        StateMachine.CurrentEnemyState.PhysicsUpdate();    
    }

    public void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }

    /// <summary>
    /// Check if this enemy is within range to attack or not
    /// </summary>
    /// <returns>Returns true if this enemy is within range to perform an attack</returns>
    public bool CheckCanAttack()
    {
        float dist = Vector3.Distance(_targetFinder.GetClosestPlayer().transform.position, transform.position);
        if (dist <= _attackDistance)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Set this enemy to target the closest player and move towards it
    /// </summary>
    public void TargetPlayer()
    {
        _movement.SetTarget(_targetFinder.GetClosestPlayer().transform);
        _rotation.SetLookDirection(_movement.GetDirection());
    }

    /// <summary>
    /// Trigger an attack
    /// </summary>
    public void Attack()
    {
        _movement.Stop();
        _weapon.Attack();
    }

    void Stop()
    {
        if (GameManager.Instance.GameEnded)
        {
            _movement.Stop();
            StateMachine.ChangeState(IdleState);
        }
    }
}
