using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerRotation))]
[RequireComponent(typeof(PlayerStamina))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerHand))]
public class PlayerController : MonoBehaviour
{
    PlayerMotor _motor;
    PlayerRotation _rotation;
    PlayerStamina _stamina;
    PlayerInteraction _interaction;

    PlayerInput _input;

    void Awake()
    {
        _input = new PlayerInput();
    }

    void Start()
    {
        _motor = GetComponent<PlayerMotor>();
        _rotation = GetComponent<PlayerRotation>();
        _stamina = GetComponent<PlayerStamina>();
        _interaction = GetComponent<PlayerInteraction>();
    }

    void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Sprint.started += ToggleSprint;
        _input.Player.Sprint.canceled += ToggleSprint;
        _input.Player.Use.performed += Interact;
        _input.Player.Fire.started += Attack;
        _input.Player.Fire.canceled += Attack;
        _input.Player.AltFire.started += SecondaryAttack;
        _input.Player.AltFire.canceled += SecondaryAttack;
    }

    void OnDisable()
    {
        _input.Player.Disable();
        _input.Player.Sprint.started -= ToggleSprint;
        _input.Player.Sprint.canceled -= ToggleSprint;
        _input.Player.Use.performed -= Interact;
        _input.Player.Fire.started -= Attack;
        _input.Player.Fire.canceled -= Attack;
        _input.Player.AltFire.started -= SecondaryAttack;
        _input.Player.AltFire.canceled -= SecondaryAttack;
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

    void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _interaction.Interact();
        }
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (context.started)
            _interaction.SetAttackState(true);
        else if (context.canceled)
            _interaction.SetAttackState(false);
    }

    void SecondaryAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            _interaction.SetSecondaryAttackState(true);
        else if (context.canceled)
            _interaction.SetSecondaryAttackState(false);
    }
}
