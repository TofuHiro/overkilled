using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerStamina))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(ItemHolder))]
public class PlayerController : MonoBehaviour
{
    PlayerMotor _motor;
    PlayerStamina _stamina;
    PlayerInteraction _interaction;
    ItemHolder _hands;

    PlayerInput _input;

    void Awake()
    {
        _input = new PlayerInput();
    }

    void Start()
    {
        _motor = GetComponent<PlayerMotor>();
        _stamina = GetComponent<PlayerStamina>();
        _interaction = GetComponent<PlayerInteraction>();
        _hands = GetComponent<ItemHolder>();
    }

    void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Sprint.started += ToggleSprint;
        _input.Player.Sprint.canceled += ToggleSprint;
        _input.Player.Use.performed += Interact;

    }

    void OnDisable()
    {
        _input.Player.Disable();
        _input.Player.Sprint.started -= ToggleSprint;
        _input.Player.Sprint.canceled -= ToggleSprint;
        _input.Player.Use.performed -= Interact;

    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        Vector2 input = _input.Player.Move.ReadValue<Vector2>();
        _motor.SetDirection(input);
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

    void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _interaction.Interact();
        }
    }
}
