using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] float _interactDistance = 1f;
    [SerializeField] LayerMask _interactLayerMask;
    IInteractable _hoveredInteract;

    ItemHolder _hand;

    void Start()
    {
        _hand = GetComponent<ItemHolder>();
    }

    void Update()
    {
        CheckInteractable();
    }

    void CheckInteractable()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _interactDistance, _interactLayerMask);
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
        if (_hand.IsHoldingItem)
            _hand.DropItem();
        else if (_hoveredInteract != null )
            _hoveredInteract.Interact(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.forward * _interactDistance);    
    }
}
