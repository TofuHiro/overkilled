using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField] float _moveForce = 200f;
    [SerializeField] float _sprintMultiplier = 1.4f;
     
    Rigidbody _rb;
    Vector2 _currentDir;
    bool _isSprinting;

    void Start()
    {
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

    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        Vector3 moveVel = new Vector3(_currentDir.x, 0f, _currentDir.y).normalized * _moveForce * _rb.mass;

        if (_isSprinting)
            moveVel *= _sprintMultiplier;

        _rb.AddForce(moveVel, ForceMode.Force);
    }
}
