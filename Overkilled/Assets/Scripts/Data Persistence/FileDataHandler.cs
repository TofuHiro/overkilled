using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    readonly string _encryptionKey = "TofuAndHiroAreTheDamnCutestFluffyChonksOnEarth";

    string _dataDirPath;
    string _dataFileName;
    bool _useEncryption;

    /// <summary>
    /// Constructor for creating a new FileDataHandler
    /// </summary>
    /// <param name="dataDirPath">The file path to store the save files</param>
    /// <param name="dataFileName">The file name of the save file. Note that the extension does not matter</param>
    /// <param name="useEncryption">Whether to use encryption for serializing/deserializing or not</param>
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        _dataDirPath = dataDirPath;
        _dataFileName = dataFileName;
        _useEncryption = useEncryption;
    }

    /// <summary>
    /// Deserializes game save file from JSON format to GameData class format
    /// </summary>
    /// <returns></returns>
    public GameData Load()
    {
        string fullPath = _dataDirPath + "/" + _dataFileName;
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad;
                //'using' blocks closes connections to file after use
                using FileStream stream = new FileStream(fullPath, FileMode.Open);
                using StreamReader reader = new StreamReader(stream);
                
                dataToLoad = reader.ReadToEnd();

                //Decrypt
                if (_useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load game data from file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    /// <summary>
    /// Serializes GameData to JSON format
    /// </summary>
    /// <param name="gameData"></param>
    public void Save(GameData gameData)
    {
        string fullPath = _dataDirPath + "/" + _dataFileName;

        try
        {
            //Create initial directory to file
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //Serialize GameData to JSON format
            string dataToStore = JsonUtility.ToJson(gameData, true);

            //Encrypt
            if (_useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            //'using' blocks closes connections to file after use
            using FileStream stream = new FileStream(fullPath, FileMode.Create);
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save game data to file: " + fullPath + "\n" + e);
        }
    }

    string EncryptDecrypt(string data)
    {
        string result = "";

        for (int i = 0; i < data.Length; i++)
        {
            result += (char)(data[i] ^ _encryptionKey[i % _encryptionKey.Length]);
        }

        return result;
    }
}
