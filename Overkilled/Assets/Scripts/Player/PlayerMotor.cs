using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [Tooltip("Force applied to rigidbody for movement. Adjust drag to fine tune movement and friction")]
    [SerializeField] float _moveForce = 200f;
    [Tooltip("Multiplier applied to movement when sprinting")]
    [SerializeField] float _sprintMultiplier = 1.4f;
    [Tooltip("The minimum speed debuff multiplier the player can have")]
    [SerializeField] float _maximumSpeedMultiplier = 2f;
    [Tooltip("The minimum speed debuff multiplier the player can have")]
    [SerializeField] float _minimumSpeedMultiplier = .3f;

    List<float> _speedMultipliers;
    Rigidbody _rb;
    Vector2 _currentDir;
    bool _isSprinting;

    void Awake()
    {
        _speedMultipliers = new List<float>();
        _rb = GetComponent<Rigidbody>();
    }

    public void SetDirection(Vector2 dir)
    {
        _currentDir = dir;
    }

    public void SetSprint(bool state)
    {
        _isSprinting = state;
    }

    public void AddMovementSpeedMultiplier(float value)
    {
        _speedMultipliers.Add(value);
    }

    public void RemoveMovementSpeedMultiplier(float value)
    {
        _speedMultipliers.Remove(value);
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        Vector3 moveVel = new Vector3(_currentDir.x, 0f, _currentDir.y).normalized * _moveForce * _rb.mass;

        if (_isSprinting)
            moveVel *= _sprintMultiplier;

        //Speed multipliers
        float totalMultiplier = 1f;
        for (int i = 0; i < _speedMultipliers.Count; i++)
            totalMultiplier += _speedMultipliers[i];

        totalMultiplier = Mathf.Clamp(totalMultiplier, _minimumSpeedMultiplier, _maximumSpeedMultiplier);
        moveVel *= totalMultiplier;

        _rb.AddForce(moveVel, ForceMode.Force);
    }
}
