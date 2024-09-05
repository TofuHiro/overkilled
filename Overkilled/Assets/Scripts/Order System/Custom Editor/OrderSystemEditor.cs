#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OrderSystem))]
public class OrderSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OrderSystem orderSystem = (OrderSystem)target;

        if (GUILayout.Button("Start Creating Orders"))
        {
            orderSystem.StartCreatingOrders(5f);
        }
    }
}

#endif