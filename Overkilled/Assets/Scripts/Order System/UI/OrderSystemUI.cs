using UnityEngine;

public class OrderSystemUI : MonoBehaviour
{
    [Tooltip("Parent transform holding the order cards")]
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
    }

    void Update()
    {
        SetTimers();        
    }

    void OnDisable()
    {
        _orderSystem.OnOrderCreate -= UpdateUI;
        _orderSystem.OnOrderComplete -= UpdateUI;
        _orderSystem.OnOrderFail -= UpdateUI;
    }

    void UpdateUI()
    {
        ActiveOrder[] orders = _orderSystem.GetActiveOrders();
        for (int i = 0; i < orders.Length; i++)
            _cards[i].SetOrder(orders[i].Order);
    }

    void SetTimers()
    {
        ActiveOrder[] orders = _orderSystem.GetActiveOrders();
        for (int i = 0; i < orders.Length; i++)
        {
            if (!orders[i].Active)
                continue;

            _cards[i].SetTimer(orders[i].Timer);
        }
    }
}
