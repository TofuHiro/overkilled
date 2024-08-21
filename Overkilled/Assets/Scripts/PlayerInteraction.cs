using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] float _interactDistance = 1f;
    [SerializeField] LayerMask _itemLayerMask;

    LayerMask _defaultMask = 1 << 0;
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
        Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, _interactDistance, _hand.IsHoldingItem ? _defaultMask : _itemLayerMask);
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.forward * _interactDistance);    
    }
}
