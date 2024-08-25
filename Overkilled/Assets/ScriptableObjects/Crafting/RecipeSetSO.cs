using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe Set", menuName = "Items/New Recipe Set")]
public class RecipeSetSO : ScriptableObject
{
    new public string name;
    [Tooltip("The set of recipe this set contains")]
    public CraftRecipeSO[] recipes;

    /// <summary>
    /// Returns the recipe that can be crafted given a set of items
    /// </summary>
    /// <param name="holders">The set of ingredient items to check with</param>
    /// <param name="itemsOnCounter">The current number of items on the crafting table. This is used to optimise search</param>
    /// <returns>Returns the scriptableobject of the recipe that can be crafted with the given items. Returns null if nothing can be crafted</returns>
    public CraftRecipeSO GetValidRecipe(ItemHolder[] holders, int itemsOnCounter)
    {
        for (int i = 0; i < recipes.Length; i++)
        {
            if (itemsOnCounter != recipes[i].ingredients.Length)
                continue;

            if (recipes[i].CheckValidRecipe(holders))
                return recipes[i];
        }

        return null;
    }
}
