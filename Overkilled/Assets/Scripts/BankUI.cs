using TMPro;
using UnityEngine;

public class BankUI : MonoBehaviour
{
    [SerializeField] TMP_Text _balanceText;

    Bank _bank;

    void Start()
    {
        _bank = Bank.Instance;

        _bank.OnBalanceChange += UpdateUI;
    }

    void OnDisable()
    {
        _bank.OnBalanceChange -= UpdateUI;
    }

    void UpdateUI()
    {
        _balanceText.text = _bank.Balance.ToString();
    }
}
