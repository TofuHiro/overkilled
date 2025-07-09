using System;
using TMPro;
using UnityEngine;

public class BankUI : MonoBehaviour
{
    [Tooltip("TMP Text component displaying the balance")]
    [SerializeField] TMP_Text _balanceText;

    void Start()
    {
        Bank.Instance.OnBalanceChange += Bank_OnBalanceChange;

        _balanceText.text = "0";
    }

    void Bank_OnBalanceChange(int previousValue, int newValue)
    {
        _balanceText.text = newValue.ToString();
    }
}
