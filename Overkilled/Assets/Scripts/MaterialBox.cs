using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialBox : CounterTop
{
    [SerializeField] GameObject _materialPrefab;

    protected override void TakeFromEmptyCounter(PlayerHand hand)
    {
        Item item = Instantiate(_materialPrefab).GetComponent<Item>();
        hand.SetItem(item);
    }
}
