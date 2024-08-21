using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerRotation))]
[RequireComponent(typeof(PlayerStamina))]
public class PlayerController : MonoBehaviour
{
    PlayerInput _input;
    PlayerMotor _motor;
    PlayerRotation _rotation;
    PlayerStamina _stamina;

    void Awake()
    {
        _input = new PlayerInput();
    }

    void Start()
    {
        _motor = GetComponent<PlayerMotor>();
        _rotation = GetComponent<PlayerRotation>();
        _stamina = GetComponent<PlayerStamina>();
    }

    void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Sprint.started += ToggleSprint;
        _input.Player.Sprint.canceled += ToggleSprint;
    }

    void OnDisable()
    {
        _input.Player.Disable();
        _input.Player.Sprint.started -= ToggleSprint;
        _input.Player.Sprint.canceled -= ToggleSprint;
    }

    void Update()
    {
        Movement();
        Rotation();
    }

    void Movement()
    {
        Vector2 input = _input.Player.Move.ReadValue<Vector2>();
        _motor.SetDirection(input);
    } 

    void Rotation()
    {
        Vector2 input = _input.Player.Move.ReadValue<Vector2>();
        _rotation.SetLookDirection(input);
    }

    void ToggleSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _stamina.SetSprint(true);
        }
        else if (context.canceled)
        {
            _stamina.SetSprint(false);
        }
    }
}
