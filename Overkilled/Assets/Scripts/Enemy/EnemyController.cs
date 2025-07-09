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
    [Tooltip("The time delay before performing an attack")]
    [SerializeField] float _attackDelay = .5f;

    /// <summary>
    /// Whether if this enemy currently has a target or not
    /// </summary>
    public bool HasTarget { get { return _targetFinder.GetClosestPlayer() != null; } }
    public bool InAttackRange { get { return CheckCanAttack(); } }
    public bool IsFacingTarget { get { return CheckIsFacingTarget(); } }
    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyStartAttackState StartAttackState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyStaggeredState StaggeredState { get; private set; }
    public EnemyStunnedState StunnedState { get; private set; }

    EnemyHealth _health;
    PlayerRotation _rotation;
    EnemyTargetFinder _targetFinder;
    EnemyMovement _movement;
    PlayerCombat _combat;

    void Awake()
    {
        _health = GetComponent<EnemyHealth>();
        _rotation = GetComponent<PlayerRotation>();
        _targetFinder = GetComponent<EnemyTargetFinder>();
        _movement = GetComponent<EnemyMovement>();
        _combat = GetComponent<PlayerCombat>();

        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        StartAttackState = new EnemyStartAttackState(this, StateMachine, _attackDelay);
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
    bool CheckCanAttack()
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

    bool CheckIsFacingTarget()
    {
        GameObject closestPlayer = _targetFinder.GetClosestPlayer();

        if (closestPlayer == null)
            return false;

        Vector3 dirFromTarget = (closestPlayer.transform.position - transform.position).normalized;
        float dotProd = Vector3.Dot(dirFromTarget, transform.forward);

        if (dotProd > 0.9)
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

    public void FacePlayer()
    {
        GameObject closestPlayer = _targetFinder.GetClosestPlayer();
        Vector3 dirFromTarget = (closestPlayer.transform.position - transform.position).normalized;
        _rotation.SetLookDirection(new Vector2(dirFromTarget.x, dirFromTarget.z));
    }

    public void StartAttack()
    {
        _combat.SetAttackState(true);
    }

    public void StopAttack()
    {
        _combat.SetAttackState(false);
    }

    public void Stop()
    {
        _movement.Stop();
    }
}
