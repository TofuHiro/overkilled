using UnityEngine;

public class MaterialBox : CounterTop
{
    [Tooltip("The source object to spawn from this box")]
    [SerializeField] GameObject _materialPrefab;

    protected override void TakeFromEmptyCounter(PlayerHand hand)
    {
        Item item = Instantiate(_materialPrefab).GetComponent<Item>();
        hand.SetItem(item);
    }
}
