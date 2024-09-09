using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkButtons : MonoBehaviour
{
    [SerializeField] Button _hostButton;
    [SerializeField] Button _clientButton;

    void Awake()
    {
        _hostButton.onClick.AddListener(() => { GameManager.Instance.StartHost(); });
        _clientButton.onClick.AddListener(() => { GameManager.Instance.StartClient(); });

    }
}
