using TMPro;
using UnityEngine;

public class BankUI : MonoBehaviour
{
    [Tooltip("TMP Text component displaying the balance")]
    [SerializeField] TMP_Text _balanceText;

    void Start()
    {
        Bank.Instance.OnBalanceChange += UpdateUI;

        _balanceText.text = "0";
    }

    void UpdateUI(int balance)
    {
        _balanceText.text = balance.ToString();
    }
}
