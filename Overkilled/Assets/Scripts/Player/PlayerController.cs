using SurvivalGame;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    /// <summary>
    /// The player instance local to the current lobby/game
    /// </summary>
    public static PlayerController LocalInstance { get; private set; }

    /// <summary>
    /// Invoked when a player is spawned into the game
    /// </summary>
    public static event Action<PlayerController> OnPlayerSpawn;

    public delegate void PlayerInputAction();
    /// <summary>
    /// Invoked when the interact key is pressed by the player
    /// </summary>
    public event PlayerInputAction OnPlayerInteractInput;
    /// <summary>
    /// Invoked when the menu button is pressed by the player while in the lobby scene
    /// </summary>
    public event PlayerInputAction OnMenuInput;
    /// <summary>
    /// Invoked when the pause key is pressed by the player
    /// </summary>
    public event PlayerInputAction OnPauseInput;
    /// <summary>
    /// Invoked when the cancel key for UI elements is pressed by the player
    /// </summary>
    public event PlayerInputAction OnUICancelInput;

    PlayerMotor _motor;
    PlayerRotation _rotation;
    PlayerStamina _stamina;
    PlayerInteraction _interaction;
    PlayerHand _hand;
    PlayerVisuals _visuals;
    PlayerCombat _combat;

    PlayerInput _input;
    Camera _camera;

    bool _canMove;
    bool _canSprint = true;
    bool _canAttack;
    bool _canInteract;
    bool _toggleManualLook;
    Plane _groundPlane = new Plane(Vector3.down, 0);

    void Start()
    {
        /*PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId);
        _visuals.SetPlayerModel(playerData.PlayerModelId);*/
    }

    public override void OnNetworkSpawn()
    {
        _canMove = true;
        _canSprint = true;
        _canAttack = true;
        _canInteract = true;

        _input = new PlayerInput();
        _camera = Camera.main;

        _motor = GetComponent<PlayerMotor>();
        _rotation = GetComponent<PlayerRotation>();
        _stamina = GetComponent<PlayerStamina>();
        _interaction = GetComponent<PlayerInteraction>();
        _hand = GetComponent<PlayerHand>();
        _combat = GetComponent<PlayerCombat>();
        _visuals = GetComponentInChildren<PlayerVisuals>();

        _input.Player.Enable();
        _input.Player.Sprint.started += ToggleSprint;
        _input.Player.Sprint.canceled += ToggleSprint;
        _input.Player.Use.performed += Interact;
        _input.Player.Fire.started += Attack;
        _input.Player.Fire.canceled += Attack;
        _input.Player.AltFire.started += SecondaryAttack;
        _input.Player.AltFire.canceled += SecondaryAttack;
        _input.Player.Throw.performed += Throw;
        _input.Player.Craft.performed += Craft;

        _input.UI.Back.performed += Cancel;

        if (LobbyManager.Instance != null)
        {
            _input.LobbyMenu.Enable();
            _input.LobbyMenu.Menu.performed += Menu;
        }

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsWaiting)
                _canMove = false;

            GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
            GameManager.Instance.OnMultiplayerGamePause += GameManager_OnMultiplayerGamePause;
            GameManager.Instance.OnMultiplayerGameUnpause += GameManager_OnMultiplayerGameUnpause;

            _input.GameMenu.Enable();
            _input.GameMenu.Pause.performed += Pause;
        }

        /*_hand.OnWeaponPickUp += PlayerHand_OnWeaponPickUp;
        _hand.OnWeaponDrop += PlayerHand_OnWeaponDrop;*/
        _combat.OnSecondaryAttackStart += PlayerHand_OnSecondaryAttackStart;
        _combat.OnSecondaryAttackStop += PlayerHand_OnSecondaryAttackStop;

        PlayerList.AddPlayer(gameObject);

        if (IsOwner)
        {
            LocalInstance = this;
            OnPlayerSpawn?.Invoke(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        _input.Player.Disable();
        _input.Player.Sprint.started -= ToggleSprint;
        _input.Player.Sprint.canceled -= ToggleSprint;
        _input.Player.Use.performed -= Interact;
        _input.Player.Fire.started -= Attack;
        _input.Player.Fire.canceled -= Attack;
        _input.Player.AltFire.started -= SecondaryAttack;
        _input.Player.AltFire.canceled -= SecondaryAttack;
        _input.Player.Throw.performed -= Throw;
        _input.Player.Craft.performed -= Craft;

        _input.UI.Disable();
        _input.UI.Back.performed -= Cancel;

        if (LobbyManager.Instance != null)
        {
            _input.LobbyMenu.Disable();
            _input.LobbyMenu.Menu.performed -= Menu;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChange -= GameManager_OnGameStateChange;
            GameManager.Instance.OnMultiplayerGamePause -= GameManager_OnMultiplayerGamePause;
            GameManager.Instance.OnMultiplayerGameUnpause -= GameManager_OnMultiplayerGameUnpause;

            _input.GameMenu.Disable();
            _input.GameMenu.Pause.performed += Pause;
        }

        PlayerList.RemovePlayer(gameObject);
    }

    void GameManager_OnGameStateChange()
    {
        if (GameManager.Instance.IsWaiting)
            _canMove = false;
        if (GameManager.Instance.IsStarting)
            _canMove = true;
        else if (GameManager.Instance.GameStarted)
            _canMove = true;
        else if (GameManager.Instance.GameEnded)
            _canMove = false;
    }

    void GameManager_OnMultiplayerGamePause()
    {
        if (GameManager.Instance.IsPaused)
        {
            _canMove = false;
            SetUIControls(true);
        }
    }

    void GameManager_OnMultiplayerGameUnpause()
    {
        if (!GameManager.Instance.IsPaused)
        {
            if (!GameManager.Instance.IsWaiting)
            {
                _canMove = true;
            }

            SetUIControls(false);
        }
    }

    /*void PlayerHand_OnWeaponPickUp()
    {
        _toggleManualLook = true;
        _rotation.ToggleLockRotationSpeed(false);
    }

    void PlayerHand_OnWeaponDrop()
    {
        _toggleManualLook = false;
        _rotation.ToggleLockRotationSpeed(true);
    }*/

    void PlayerHand_OnSecondaryAttackStart(float movementSpeedMultiplier, SecondaryType secondaryType)
    {
        //Aiming
        _toggleManualLook = true;
        _rotation.ToggleLockRotationSpeed(false);

        //Movement debuff
        _motor.AddMovementSpeedMultiplier(movementSpeedMultiplier);
        _canSprint = false;
        _stamina.SetSprint(false);
    }

    void PlayerHand_OnSecondaryAttackStop(float movementSpeedMultiplier, SecondaryType secondaryType)
    {
        //Aiming
        _toggleManualLook = false;
        _rotation.ToggleLockRotationSpeed(true);

        //Movement debuff
        _motor.RemoveMovementSpeedMultiplier(movementSpeedMultiplier);
        _canSprint = true;
    }

    /// <summary>
    /// Set the controls between UI and player controls
    /// </summary>
    /// <param name="state"></param>
    public void SetUIControls(bool state)
    {
        if (state)
        {
            _input.Player.Disable();
            _input.UI.Enable();

            _stamina.SetSprint(false);
            _combat.SetAttackState(false);
            _combat.SetSecondaryAttackState(false);
        }
        else
        {
            _input.Player.Enable();
            _input.UI.Disable();
        }
    }

    public void SetCanMove(bool state) { _canMove = state; }
    public void SetCanAttack(bool state) { _canAttack = state; }
    public void SetCanInteract(bool state) { _canInteract = state; }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    void Update()
    {
        if (!IsOwner)
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
        if (!_canMove)
            return;

        if (_toggleManualLook)
        {
            Vector3 screenPosition = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            if (_groundPlane.Raycast(ray, out float distance))
            {
                Vector3 dir = (ray.GetPoint(distance) - transform.position).normalized;
                _rotation.SetLookDirection(dir);
            }
        }
        else
        {
            Vector2 input = _input.Player.Move.ReadValue<Vector2>();
            _rotation.SetLookDirection(input);
        }
    }

    void ToggleSprint(InputAction.CallbackContext context)
    {
        if (!IsOwner || !_canMove || !_canSprint)
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
            
            if (_canInteract)
                _interaction.Interact();
        }
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (!IsOwner || !_canAttack)
            return;

        if (context.started)
            _combat.SetAttackState(true);
        else if (context.canceled)
            _combat.SetAttackState(false);
    }

    void SecondaryAttack(InputAction.CallbackContext context)
    {
        if (!IsOwner || !_canAttack)
            return;

        if (context.started)
            _combat.SetSecondaryAttackState(true);
        else if (context.canceled)
            _combat.SetSecondaryAttackState(false);
    }

    void Throw(InputAction.CallbackContext context)
    {
        if (!IsOwner || !_canMove)
            return;

        if (context.performed)
            _interaction.Throw();
    }

    void Craft(InputAction.CallbackContext context)
    {
        if (!IsOwner || !_canInteract)
            return;

        if (context.performed)
            _interaction.Craft();
    }

    void Pause(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;

        if (context.performed)
        {
            OnPauseInput?.Invoke();
        }
    }

    #endregion


    #region UI Controls

    void Cancel(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;

        if (context.performed)
        {
            OnUICancelInput?.Invoke();
        }
    }

    #endregion


    #region Lobby Menu Controls

    void Menu(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;

        if (context.performed)
        {
            OnMenuInput?.Invoke();
        }
    }

    #endregion

}
