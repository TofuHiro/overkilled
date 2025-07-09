using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cosmetic", menuName = "Player Customization/New Cosmetics")]
public class CosmeticSO : ScriptableObject
{
    new public string name;
    public Sprite icon;

    [Header("Unlock")]
    public string unlockText;
}
