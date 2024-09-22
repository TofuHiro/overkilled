using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [Tooltip("The position to hold items at")]
    [SerializeField] Transform _holdPosition;
    [Tooltip("The multiplying factor to apply to objects when being held")]
    [SerializeField] float _itemShrinkfactor = 1f;
    [Tooltip("The rotation to set to objects when being held")]
    [SerializeField] Vector3 _lockRotation = Vector3.zero;
    [Tooltip("If is static, when parenting, object will just be placed at hold position once, while if not static, the object will constantly be set to the position")]
    [SerializeField] bool _isStatic = true;

    Transform _itemTransform = null;

    /// <summary>
    /// Whether this holder is currently holding an object
    /// </summary>
    public bool IsOccupied { get; private set; } = false;

    public delegate void ItemHolderAction(Item item);
    public event ItemHolderAction OnItemChange;

    Item _currentItem;
    Rigidbody _currentRigidbody;
    Collider[] _currentColliders;

    /// <summary>
    /// Get the current item thats being held
    /// </summary>
    /// <returns></returns>
    public Item GetItem() { return _currentItem; }

    /// <summary>
    /// The position of where the item is held at
    /// </summary>
    /// <returns></returns>
    public Vector3 GetHoldPosition() { return _holdPosition.position; }

    void Update()
    {
        if (_isStatic)
            return;

        if (IsOccupied)
        {
            _itemTransform.position = _holdPosition.position;
            _itemTransform.rotation = _holdPosition.rotation;
        }
    }

    /// <summary>
    /// Assigns a new item to this holder, locking it into place
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(Item item)
    {
        if (item != null)
        {
            _currentItem = item;
            _currentRigidbody = item.GetComponent<Rigidbody>();
            _currentColliders = item.GetComponents<Collider>();
            _itemTransform = item.transform;
            IsOccupied = true;
            SetLockItem(true);
        }
        else
        {
            SetLockItem(false);
            _currentItem = null;
            _currentRigidbody = null;
            _currentColliders = null;
            _itemTransform = null;
            IsOccupied = false;
        }

        OnItemChange?.Invoke(item);
    }

    public void SetLockItem(bool state)
    {
        _currentRigidbody.isKinematic = state;

        foreach (Collider collider in _currentColliders)
            collider.enabled = !state;
        if (state == true)
        {
            _currentItem.transform.position = _holdPosition.transform.position;
            _currentItem.transform.rotation = Quaternion.Euler(_lockRotation);
            _currentItem.transform.localScale *= _itemShrinkfactor;
        }
        else
        {
            _currentItem.transform.localScale /= _itemShrinkfactor;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_holdPosition.position, Vector3.one / 8f);
    }
}