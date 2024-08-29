using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OrderSystem))]
public class OrderSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OrderSystem orderSystem = (OrderSystem)target;

        if (GUILayout.Button("Load Orders To Orders Catalog List"))
        {
            orderSystem.OrdersCatalog = Resources.LoadAll<OrderSO>("");
        }

        if (GUILayout.Button("Start Creating Orders"))
        {
            orderSystem.StartCreatingOrders();
        }
    }
}
