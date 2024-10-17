using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence 
{
    public void SaveGame(ref GameData data);
    public void LoadGame(GameData data);
}
