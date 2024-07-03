using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using static GameInfo;

public class DataManager : MonoBehaviour
{
    public DataInfo currentGameData;
    public static List<DataInfo> dataList = new List<DataInfo>();

    public enum GameState
    {
        NewMap,
        LoadMap,
    }

    [HideInInspector]
    public GameState gameState;

    public static DataManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(Instance);

        ReadFiles();
    }

    // Reads all files in the save folder.
    private void ReadFiles()
    {
        dataList.Clear();

        string[] filesPaths = Directory.GetFiles(Application.persistentDataPath, "*GameData.json");

        if (filesPaths.Length > 0)
        {
            foreach (string path in filesPaths)
            {
                DataInfo dataInfo = new DataInfo();
                dataInfo.path = path;
                dataInfo.data = DataInfo.ReadData(path);
                dataList.Add(dataInfo);
            }
        }
    }

    // Deletes a file.
    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    // Deletes all files in the save folder.
    public static void DeleteAllFile()
    {
        if (dataList.Count > 0)
        {
            foreach (var dataInfo in dataList)
            {
                File.Delete(dataInfo.path);
            }

            dataList.Clear();
        }
    }

    // Creates a new GameData file.
    public static DataInfo NewSaveFile(string _cityName, int _mapSeed)
    {
        string _fileName = _cityName.ToLower();

        if (!File.Exists(Application.persistentDataPath + _cityName + "GameData.json"))
        {
            GameData gameData = new GameData();
            gameData.meta.cityName = _cityName;
            gameData.info.mapSeed = _mapSeed;
            gameData.meta.fileName = _fileName;
            gameData.meta.daysPlayed = 0;
            var culture = new CultureInfo("en-Gb");
            gameData.meta.lastPlayDate = DateTime.Now.ToString(culture);

            string filePath = Path.Combine(Application.persistentDataPath, _fileName + "GameData.json");
            string jsonData = JsonUtility.ToJson(gameData, true);

            DataInfo dataInfo = DataInfo.CreateDataInfo(filePath, gameData);

            dataList.Add(dataInfo);
            File.WriteAllText(filePath, jsonData);

            return dataInfo;
        }
        else
        {
            return null;
        }

    }

    // Saves the game to the current GameData file.
    public void SaveFile()
    {
        var culture = new CultureInfo("en-Gb");
        currentGameData.data.meta.lastPlayDate = DateTime.Now.ToString(culture);

        string jsonData = JsonUtility.ToJson(currentGameData.data, true);
        File.WriteAllText(currentGameData.path, jsonData);

        Debug.Log("Game saved!");
    }

    public void SaveData<T>(T dataType) where T : class
    {
        switch (dataType)
        {
            case ValuesPerResource valuesPerResource:
                currentGameData.data.info.resourcesData.Add(valuesPerResource);
                break;

            case BuildingsData buildingsData:
                currentGameData.data.info.buildingsData.Add(buildingsData);
                break;

            default:
                break;
        }
    }
}

// Helps organize the file path and its GameData.
public class DataInfo
{
    public GameData data;

    public string path;
    public static DataInfo CreateDataInfo(string paht, GameData data)
    {
        DataInfo info = new DataInfo();
        info.data = data;
        info.path = paht;
        return info;
    }

    public static GameData ReadData(string path)
    {
        GameData _data = new GameData();
        string jsonData = File.ReadAllText(path);
        _data = JsonUtility.FromJson<GameData>(jsonData);
        return _data;
    }

    public static GameData ReadData(DataInfo dataInfo)
    {
        return ReadData(dataInfo.path);
    }
}

[Serializable]
public class GameData
{
    public Meta meta = new Meta();
    public GameInfo info = new GameInfo();
}

[Serializable]
public class Meta
{
    public string cityName;
    public string fileName;
    public int daysPlayed;
    public string lastPlayDate;
}

[Serializable]
public class GameInfo
{
    public int mapSeed;

    public List<Extension> mapExtensions = new List<Extension>();

    public int gameLevel;

    public List<ValuesPerResource> resourcesData = new List<ValuesPerResource>();

    public List<BuildingsData> buildingsData = new List<BuildingsData>();
}

[Serializable]
public class ValuesPerResource
{
    public string name;
    public List<ValuesPerLevel> resourcesValues = new List<ValuesPerLevel>();
}

[Serializable]
public class ValuesPerLevel
{
    public int level;
    public int amount;
    public int delta;
    public int maxStorage;
}

[Serializable]
public class BuildingsData
{
    public Vector3 buildingPosition;
    public string buildingName;
    public int buildingID;
}