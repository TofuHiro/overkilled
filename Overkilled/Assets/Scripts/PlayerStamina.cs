using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerStamina : MonoBehaviour
{
    [SerializeField] float _maxStamina = 5f;
    [SerializeField] float _staminaRegenDelay = 2f;

    public float CurrentStamina
    {
        get { return _currentStamina; }
        set
        {
            _currentStamina = value;
            Mathf.Clamp(_currentStamina, 0f, _maxStamina);

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
