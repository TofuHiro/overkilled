using UnityEngine;

public class MaterialBox : CounterTop
{
    [Tooltip("The source objects to spawn from this box")]
    [SerializeField] GameObject[] _materialPrefabs;

    ObjectSpawner _objectSpawner;

    protected override void Start()
    {
        base.Start();

        _objectSpawner = ObjectSpawner.Instance;
    }

    protected override void TakeFromEmptyCounter(PlayerHand hand)
    {
        _objectSpawner.SpawnObject(GetObject(), hand);
    }

    GameObject GetObject()
    {
        if (_materialPrefabs.Length > 1)
            return _materialPrefabs[Random.Range(0, _materialPrefabs.Length)];
        else
            return _materialPrefabs[0];
    }
}
