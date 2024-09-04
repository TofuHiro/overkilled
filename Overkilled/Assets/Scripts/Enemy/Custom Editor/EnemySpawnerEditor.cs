#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnemySpawner enemySpawner = (EnemySpawner)target;

        if (GUILayout.Button("Spawn Enemy"))
        {
            enemySpawner.Spawn();
        }
    }
}

#endif