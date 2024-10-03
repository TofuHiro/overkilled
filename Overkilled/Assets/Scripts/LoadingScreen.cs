using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    void Start()
    {
        Loader.LoadScene(Loader.Scene.SafeHouseScene);    
    }
}
