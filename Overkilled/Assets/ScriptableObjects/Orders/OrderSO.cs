using UnityEngine;

[CreateAssetMenu(fileName = "New Order", menuName = "Orders/New Order")]
public class OrderSO : ScriptableObject
{
    [Tooltip("Item that needs to be crafted and sent")]
    public CraftRecipeSO requestedItemRecipe;
    [Tooltip("The amount of currency awarded for this order")]
    public int reward;
    [Tooltip("The minimum amount of currency awarded despite weapon durability")]
    public int minReward;
    [Tooltip("The maximum time allowed to fulfill this order")]
    public float timeLimit;
}
