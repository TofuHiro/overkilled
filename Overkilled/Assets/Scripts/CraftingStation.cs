using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : CounterTop
{
    [SerializeField] RecipeSetSO _recipeSet;
    [SerializeField] ItemHolder _firstItemHolder;

    CraftRecipeSO _validRecipe;
    float _craftProgress = 0f;
    bool _isCrafting = false;

    public override void Interact(PlayerInteraction player)
    {
        StopCrafting();
        base.Interact(player);
        _validRecipe = _recipeSet.GetValidRecipe(_holders, ItemsOnCounter);
    }

    void Update()
    {
        if (_isCrafting)
        {
            _craftProgress += Time.deltaTime;
            Debug.Log(_craftProgress);

            if (_craftProgress >= _validRecipe.craftTime)
                CompleteCraft();
        }    
    }

    public void Craft()
    {
        if (_validRecipe != null)
        {
            BeginCrafting();

            if (_isCrafting)
                SpeedUpCraft();
        }
        else
        {
            Debug.Log("Cant craft");
        }
    }

    void BeginCrafting()
    {
        if (_isCrafting)
            return;

        _isCrafting = true;
    }

    void StopCrafting()
    {
        if (!_isCrafting)
            return;

        _craftProgress = 0f;
        _validRecipe = null;
        _isCrafting = false;
    }

    void SpeedUpCraft()
    {
        _craftProgress += .5f;
    }

    void CompleteCraft()
    {
        SpawnProduct();
        StopCrafting();
    }

    void SpawnProduct()
    {
        foreach (ItemHolder holder in _holders)
        {
            if (holder.GetItem() == null) 
                continue;

            Item item = holder.GetItem();
            holder.SetItem(null);
            Destroy(item.gameObject);
        }

        Item product = Instantiate(_validRecipe.product.prefabReference).GetComponent<Item>();
        _holders[0].SetItem(product);
    }

    void OnDrawGizmos()
    {
        if (_firstItemHolder == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(_firstItemHolder.GetHoldPosition(), Vector3.one / 7f);
    }
}
