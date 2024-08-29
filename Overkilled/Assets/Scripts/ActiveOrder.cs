[System.Serializable]
public class ActiveOrder
{
    public OrderSO Order;
    public float Timer;
    public bool Active;

    public ActiveOrder()
    {
        Order = null;
        Timer = 0f;
        Active = false;
    }
    public ActiveOrder(OrderSO order, float timer)
    {
        Order = order;
        Timer = timer;
        Active = false;
    }
}