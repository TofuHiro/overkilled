using UnityEngine;

public class OrderSystemUI : MonoBehaviour
{
    [SerializeField] RectTransform _orderCardsHolder;

    OrderCardUI[] _cards;
    OrderSystem _orderSystem;

    void Awake()
    {
        _cards = _orderCardsHolder.GetComponentsInChildren<OrderCardUI>();
    }

    void Start()
    {
        _orderSystem = OrderSystem.Instance;
        
        _orderSystem.OnOrderCreate += UpdateUI;
        _orderSystem.OnOrderComplete += UpdateUI;
        _orderSystem.OnOrderFail += UpdateUI;
        _orderSystem.OnOrderTimeTick += SetTimers;
    }

    void OnDisable()
    {
        _orderSystem.OnOrderCreate -= UpdateUI;
        _orderSystem.OnOrderComplete -= UpdateUI;
        _orderSystem.OnOrderFail -= UpdateUI;
        _orderSystem.OnOrderTimeTick -= SetTimers;
    }

    void UpdateUI()
    {
        OrderSO[] orders = _orderSystem.GetActiveOrders();
        for (int i = 0; i < orders.Length; i++)
            _cards[i].SetOrder(orders[i]);
    }

    void SetTimers()
    {
        float[] timers = _orderSystem.GetActiveOrdersTimers();
        for (int i = 0; i < timers.Length; i++)
            _cards[i].SetTimer(timers[i]);
    }
}
