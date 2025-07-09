using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    [Tooltip("Maximum distance player can interact within")]
    [SerializeField] float _interactDistance = 1.3f;
    [Tooltip("Maximum distance player can interact within")]
    [SerializeField] float _craftDistance = 1.3f;
    [Tooltip("Radius to poll at the interact distance")]
    [SerializeField] float _interactRadius = 1f;
    [Tooltip("Layer mask to use when player is not holding anything")]
    [SerializeField] LayerMask _unarmedLayerMask;
    [Tooltip("Layer mask to use when player is holding something")]
    [SerializeField] LayerMask _armedLayerMask;
    [Tooltip("Layer mask of counters")]
    [SerializeField] LayerMask _counterMask;

    /// <summary>
    /// Whether this player is holding an item
    /// </summary>
    public bool IsHoldingItem { get { return _hand.IsHoldingItem; } }

    IInteractable _hoveredInteract;
    HighlightItem _hoveredHighlightItem;
    PlayerHand _hand;

    /// <summary>
    /// Get the hand reference for this player
    /// </summary>
    /// <returns></returns>
    public PlayerHand GetHand() { return _hand; }

    void Awake()
    {
        _hand = GetComponent<PlayerHand>();
    }

    void Update()
    {
        if (!IsOwner)
            return;

        CheckInteractable();
    }

    void CheckInteractable()
    {
        Physics.Raycast(transform.position + (Vector3.up * 0.5f), transform.forward, out RaycastHit hit, _interactDistance, IsHoldingItem ? _armedLayerMask : _unarmedLayerMask);
        if (hit.transform != null)
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();

            if (interactable == _hoveredInteract)
                return;

            //Was looking at another item before
            if (_hoveredHighlightItem != null)
                _hoveredHighlightItem.SetHighlight(false);

            if (interactable != null)
            {
                _hoveredInteract = interactable;

                //Highlight
                _hoveredHighlightItem = hit.transform.GetComponent<HighlightItem>();
                if (_hoveredHighlightItem != null)
                    _hoveredHighlightItem.SetHighlight(true);
            }
            else
            {
                _hoveredInteract = null;
                _hoveredHighlightItem = null;
            }
        }
        else
        {
            //Was looking at another item before
            if (_hoveredHighlightItem != null)
                _hoveredHighlightItem.SetHighlight(false);

            _hoveredInteract = null;
            _hoveredHighlightItem = null;
        }
    }

    /// <summary>
    /// Perform interaction action
    /// </summary>
    public void Interact()
    {
        if (_hoveredInteract != null && IsHoldingItem)
            _hoveredInteract.Interact(this);
        else if (IsHoldingItem)
            _hand.DropItem();
        else if (_hoveredInteract != null)
            _hoveredInteract.Interact(this);
    }

    public void Throw()
    {
        if (IsHoldingItem)
            _hand.ThrowItem();
    }

    public void Craft()
    {
        CraftingStation station = CheckCraftingTable();
        if (station != null)
        {
            station.Craft();
        }
    }

    CraftingStation CheckCraftingTable()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _craftDistance, _counterMask);
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
