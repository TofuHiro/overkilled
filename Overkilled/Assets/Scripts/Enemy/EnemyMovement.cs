using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Tooltip("Max speed of the navmesh")]
    [SerializeField] float _speed = 3f;
    [Tooltip("Max acceleration of the navmesh")]
    [SerializeField] float _acceleration = 3f;
    [Tooltip("Limits to how often a path to the target is drawn")]
    [SerializeField] float _pathDrawRate = .3f;

    NavMeshAgent _navAgent;
    float _timer = 0f;

    void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.speed = _speed;
        _navAgent.acceleration = _acceleration;
    }

    void Update()
    {
        _timer += Time.deltaTime;
    }

    /// <summary>
    /// Get the current direction the navmesh is moving towards
    /// </summary>
    /// <returns></returns>
    public Vector2 GetDirection()
    {
        return new Vector2(_navAgent.velocity.x, _navAgent.velocity.z).normalized;
    }

    /// <summary>
    /// Set the navmesh destination to a target's position
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        if (_timer < _pathDrawRate)
            return;

        _navAgent.isStopped = false;
        _navAgent.SetDestination(target.position);
        _timer = 0f;
    }

    public void Stop()
    {
        _navAgent.isStopped = true;
    }
}
