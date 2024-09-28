using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class is used to invoke Start method for mass amount of objects that are disabled on start.
/// Start may not be called on all objects when disabled on start so this utility class will call a method implemented by the IStartInvoke interface to ensure it.
/// </summary>
public class StartInvoker : MonoBehaviour
{
    void Start()
    {
        var gameObjects = FindObjectsOfType<MonoBehaviour>().OfType<IStartInvoke>();
        
        foreach (var gameObject in gameObjects)
        {
            gameObject.InvokeStart();
        }
    }
}
