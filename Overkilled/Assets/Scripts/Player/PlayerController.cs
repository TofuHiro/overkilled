using SurvivalGame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerRotation))]
[RequireComponent(typeof(PlayerStamina))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerHand))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : NetworkBehaviour
{
    public static PlayerController LocalInstance { get; private set; }

    public delegate void PlayerInputAction();
    public event PlayerInputAction OnPlayerInteractInput;
    public event PlayerInputAction OnPlayerPauseInput;

    PlayerMotor _motor;
    PlayerRotation _rotation;
    PlayerStamina _stamina;
    PlayerInteraction _interaction;
    PlayerVisuals _visuals;

    PlayerInput _input;

    bool _canMove;

    public override void OnNetworkSpawn()
    {
        PlayerList.AddPlayer(gameObject);

        _canMove = false;

        _input.Player.Enable();
        _input.Player.Sprint.started += ToggleSprint;
        _input.Player.Sprint.canceled += ToggleSprint;
        _input.Player.Use.performed += Interact;
        _input.Player.Fire.started += Attack;
        _input.Player.Fire.canceled += Attack;
        _input.Player.AltFire.started += SecondaryAttack;
        _input.Player.AltFire.canceled += SecondaryAttack;
        _input.Player.Pause.performed += Pause;

        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
        GameManager.Instance.OnLocalGamePause += GameManager_OnLocalGamePause;
        GameManager.Instance.OnLocalGameUnpause += GameManager_OnLocalGameUnpause;

        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId);
        _visuals.SetPlayerModel(playerData.PlayerModelId);
    }

    public override void OnNetworkDespawn()
    {
        PlayerList.RemovePlayer(gameObject);

        _input.Player.Disable();
        _input.Player.Sprint.started -= ToggleSprint;
        _input.Player.Sprint.canceled -= ToggleSprint;
        _input.Player.Use.performed -= Interact;
        _input.Player.Fire.started -= Attack;
        _input.Player.Fire.canceled -= Attack;
        _input.Player.AltFire.started -= SecondaryAttack;
        _input.Player.AltFire.canceled -= SecondaryAttack;
        _input.Player.Pause.performed -= Pause;

        GameManager.Instance.OnGameStateChange -= GameManager_OnGameStateChange;
        GameManager.Instance.OnLocalGamePause -= GameManager_OnLocalGamePause;
        GameManager.Instance.OnLocalGameUnpause -= GameManager_OnLocalGameUnpause;
    }

    void Awake()
    {
        _input = new PlayerInput();

        if (LocalInstance != null && LocalInstance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Player Controller found. Destroying " + name);
            Destroy(LocalInstance);
        }

        LocalInstance = this;

        _motor = GetComponent<PlayerMotor>();
        _rotation = GetComponent<PlayerRotation>();
        _stamina = GetComponent<PlayerStamina>();
        _interaction = GetComponent<PlayerInteraction>();
        _visuals = GetComponentInChildren<PlayerVisuals>();
    }

    void GameManager_OnGameStateChange()
    {
        if (GameManager.Instance.GameEnded)
        {
            _canMove = false;
        }
        else if (GameManager.Instance.GameStarted)
        {
            _canMove = true;
        }
    }
    
    void GameManager_OnLocalGamePause()
    {
        if (GameManager.Instance.IsPaused)
        {
            _canMove = false;
        }
    }

    void GameManager_OnLocalGameUnpause()
    {
        if (!GameManager.Instance.IsPaused)
        {
            _canMove = true;
        }
    }

    void Update()
    {
        if (!IsOwner) 
            return;

        Movement();
        Rotation();
    }

    void Movement()
    {
        if (_canMove)
        {
            Vector2 input = _input.Player.Move.ReadValue<Vector2>();
            _motor.SetDirection(input);
        }
        else
        {
            _motor.SetDirection(Vector2.zero);
        }
    } 

    void Rotation()
    {
        if (_canMove)
        {
            Vector2 input = _input.Player.Move.ReadValue<Vector2>();
            _rotation.SetLookDirection(input);
        }
        else
        {
            _rotation.SetLookDirection(Vector2.zero);
        }
    }

    void ToggleSprint(InputAction.CallbackContext context)
    {
        if (!IsOwner || !_canMove)
            return;

        if (context.started)
            _stamina.SetSprint(true);
        else if (context.canceled)
            _stamina.SetSprint(false);
    }

    void Interact(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;

        if (context.performed)
        {
            OnPlayerInteractInput?.Invoke();

            if (_canMove)
                _interaction.Interact();
        }
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;

        if (_canMove)
        {
            if (context.started)
                _interaction.SetAttackState(true);
            else if (context.canceled)
                _interaction.SetAttackState(false);
        }
        else
        {
            _interaction.SetAttackState(false);
        }
    }

    void SecondaryAttack(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;

        if (_canMove)
        {
            if (context.started)
                _interaction.SetSecondaryAttackState(true);
            else if (context.canceled)
                _interaction.SetSecondaryAttackState(false);
        }
        else
        {
            _interaction.SetSecondaryAttackState(false);
        }
    }

    void Pause(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;

        OnPlayerPauseInput?.Invoke();
    }
}
