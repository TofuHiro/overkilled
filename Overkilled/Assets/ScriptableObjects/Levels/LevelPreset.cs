using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Preset", menuName = "Levels/New Level Preset")]
public class LevelPreset : ScriptableObject
{
    new public string name;
    
    [Tooltip("Whether to use camera focusing mode or not (fixed otherwise)")]
    public bool useCameraFocusMode;
    [Tooltip("The time limit in seconds for this level")]
    public int timeLimit;

    [Header("Orders")]
    [Tooltip("Orders to create for this level")]
    public OrderSO[] orders;
    [Tooltip("The time between each order creation")]
    public float orderCreationRate;

    [Header("Money Grading")]
    [Tooltip("Minimum money required for five star rating")]
    public int fiveStarsMinimum;
    [Tooltip("Minimum money required for four star rating")]
    public int fourStarsMinimum;
    [Tooltip("Minimum money required for three star rating")]
    public int threeStarsMinimum;
    [Tooltip("Minimum money required for two star rating")]
    public int twoStarsMinimum;
    [Tooltip("Minimum money required for one star rating")]
    public int oneStarMinimum;
}
