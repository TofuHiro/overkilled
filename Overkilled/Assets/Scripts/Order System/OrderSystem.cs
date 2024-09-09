using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using SurvivalGame;

public class OrderSystem : NetworkBehaviour
{
    [Tooltip("Orders to create from")]
    [SerializeField] OrderSO[] _ordersCatalog;
    [Tooltip("The maximum number of active orders allowed at once")]
    [SerializeField] int _maxActiveOrders = 5;

    public static OrderSystem Instance;

    public delegate void OrderSystemAction();
    public event OrderSystemAction OnOrderCreate;
    public event OrderSystemAction OnOrderComplete;
    public event OrderSystemAction OnOrderFail;

    ActiveOrder[] _activeOrders;
    float _createRate;

    void Awake()
    {
        _activeOrders = new ActiveOrder[_maxActiveOrders];
        for (int i = 0; i < _activeOrders.Length; i++)
            _activeOrders[i] = new ActiveOrder();

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Order System found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;

        GameManager.OnGameInitialize += Initialize;
        GameManager.OnGameStateChange += StartCreatingOrders;
        GameManager.OnGameStateChange += StopCreatingOrders;
    }

    void Update()
    {
        if (GameManager.Instance.GameEnded)
            return;

        TickOrderTimes();

        if (IsServer)
            FailCheckOrders();
    }

    void Initialize(LevelPreset preset)
    {
        SetOrderCatalog(preset.orders);
        _createRate = preset.orderCreationRate;
    }

    void SetOrderCatalog(OrderSO[] orders) => _ordersCatalog = orders;

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
    /// Returns a random order from the order catalog
    /// </summary>
    /// <returns></returns>
    OrderSO GetRandomOrder() { return _ordersCatalog[Random.Range(0, _ordersCatalog.Length)]; }

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

    void StartCreatingOrders()
    {
        if (!IsServer)
            return;

        if (GameManager.Instance.GameStarted)
        {
            Debug.Log("Starting to create orders");
            InvokeRepeating("CreateRandomOrder", 2f, _createRate);
        }
    }

    void StopCreatingOrders()
    {
        if (!IsServer)
            return;

        if (GameManager.Instance.GameEnded)
        {
            Debug.Log("Stopped creating orders");
            CancelInvoke("CreateRandomOrder");
        }
    }

    void CreateRandomOrder()
    {
        if (GetNextFreeOrderSlot() == -1)
            return;
        
        OrderSO order = GetRandomOrder();
        CreateOrderClientRpc(GetIndexFromOrder(order));
    }

    int GetIndexFromOrder(OrderSO order)
    {
        for (int i = 0; i < _ordersCatalog.Length; i++)
        {
            if (_ordersCatalog[i] == order)
                return i;
        }

        Debug.LogError("Error. Could not find order " + order.name + ". Ensure that the order is set in the order catalog.");
        return -1;
    }

    [ClientRpc]
    void CreateOrderClientRpc(int orderIndex)
    {
        OrderSO order = _ordersCatalog[orderIndex];
        int freeSlot = GetNextFreeOrderSlot();
        _activeOrders[freeSlot].Order = order;
        _activeOrders[freeSlot].Timer = order.timeLimit;
        _activeOrders[freeSlot].Active = true;
        OnOrderCreate?.Invoke();
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

        Bank.AddMoney((int)Mathf.Max(order.minReward, order.reward * durabilityFactor));
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
}
