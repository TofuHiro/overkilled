using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] float _interactDistance = 1f;
    [SerializeField] LayerMask _unarmedLayerMask;
    [SerializeField] LayerMask _armedLayerMask;
    [SerializeField] LayerMask _craftingTableMask;

    IInteractable _hoveredInteract;
    PlayerHand _hand;

    void Start()
    {
        _hand = GetComponent<PlayerHand>();
    }

    void Update()
    {
        CheckInteractable();
    }

    void CheckInteractable()
    {
        Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, _interactDistance, _hand.IsHoldingItem ? _armedLayerMask : _unarmedLayerMask);
        if (hit.transform != null)
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable == _hoveredInteract)
                return;
            else if (interactable != null)
                _hoveredInteract = interactable;
            else
                _hoveredInteract = null;
        }
        else
        {
            _hoveredInteract = null;
        }
    }

    public void Interact()
    {
        if (_hoveredInteract != null && _hand.IsHoldingItem)
            _hoveredInteract.Interact(this);
        else if (_hand.IsHoldingItem)
            _hand.DropItem();
        else if (_hoveredInteract != null)
            _hoveredInteract.Interact(this);
    }

    public void SetAttackState(bool state)
    {
        if (_hand.IsHoldingItem)
            _hand.SetAttackState(state);
        else
            if (state)
                TryCraft();
    }

    public void SetSecondaryAttackState(bool state)
    {
        if (_hand.IsHoldingItem)
            _hand.SetSecondaryAttackState(state);
    }

    void TryCraft()
    {
        CraftingStation station = CheckCraftingTable();
        if (station != null)
        {
            station.Craft();
        }
    }

    CraftingStation CheckCraftingTable()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _interactDistance, _craftingTableMask);
        if (hit.transform != null)
        {
            return hit.transform.GetComponent<CraftingStation>();
        }
        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * _interactDistance);    
    }
}
