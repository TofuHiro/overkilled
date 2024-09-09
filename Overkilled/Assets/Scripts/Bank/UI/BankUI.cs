using TMPro;
using UnityEngine;

public class BankUI : MonoBehaviour
{
    [Tooltip("TMP Text component displaying the balance")]
    [SerializeField] TMP_Text _balanceText;

    void Awake()
    {
        Bank.OnBalanceChange += UpdateUI;
    }

    void OnDestroy()
    {
        Bank.OnBalanceChange -= UpdateUI;
    }

    void UpdateUI()
    {
        _balanceText.text = Bank.Balance.ToString();
    }
}
