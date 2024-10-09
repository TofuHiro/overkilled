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
    public EnemyStaggeredState StaggeredState { get; private set; }
    public EnemyStunnedState StunnedState { get; private set; }

    EnemyHealth _health;
    PlayerRotation _rotation;
    EnemyTargetFinder _targetFinder;
    EnemyMovement _movement;

    void Awake()
    {
        _health = GetComponent<EnemyHealth>();
        _rotation = GetComponent<PlayerRotation>();
        _targetFinder = GetComponent<EnemyTargetFinder>();
        _movement = GetComponent<EnemyMovement>();

        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine, _attackDuration);
        StaggeredState = new EnemyStaggeredState(this, StateMachine, _health.GetStaggerTime());
        StunnedState = new EnemyStunnedState(this, StateMachine);

        StateMachine.Initialize(IdleState);

        _health.OnDamage += Health_OnDamage;
        _health.OnStun += Health_OnStun;
        _health.OnFlatten += Health_OnFlatten;
    }

    void Health_OnDamage(float value)
    {
        StateMachine.ChangeState(StaggeredState);
    }

    void Health_OnStun(float value)
    {
        Debug.Log("Stunned");
        StunnedState.SetStunTime(value);
        StateMachine.ChangeState(StunnedState);
    }
    
    void Health_OnFlatten(float value)
    {
        //Kinematic?
        Debug.Log("Flatten");
    }

    public override void OnNetworkSpawn()
    {
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
    }

    public override void OnNetworkDespawn()
    {
        GameManager.Instance.OnGameStateChange -= GameManager_OnGameStateChange;
    }

    void GameManager_OnGameStateChange()
    {
        if (GameManager.Instance.GameEnded)
        {
            Stop();
            StateMachine.ChangeState(IdleState);
        }
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
        GameObject closestPlayer = _targetFinder.GetClosestPlayer();

        if (closestPlayer == null)
            return false;

        float dist = Vector3.Distance(closestPlayer.transform.position, transform.position);
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
        GameObject closestPlayer = _targetFinder.GetClosestPlayer();
        _movement.SetTarget(closestPlayer ? closestPlayer.transform : null);
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

    public void Stop()
    {
        _movement.Stop();
    }
}
