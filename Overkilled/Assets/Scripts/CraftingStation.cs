using Unity.Netcode;
using UnityEngine;

public class CraftingStation : CounterTop
{
    [Tooltip("The set of recipes this crafting station will allow crafting for")]
    [SerializeField] RecipeSetSO _recipeSet;
    [Tooltip("For dev. Products spawn at the first item holder in the hierachy. Assign this for a visual reference of where that will be on the table")]
    [SerializeField] ItemHolder _firstItemHolder;

    NetworkVariable<float> _craftProgress = new NetworkVariable<float>(0f);
    NetworkVariable<bool> _isCrafting = new NetworkVariable<bool>(false);
    NetworkVariable<bool> _hasValidRecipe = new NetworkVariable<bool>(false);
    CraftRecipeSO _validRecipe;

    public override void Interact(PlayerInteraction player)
    {
        base.Interact(player);
        StopCraftingServerRpc();
        SetValidRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void SetValidRecipeServerRpc()
    {
        SetValidRecipeClientRpc();
    }
    [ClientRpc]
    void SetValidRecipeClientRpc()
    {
        _validRecipe = _recipeSet.GetValidRecipe(_holders, ItemsOnCounter);
        if (_validRecipe != null )
            _hasValidRecipe.Value = true;
    }

    void Update()
    {
        if (!IsServer)
            return;

        if (_isCrafting.Value)
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
        if (_hasValidRecipe.Value)
        {
            if (!_isCrafting.Value)
                BeginCraftingServerRpc();
            else
                SpeedUpCraftServerRpc();
        }
        else
        {
            Debug.Log("Cant craft");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void BeginCraftingServerRpc()
    {
        _isCrafting.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    void StopCraftingServerRpc()
    {
        if (!_isCrafting.Value)
            return;

        _craftProgress.Value = 0f;
        _isCrafting.Value = false;
        _hasValidRecipe.Value = false;
        _validRecipe = null;
    }

    [ServerRpc(RequireOwnership = false)]
    void SpeedUpCraftServerRpc()
    {
        _craftProgress.Value += .5f;
    }

    void CompleteCraft()
    {
        SpawnProduct();
        StopCraftingServerRpc();
    }

    void SpawnProduct()
    {
        foreach (ItemHolder holder in _holders)
        {
            if (holder.GetItem() == null) 
                continue;

            Item item = holder.GetItem();
            holder.SetItem(null);
            DestroyIngredientServerRpc(item.GetNetworkObject());
        }

        NetworkObject productNetworkObject = Instantiate(_validRecipe.product.prefabReference).GetComponent<NetworkObject>();
        productNetworkObject.Spawn(true);

        Item product = productNetworkObject.GetComponent<Item>();
        _holders[0].SetItem(product);
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyIngredientServerRpc(NetworkObjectReference itemNetworkObjectReference)
    {
        itemNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);
        Destroy(itemNetworkObject.gameObject);
    }

    void OnDrawGizmos()
    {
        if (_firstItemHolder == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(_firstItemHolder.GetHoldPosition(), Vector3.one / 7f);
    }
}
