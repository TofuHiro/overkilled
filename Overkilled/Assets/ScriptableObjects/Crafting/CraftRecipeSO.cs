using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Items/New Recipe")]
public class CraftRecipeSO : ScriptableObject
{
    public ItemSO product;
    public ItemSO[] ingredients;
    public float craftTime;

    public bool CheckValidRecipe(ItemHolder[] holders)
    {
        //tracking checked items to avoid duplicates matching
        bool[] acquiredItems = new bool[ingredients.Length];

        //check for matching items on counter to ingredients
        for (int i = 0; i < ingredients.Length; i++)
        {
            int itemIndex = ItemIndexInIngredients(holders[i].GetItem(), ref acquiredItems);
            if (itemIndex >= 0 && !acquiredItems[itemIndex])
                acquiredItems[itemIndex] = true;
            //invalid item(s)
            else if (itemIndex < 0)
                return false;
        }

        foreach (bool itemAcquired in acquiredItems)
        {
            if (!itemAcquired)
                return false;
        }

        return true;
    }

    int ItemIndexInIngredients(Item item, ref bool[] acquired)
    {
        for (int i = 0; i < ingredients.Length; i++)
        {
            if (item.GetItemInfo() == ingredients[i] && !acquired[i])
                return i;
        }
        return -1;
    }
}
