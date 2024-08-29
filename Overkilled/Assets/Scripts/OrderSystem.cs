using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderSystem : MonoBehaviour
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
    public ref ActiveOrder[] GetActiveOrders() { return ref _activeOrders; }

    /// <summary>
    /// Get the first order of a specified item in the array of active orders
    /// </summary>
    /// <param name="item">The product item to search for in the active orders</param>
    /// <returns></returns>
    OrderSO GetFirstActiveOrder(ItemSO item)
    {
        foreach (ActiveOrder activeOrder in _activeOrders)
        {
            if (!activeOrder.Active) 
                continue;

            if (activeOrder.Order.requestedItemRecipe.product == item)
                return activeOrder.Order;
        }

        return null;
    }

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

    void CreateRandomOrder()
    {
        if (GetNextFreeOrderSlot() == -1)
            return;

        CreateOrder(GetRandomRecipe());
    }

    void CreateOrder(CraftRecipeSO recipe)
    {
        foreach (OrderSO order in OrdersCatalog)
        {
            if (order.requestedItemRecipe == recipe)
            {
                int freeSlot = GetNextFreeOrderSlot();
                _activeOrders[freeSlot].Order = order;
                _activeOrders[freeSlot].Timer = order.timeLimit;
                _activeOrders[freeSlot].Active = true;
                OnOrderCreate?.Invoke();
                return;
            }
        }

        Debug.LogError("Error. Recipe " + recipe.name + " is not found in orders catalog. Reload resources or create a new order for the recipe");
    }

    CraftRecipeSO GetRandomRecipe()
    {
        RecipeSetSO recipeSet = _activeRecipeSets[Random.Range(0, _activeRecipeSets.Count)];
        CraftRecipeSO recipe = recipeSet.recipes[Random.Range(0, recipeSet.recipes.Length)];
        return recipe;
    }

    /// <summary>
    /// Check if there is an active order for a given item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CheckForOrder(ItemSO item)
    {
        return GetFirstActiveOrder(item) != null;
    }

    /// <summary>
    /// Complete an active order for a given item
    /// </summary>
    /// <param name="item">The product order to fulfill</param>
    /// <param name="durabilityFactor">The durability factor of the item to apply to the reward</param>
    public void CompleteOrder(ItemSO item, float durabilityFactor)
    {
        Debug.Log("Order completed of " + item.name);

        OrderSO order = GetFirstActiveOrder(item);

        _bank.AddMoney((int)Mathf.Max(order.minReward, order.reward * durabilityFactor));

        RemoveOrder(order);
        OnOrderComplete?.Invoke();
    }

    /// <summary>
    /// Fails an incomplete order and removes it from active orders
    /// </summary>
    /// <param name="index"></param>
    public void FailOrder(int index)
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
                    FailOrder(i);
    }

    ///////////////
    public void StartCreatingOrders()
    {
        Debug.Log("Starting to create orders");
        InvokeRepeating("CreateRandomOrder", 2f, 5f);
    }
}
