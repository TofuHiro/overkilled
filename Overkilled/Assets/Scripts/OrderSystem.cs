using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrderSystem : MonoBehaviour
{
    [SerializeField] List<RecipeSetSO> _activeRecipeSets;
    [SerializeField] int _maxActiveOrders = 5;

    public static OrderSystem Instance;
    public OrderSO[] OrdersCatalog;

    public delegate void OrderSystemAction();
    public event OrderSystemAction OnOrderCreate;
    public event OrderSystemAction OnOrderComplete;
    public event OrderSystemAction OnOrderFail;
    public event OrderSystemAction OnOrderTimeTick;

    Bank _bank;
    OrderSO[] _activeOrders;
    float[] _orderTimers;

    void Awake()
    {
        if (_activeRecipeSets == null)
            _activeRecipeSets = new List<RecipeSetSO>();

        _bank = Bank.Instance;
        _activeOrders = new OrderSO[_maxActiveOrders];
        _orderTimers = new float[_maxActiveOrders];

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Order System found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    void Update()
    {
        TickOrderTime();

        for (int i = 0; i < _orderTimers.Length; i++)
        {
            if (_activeOrders[i] == null)
                continue;

            if (_orderTimers[i] <= 0f)
                FailOrder(i);
        }
    }

    public void AddRecipeSet(RecipeSetSO recipeSet) => _activeRecipeSets.Add(recipeSet);
    public void RemoveRecipeSet(RecipeSetSO recipeSetSO)
    {
        if (_activeRecipeSets.Contains(recipeSetSO))
            _activeRecipeSets.Remove(recipeSetSO);
    }
    public void SetRecipeSets(List<RecipeSetSO> recipeSets) => _activeRecipeSets = recipeSets;
    public void RemoveAllRecipeSets() => _activeRecipeSets.Clear();
    public OrderSO[] GetActiveOrders() { return _activeOrders; }
    public OrderSO GetFirstActiveOrder(ItemSO item)
    {
        foreach (OrderSO order in _activeOrders)
        {
            if (order == null) 
                continue;

            if (order.requestedItemRecipe.product == item)
                return order;
        }

        return null;
    }
    public ref float[] GetActiveOrdersTimers() { return ref _orderTimers; }

    int GetNextFreeOrderSlot()
    {
        for (int i = 0; i < _activeOrders.Length; i++)
            if (_activeOrders[i] == null)
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
                _activeOrders[freeSlot] = order;
                _orderTimers[freeSlot] = order.timeLimit;
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

    public bool CheckForOrder(ItemSO item)
    {
        return GetFirstActiveOrder(item) != null;
    }

    public void CompleteOrder(ItemSO item, float durabilityFactor)
    {
        Debug.Log("Order completed of " + item.name);

        OrderSO order = GetFirstActiveOrder(item);

        _bank.AddMoney((int)Mathf.Max(order.minReward, order.reward * durabilityFactor));

        RemoveOrder(order);
        OnOrderComplete?.Invoke();
    }

    public void FailOrder(int index)
    {
        Debug.Log("Order failed for " + _activeOrders[index].requestedItemRecipe.product.name);

        RemoveOrder(index);
        OnOrderFail?.Invoke();
    }

    void RemoveOrder(OrderSO order)
    {
        for (int i = 0; i < _activeOrders.Length; i++)
        {
            if (_activeOrders[i] == order)
            {
                _activeOrders[i] = null;
                break;
            }
        }
    }
    void RemoveOrder(int index)
    {
        _activeOrders[index] = null;
    }

    void TickOrderTime()
    {
        for (int i = 0; i < _activeOrders.Length; i++)
            if (_activeOrders[i] != null)
                _orderTimers[i] -= Time.deltaTime;

        OnOrderTimeTick?.Invoke();
    }

    ///////////////
    public void StartCreatingOrders()
    {
        Debug.Log("Starting to create orders");
        InvokeRepeating("CreateRandomOrder", 2f, 5f);
    }
}
