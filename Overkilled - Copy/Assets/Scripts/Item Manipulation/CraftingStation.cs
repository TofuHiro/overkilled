using Unity.Netcode;
using UnityEngine;

public class CraftingStation : CounterTop
{
    [Tooltip("The set of recipes this crafting station will allow crafting for")]
    [SerializeField] RecipeSetSO _recipeSet;
    [Tooltip("Where crafted products will spawn")]
    [SerializeField] Transform _productSpawnPoint;
    [Tooltip("If true, crafting progress will increase automatically")]
    [SerializeField] bool _autoCraft;

    const float k_craftIncrement = .5f;

    public float CraftProgress { get { return _craftProgress.Value; } private set { _craftProgress.Value = value; } }
    public bool IsCrafting { get { return _isCrafting.Value; } private set { _isCrafting.Value = value; } }
    public bool HasValidRecipe { get { return _hasValidRecipe.Value; } private set { _hasValidRecipe.Value = value; } }

    NetworkVariable<float> _craftProgress = new NetworkVariable<float>(0f);
    NetworkVariable<bool> _isCrafting = new NetworkVariable<bool>(false);
    NetworkVariable<bool> _hasValidRecipe = new NetworkVariable<bool>(false);

    CraftRecipeSO _validRecipe;

    protected override void Start()
    {
        base.Start();

        if (_recipeSet == null)
        {
            Debug.LogWarning($"{name} does not have a recipe set");
        }
    }

    public override void Interact(PlayerInteraction player)
    {
        base.Interact(player);

        InteractServerRpc();
    }

    [Rpc(SendTo.Server)]
    void InteractServerRpc()
    {
        Server_StopCrafting();
        Server_CheckValidRecipe();
    }

    void Server_CheckValidRecipe()
    {
        _validRecipe = _recipeSet.GetValidRecipe(_holders, NumberOfItemsOnCounter);

        if (_validRecipe != null)
            HasValidRecipe = true;
    }

    void Update()
    {
        if (!IsServer)
            return;

        if (IsCrafting)
        {
            if (_autoCraft)
                CraftProgress += Time.deltaTime;

            if (CraftProgress >= _validRecipe.craftTime)
                Server_CompleteCraft();
        }    
    }

    /// <summary>
    /// Start or speed up crafting at this station
    /// </summary>
    public void Craft()
    {
        CraftServerRpc();
    }

    [Rpc(SendTo.Server)]
    void CraftServerRpc()
    {
        if (HasValidRecipe)
        {
            if (!IsCrafting)
                IsCrafting = true;
            else
                CraftProgress += k_craftIncrement;
        }
        else
        {
            Debug.Log("Cant craft");
        }
    }

    void Server_StopCrafting()
    {
        if (!IsCrafting)
            return;

        CraftProgress = 0f;

        IsCrafting = false;
        HasValidRecipe = false;
        _validRecipe = null;
    }

    void Server_CompleteCraft()
    {
        Server_SpawnProduct();
        Server_StopCrafting();
    }

    void Server_SpawnProduct()
    {
        foreach (ItemHolder holder in _holders)
        {
            Item item = holder.GetItem();
            if (item == null) 
                continue;

            holder.SetItem(null);
            ObjectSpawner.Instance.DestroyObject(item.gameObject);
        }

        ObjectSpawner.Instance.SpawnObject(_validRecipe.productPrefab, _productSpawnPoint.position, _productSpawnPoint.rotation);
    }

    void OnDrawGizmos()
    {
        if (_productSpawnPoint == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(_productSpawnPoint.position, Vector3.one / 7f);
    }
}
