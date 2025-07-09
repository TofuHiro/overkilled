using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Items/New Recipe")]
public class CraftRecipeSO : ScriptableObject
{
    [Tooltip("The product item that this recipe will grant")]
    public GameObject productPrefab;
    [Tooltip("The ingredient items required to craft the product")]
    public ItemSO[] ingredients;
    [Tooltip("The base time required to craft this recipe")]
    public float craftTime;

    public ItemSO ProductItem
    {
        get 
        { 
           return productPrefab.GetComponent<Item>().GetItemInfo();
        }
    }

    /// <summary>
    /// Checks whether the given set of items can be used to craft this recipe
    /// </summary>
    /// <param name="holders">The set of ingredient items to check with</param>
    /// <returns>Returns true if the recipe can be craft with the given set of items</returns>
    public bool CheckValidRecipe(ItemHolder[] holders)
    {
        Dictionary<ItemSO, int> ingredientCounts = new Dictionary<ItemSO, int>();

        // Create counts for required items
        foreach (ItemSO item in ingredients)
        {
            if (ingredientCounts.ContainsKey(item))
            {
                ingredientCounts[item]++;
            }
            else
            {
                ingredientCounts.Add(item, 1);
            }
        }

        // Decrement counts with current items
        foreach (ItemHolder holder in holders)
        {
            if (holder.GetItem() == null)
                continue;

            ItemSO itemSO = holder.GetItem().GetItemInfo();
            if (ingredientCounts.ContainsKey(itemSO))
            {
                ingredientCounts[itemSO]--;
            }
        }

        // If any remaining required item counts is not 0 or less, dont have all items
        foreach (var counts in ingredientCounts)
        {
            if (counts.Value > 0)
            {
                return false;
            }
        }

        return true;
    }
}
