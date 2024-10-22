using Unity.Netcode;
using UnityEngine;

public class CraftingStation : CounterTop
{
    [Tooltip("The set of recipes this crafting station will allow crafting for")]
    [SerializeField] RecipeSetSO _recipeSet;
    [Tooltip("For dev. Products spawn at the first item holder in the hierachy. Assign this for a visual reference of where that will be on the table")]
    [SerializeField] ItemHolder _firstItemHolder;

    NetworkVariable<float> _craftProgress = new NetworkVariable<float>(0f);
    CraftRecipeSO _validRecipe;
    bool _hasValidRecipe;
    bool _isCrafting;

    public override void Interact(PlayerInteraction player)
    {
        base.Interact(player);
        InteractServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void InteractServerRpc()
    {
        StopCrafting();
        SetValidRecipe();
    }

    void SetValidRecipe()
    {
        _validRecipe = _recipeSet.GetValidRecipe(_holders, NumberOfItemsOnCounter);
        if (_validRecipe != null)
            _hasValidRecipe = true;
    }

    void Update()
    {
        if (!IsServer)
            return;

        if (_isCrafting)
        {
            _craftProgress.Value += Time.deltaTime;
            Debug.Log(_craftProgress.Value);

            if (_craftProgress.Value >= _validRecipe.craftTime)
                CompleteCraft();
        }    
    }

    /// <summary>
    /// Start or speed up crafting at this station
    /// </summary>
    public void Craft()
    {
        CraftServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void CraftServerRpc()
    {
        if (_hasValidRecipe)
        {
            if (!_isCrafting)
                _isCrafting = true;
            else
                _craftProgress.Value += .5f;
        }
        else
        {
            Debug.Log("Cant craft");
        }
    }

    void StopCrafting()
    {
        if (!_isCrafting)
            return;

        _craftProgress.Value = 0f;

        _isCrafting = false;
        _hasValidRecipe = false;
        _validRecipe = null;
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
            MultiplayerManager.Instance.DestroyObject(item.gameObject);
        }

        GameObject productObject = Instantiate(_validRecipe.product.prefabReference);
        MultiplayerManager.Instance.SpawnObject(productObject);
        Item product = productObject.GetComponent<Item>();
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
