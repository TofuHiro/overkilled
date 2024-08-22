using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe Set", menuName = "Items/New Recipe Set")]
public class RecipeSetSO : ScriptableObject
{
    new public string name;
    public CraftRecipeSO[] recipes;

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
