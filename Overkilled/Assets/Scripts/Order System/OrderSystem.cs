using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class OrderSystem : NetworkBehaviour
{
    [Tooltip("Recipe sets to pull orders from. Preset sets for testing")]
    [SerializeField] List<RecipeSetSO> _activeRecipeSets;
    [Tooltip("The maximum number of active orders allowed at once")]
    [SerializeField] int _maxActiveOrders = 5;

    public static OrderSystem Instance;
    public OrderSO[] OrdersCatalog;

    public delegate void OrderSystemAction();
    public event OrderSystemAction OnOrderCreate;
    public event OrderSystemAction OnOrderComplete;
    public event OrderSystemAction OnOrderFail;

    Bank _bank;
    ActiveOrder[] _activeOrders;

    void Awake()
    {
        if (_activeRecipeSets == null)
            _activeRecipeSets = new List<RecipeSetSO>();

        _activeOrders = new ActiveOrder[_maxActiveOrders];
        for (int i = 0; i < _activeOrders.Length; i++)
            _activeOrders[i] = new ActiveOrder();

        _bank = Bank.Instance;

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Order System found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    void Update()
    {
        TickOrderTimes();

        if (IsServer)
            FailCheckOrders();
    }

    /// <summary>
    /// Adds a recipe set to create orders from
    /// </summary>
    /// <param name="recipeSet"></param>
    public void AddRecipeSet(RecipeSetSO recipeSet) => _activeRecipeSets.Add(recipeSet);

    /// <summary>
    /// Removes a recipe set to use to create orders from
    /// </summary>
    /// <param name="recipeSetSO"></param>
    public void RemoveRecipeSet(RecipeSetSO recipeSetSO)
    {
        if (_activeRecipeSets.Contains(recipeSetSO))
            _activeRecipeSets.Remove(recipeSetSO);
    }

    /// <summary>
    /// Set the active recipe sets to a predefined list of sets
    /// </summary>
    /// <param name="recipeSets">A list of recipe set scriptableobjects to set to</param>
    public void SetRecipeSets(List<RecipeSetSO> recipeSets) => _activeRecipeSets = recipeSets;

    /// <summary>
    /// Clear the list of active recipe sets
    /// </summary>
    public void RemoveAllRecipeSets() => _activeRecipeSets.Clear();

    /// <summary>
    /// Returns an array storing the active orders
    /// </summary>
    /// <returns></returns>
    public ActiveOrder[] GetActiveOrders() { return _activeOrders; }

    /// <summary>
    /// Get the index of the first order of a specified item in the array of active orders
    /// </summary>
    /// <param name="item">The product item to search for in the active orders</param>
    /// <returns>Returns the index of the active order for a given item in the active orders array. Returns -1 if null</returns>
    int GetActiveOrderIndex(ItemSO item)
    {
        for (int i = 0; i < _activeOrders.Length; i++)
        {
            if (!_activeOrders[i].Active)
                continue;

            if (_activeOrders[i].Order.requestedItemRecipe.product == item)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Returns a random index for a recipe set in the active recipe set list
    /// </summary>
    /// <returns></returns>
    int GetRandomRecipeSetIndex() { return Random.Range(0, _activeRecipeSets.Count); }

    /// <summary>
    /// Returns a random index for a recipe in a given recipe set
    /// </summary>
    /// <param name="recipeSet"></param>
    /// <returns></returns>
    int GetRandomRecipeIndex(RecipeSetSO recipeSet) { return Random.Range(0, recipeSet.recipes.Length); }

    /// <summary>
    /// Returns the index of the next available free slot in the array of active orders
    /// </summary>
    /// <returns></returns>
    int GetNextFreeOrderSlot()
    {
        for (int i = 0; i < _activeOrders.Length; i++)
            if (!_activeOrders[i].Active)
                return i;

        return -1;
    }

    /// <summary>
    /// Check if there is an active order for a given item
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Returns true if the item matches an active order</returns>
    public bool CheckActiveOrder(ItemSO item)
    {
        if (GetActiveOrderIndex(item) == -1) 
            return false;

        return true;
    }

    void CreateRandomOrder()
    {
        if (GetNextFreeOrderSlot() == -1)
            return;
        
        int recipeSetIndex = GetRandomRecipeSetIndex();
        CreateOrderClientRpc(recipeSetIndex, GetRandomRecipeIndex(_activeRecipeSets[recipeSetIndex]));
    }

    [ClientRpc]
    void CreateOrderClientRpc(int recipeSetIndex, int recipeIndex)
    {
        foreach (OrderSO order in OrdersCatalog)
        {
            if (order.requestedItemRecipe == _activeRecipeSets[recipeSetIndex].recipes[recipeIndex])
            {
                int freeSlot = GetNextFreeOrderSlot();
                _activeOrders[freeSlot].Order = order;
                _activeOrders[freeSlot].Timer = order.timeLimit;
                _activeOrders[freeSlot].Active = true;
                OnOrderCreate?.Invoke();
                return;
            }
        }

        Debug.LogError("Error. Recipe " + _activeRecipeSets[recipeIndex].name + " is not found in orders catalog. Reload resources or create a new order for the recipe");
    }

    /// <summary>
    /// Attempt to deliver an item 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="durabilityFactor"></param>
    /// <returns></returns>
    public void DeliverRecipe(ItemSO item, float durabilityFactor)
    {
        int orderIndex = GetActiveOrderIndex(item);
        if (orderIndex == -1)
            return;

        CompleteOrderServerRpc(orderIndex, durabilityFactor);
    }

    [ServerRpc(RequireOwnership = false)]
    void CompleteOrderServerRpc(int orderIndex, float durabilityFactor)
    {
        CompleteOrderClientRpc(orderIndex, durabilityFactor);
    }

    [ClientRpc]
    void CompleteOrderClientRpc(int orderIndex, float durabilityFactor)
    {
        OrderSO order = _activeOrders[orderIndex].Order;

        _bank.AddMoney((int)Mathf.Max(order.minReward, order.reward * durabilityFactor));
        RemoveOrder(order);
        OnOrderComplete?.Invoke();

        Debug.Log("Order completed of " + order.requestedItemRecipe.product.name);
    }

    [ClientRpc]
    /// <summary>
    /// Fails an incomplete order and removes it from active orders
    /// </summary>
    /// <param name="index"></param>
    void FailOrderClientRpc(int index)
    {
        Debug.Log("Order failed for " + _activeOrders[index].Order.requestedItemRecipe.product.name);

        RemoveOrder(index);
        OnOrderFail?.Invoke();
    }

    void RemoveOrder(OrderSO order)
    {
        for (int i = 0; i < _activeOrders.Length; i++)
        {
            if (_activeOrders[i].Order == order)
            {
                RemoveOrder(i);
                break;
            }
        }
    }
    void RemoveOrder(int index)
    {
        _activeOrders[index].Order = null;
        _activeOrders[index].Timer = 0f;
        _activeOrders[index].Active = false;
        _activeOrders = _activeOrders.OrderBy(e => e.Active == false).ToArray();
    }

    void TickOrderTimes()
    {
        for (int i = 0; i < _activeOrders.Length; i++)
            if (_activeOrders[i].Active)
                _activeOrders[i].Timer -= Time.deltaTime;
    }

    void FailCheckOrders()
    {
        for (int i = 0; i < _activeOrders.Length; i++)
            if (_activeOrders[i].Active)
                if (_activeOrders[i].Timer <= 0f)
                    FailOrderClientRpc(i);
    }

    ///////////////
    public void StartCreatingOrders()
    {
        if (!IsServer)
            return;

        Debug.Log("Starting to create orders");
        InvokeRepeating("CreateRandomOrder", 2f, 5f);
    }

    
}
