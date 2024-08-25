using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerStamina : MonoBehaviour
{
    [Tooltip("Maximum stamina (seconds) this player can have")]
    [SerializeField] float _maxStamina = 5f;
    [Tooltip("The time delay required after using stamina before recharging")]
    [SerializeField] float _staminaRegenDelay = 2f;

    public float CurrentStamina
    {
        get { return _currentStamina; }
        set
        {
            _currentStamina = Mathf.Clamp(value, 0f, _maxStamina);

            if (_currentStamina <= 0f)
                SetSprint(false);
        }
    }
    private float _currentStamina;

    PlayerMotor _motor;
    bool _isRegeningStamina, _isSprinting;
    Coroutine _staminaRegenRoutine;

    void Start()
    {
        _motor = GetComponent<PlayerMotor>();

        CurrentStamina = _maxStamina;
    }

    /// <summary>
    /// Set this player's sprint state
    /// </summary>
    /// <param name="state"></param>
    public void SetSprint(bool state)
    {
        _isSprinting = state;
        _motor.SetSprint(state);

        if (state == false)
        {
            _staminaRegenRoutine = StartCoroutine(StartStaminaRegen());
        }
        else
        {
            if (_staminaRegenRoutine != null)
            {
                _isRegeningStamina = false;
                StopCoroutine(_staminaRegenRoutine);
            }
        }
    }

    void Update()
    {
        if (_isSprinting && CurrentStamina > 0f)
            CurrentStamina -= Time.deltaTime;

        if (_isRegeningStamina && CurrentStamina < _maxStamina)
            CurrentStamina += Time.deltaTime;
    }

    IEnumerator StartStaminaRegen()
    {
        yield return new WaitForSeconds(_staminaRegenDelay);

        _isRegeningStamina = true;
    }
}
