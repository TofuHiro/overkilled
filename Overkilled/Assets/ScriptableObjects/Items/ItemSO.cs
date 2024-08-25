using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
public class ItemSO : ScriptableObject
{
    new public string name;
    [Tooltip("The prefab game object for this item")]
    public GameObject prefabReference;
    [Tooltip("The sprite icon for this item")]
    public Sprite icon;
}
