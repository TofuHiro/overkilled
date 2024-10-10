using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wardrobe : MonoBehaviour, IInteractable
{
    [SerializeField] CharacterCustomizationUI _characterCustomizationUI;
    [SerializeField] Camera _wardrobeCamera;

    public bool IsWardrobeOpen { get { return _characterCustomizationUI.isActiveAndEnabled; } }

    Camera _mainCam;

    void Awake()
    {
        _mainCam = Camera.main;
        _wardrobeCamera.enabled = false;
    }

    void Start()
    {
        LobbyInterface.Instance.OnUICancel += Hide;
        LobbyInterface.Instance.OnMenuToggle += Hide;
    }

    public void Interact(PlayerInteraction player)
    {
        _characterCustomizationUI.Show();

        ToggleWardrobeCamera(true);
        LobbyInterface.Instance.ToggleInterface(true);
    }

    void Hide()
    {
        if (!IsWardrobeOpen)
            return;

        _characterCustomizationUI.Hide();

        ToggleWardrobeCamera(false);
        LobbyInterface.Instance.ToggleInterface(false);
    }

    void ToggleWardrobeCamera(bool state)
    {
        _mainCam.enabled = !state;
        _wardrobeCamera.enabled = state;
    }
}
