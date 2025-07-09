using Unity.Netcode;
using UnityEngine;

public class MaterialBox : CounterTop
{
    [Tooltip("The source objects to spawn from this box")]
    [SerializeField] GameObject[] _materialPrefabs;

    MultiplayerManager _multiplayerManager;

    protected override void Start()
    {
        base.Start();

        _multiplayerManager = MultiplayerManager.Instance;
    }

    protected override void TakeFromEmptyCounter(PlayerHand hand)
    {
        _multiplayerManager.CreateObject(GetObject(), hand);
    }

    GameObject GetObject()
    {
        if (_materialPrefabs.Length > 1)
            return _materialPrefabs[Random.Range(0, _materialPrefabs.Length)];
        else
            return _materialPrefabs[0];
    }
}
