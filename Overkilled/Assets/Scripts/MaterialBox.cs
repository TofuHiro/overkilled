using Unity.Netcode;
using UnityEngine;

public class MaterialBox : CounterTop
{
    [Tooltip("The source object to spawn from this box")]
    [SerializeField] GameObject _materialPrefab;

    MultiplayerManager _multiplayerManager;

    protected override void Start()
    {
        base.Start();

        _multiplayerManager = MultiplayerManager.Instance;
    }

    protected override void TakeFromEmptyCounter(PlayerHand hand)
    {
        _multiplayerManager.SpawnObject(_materialPrefab, hand);
    }
}
