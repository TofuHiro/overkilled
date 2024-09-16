using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    private void Awake()
    {
        PlayerList.ResetStaticData();
        Bank.ResetStaticData();
    }
}
