using SurvivalGame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerRotation))]
[RequireComponent(typeof(PlayerStamina))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerHand))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController LocalInstance { get; private set; }

    public delegate void PlayerReferenceSwitch(PlayerController prevRef, PlayerController newRef);
    public static event PlayerReferenceSwitch OnSingletonSwitch;

    public delegate void PlayerInputAction();
    public event PlayerInputAction OnPlayerInteractInput;
    public event PlayerInputAction OnPlayerPauseInput;
    public event PlayerInputAction OnUICancelInput;

    PlayerMotor _motor;
    PlayerRotation _rotation;
    PlayerStamina _stamina;
    PlayerInteraction _interaction;
    PlayerVisuals _visuals;

    PlayerInput _input;

    bool _canMove, _canControl;

    void Awake()
    {
        _input = new PlayerInput();

        _motor = GetComponent<PlayerMotor>();
        _rotation = GetComponent<PlayerRotation>();
        _stamina = GetComponent<PlayerStamina>();
        _interaction = GetComponent<PlayerInteraction>();
        _visuals = GetComponentInChildren<PlayerVisuals>();

        if (GetIsOffline())
        {
            if (LocalInstance != null && LocalInstance != this)
            {
                Debug.LogWarning("Warning. Multiple instances of Player Controller found");
            }

            LocalInstance = this;
        }
    }

    void Start()
    {
        _canMove = true;
        _canControl = true;

        /*PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId);
        _visuals.SetPlayerModel(playerData.PlayerModelId);*/

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
            GameManager.Instance.OnLocalGamePause += GameManager_OnLocalGamePause;
            GameManager.Instance.OnLocalGameUnpause += GameManager_OnLocalGameUnpause;
        }
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
        _input.Player.Pause.performed += Pause;

        _input.UI.Cancel.performed += Cancel;
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
        _input.Player.Pause.performed -= Pause;

        _input.UI.Disable();
        _input.UI.Cancel.performed -= Cancel;
    }

    void OnDestroy()
    {
        _input.Player.Disable();
        _input.Player.Sprint.started -= ToggleSprint;
        _input.Player.Sprint.canceled -= ToggleSprint;
        _input.Player.Use.performed -= Interact;
        _input.Player.Fire.started -= Attack;
        _input.Player.Fire.canceled -= Attack;
        _input.Player.AltFire.started -= SecondaryAttack;
        _input.Player.AltFire.canceled -= SecondaryAttack;
        _input.Player.Pause.performed -= Pause;

        _input.UI.Disable();
        _input.UI.Cancel.performed -= Cancel;
    }

    public void InitSingleton(PlayerController instance)
    {
        OnSingletonSwitch?.Invoke(LocalInstance, instance);

        LocalInstance = instance;
    }

    bool GetIsOffline()
    {
        return (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsClient) || NetworkManager.Singleton == null;
    }

    void GameManager_OnGameStateChange()
    {
        if (GameManager.Instance.IsWaiting)
            _canMove = false;
        else if (GameManager.Instance.GameStarted)
            _canMove = true;
        else if (GameManager.Instance.GameEnded)
            _canMove = false;
    }

    void GameManager_OnLocalGamePause()
    {
        if (GameManager.Instance.IsPaused)
            _canMove = false;
    }

    void GameManager_OnLocalGameUnpause()
    {
        if (!GameManager.Instance.IsPaused)
            _canMove = true;
    }

    public void SetUIControls(bool state)
    {
        if (state)
        {
            _input.Player.Disable();
            _input.UI.Enable();

            _stamina.SetSprint(false);
            _interaction.SetAttackState(false);
            _interaction.SetSecondaryAttackState(false);
        }
        else
        {
            _input.Player.Enable();
            _input.UI.Disable();
        }
    }

    public void SetCanMove(bool state)
    {
        _canMove = state;
    }

    public void SetCanControl(bool state)
    {
        _canControl = state;
    }

    void Update()
    {
        if (!_canControl)
            return;

        Movement();
        Rotation();
    }

    #region Player Controls

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
        if (!_canControl || !_canMove)
            return;

        if (context.started)
            _stamina.SetSprint(true);
        else if (context.canceled)
            _stamina.SetSprint(false);
    }

    void Interact(InputAction.CallbackContext context)
    {
        if (!_canControl)
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
        if (!_canControl || !_canMove)
            return;

        if (context.started)
            _interaction.SetAttackState(true);
        else if (context.canceled)
            _interaction.SetAttackState(false);
    }

    void SecondaryAttack(InputAction.CallbackContext context)
    {
        if (!_canControl || !_canMove)
            return;

        if (context.started)
            _interaction.SetSecondaryAttackState(true);
        else if (context.canceled)
            _interaction.SetSecondaryAttackState(false);
    }

    void Pause(InputAction.CallbackContext context)
    {
        if (!_canControl)
            return;

        if (context.performed)
        {
            OnPlayerPauseInput?.Invoke();
        }
    }

    #endregion

    #region UI Controls

    void Cancel(InputAction.CallbackContext context)
    {
        if (!_canControl)
            return;

        if (context.performed)
        {
            OnUICancelInput?.Invoke();
        }
    }

    #endregion
}
