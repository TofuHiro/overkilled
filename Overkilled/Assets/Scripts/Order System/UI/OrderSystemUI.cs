using UnityEngine;

public class OrderSystemUI : MonoBehaviour
{
    [Tooltip("Parent transform holding the order cards")]
    [SerializeField] RectTransform _orderCardsHolder;

    OrderCardUI[] _cards;
    
    void Awake()
    {
        _cards = _orderCardsHolder.GetComponentsInChildren<OrderCardUI>();
    }

    void Start()
    {
        OrderSystem.Instance.OnOrderCreate += UpdateUI;
        OrderSystem.Instance.OnOrderComplete += UpdateUI;
        OrderSystem.Instance.OnOrderFail += UpdateUI;
    }

    void Update()
    {
        SetTimers();        
    }

    void UpdateUI()
    {
        OrderSystem.ActiveOrder[] orders = OrderSystem.Instance.GetActiveOrders();
        for (int i = 0; i < orders.Length; i++)
            _cards[i].SetOrder(orders[i].Order);
    }

    void SetTimers()
    {
        OrderSystem.ActiveOrder[] orders = OrderSystem.Instance.GetActiveOrders();
        for (int i = 0; i < orders.Length; i++)
        {
            if (!orders[i].Active)
                continue;

            _cards[i].SetTimer(orders[i].Timer);
        }
    }
}
