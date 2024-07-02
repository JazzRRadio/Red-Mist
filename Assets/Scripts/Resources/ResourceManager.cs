using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GameInfo;

public class ResourceManager : MonoBehaviour
{
    [SerializeField]
    private List<ResourceAssets> resourceList;

    public static int currentGameLevel = 0;

    private void Start()
    {
        ResetRecourses();

        switch (DataManager.Instance.gameState)
        {
            case DataManager.GameState.NewMap:
            default:
                AddLevel();
                SaveResourceData(DataManager.Instance.currentGameData.data.info);
                DataManager.Instance.SaveData();
                break;

            case DataManager.GameState.LoadMap:
                LoadResourceData(DataManager.Instance.currentGameData.data.info);
                break;
        }

        ResourceAssets.OnValueChanged?.Invoke();
    }


    // Adds a level to all resources with levels. At the start of the game, add a level to all the resource.
    private void AddLevel()
    {
        currentGameLevel++;

        UIDirector.Instance.sc_ResourceMenuList?.AddButton(currentGameLevel);
        UIDirector.Instance.sc_BuildingsMenuList?.AddButton(currentGameLevel);

        foreach (var resource in resourceList)
        {
            if (resource.hasLevel)
            {
                Values _values = resource.AddNewLevel(currentGameLevel);
                resource.values.Add(_values);
            }
            else if (!resource.hasLevel)
            {
                if (resource.values.Count == 0)
                {
                    Values _values = resource.AddNewLevel(currentGameLevel);
                    resource.values.Add(_values);
                }
            }
        }
    }

    // Adds several level at once.
    private void AddLevels(int numLevels)
    {
        for (int i = 0; i < numLevels; i++)
        {
            Debug.Log("Test");
            AddLevel();
        }
    }

    // Resets all the resources.
    private void ResetRecourses()
    {
        currentGameLevel = 0;

        UIDirector.Instance.sc_ResourceMenuList?.DestroyButtons();
        UIDirector.Instance.sc_BuildingsMenuList?.DestroyButtons();

        foreach (var resource in resourceList)
        {
            resource.values.Clear();
        }
    }

    public void SaveResourceData(GameInfo data)
    {
        data.gameLevel = currentGameLevel;
        data.resourcesData.Clear();

        foreach (var resource in resourceList)
        {
            ValuesPerResource values = new ValuesPerResource();
            values.name = resource.name;

            int level = 1;

            foreach (var value in resource.values)
            {
                ValuesPerLevel valuesPerLevel = new ValuesPerLevel();
                valuesPerLevel.level = level;
                valuesPerLevel.amount = value.amount;
                valuesPerLevel.delta = value.delta;
                valuesPerLevel.maxStorage = value.maxStorage;
                values.resourcesValues.Add(valuesPerLevel);
                level++;
            }

            data.resourcesData.Add(values);
        }
    }

    private void LoadResourceData(GameInfo data)
    {
        AddLevels(data.gameLevel);

        for (int index = 0; index < resourceList.Count; index++)
        {
            LoadResourceLevel(resourceList[index].values, data.resourcesData[index].resourcesValues);
        }

    }

    private void LoadResourceLevel(List<Values> listValues, List<ValuesPerLevel> valuesPerLevels)
    {
        for (int index = 0; index < listValues.Count; index++)
        {
            listValues[index].amount = valuesPerLevels[index].amount;
            listValues[index].delta = valuesPerLevels[index].delta;
            listValues[index].maxStorage = valuesPerLevels[index].maxStorage;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ResourceManager))]
    public class ResourceManagerEditor : Editor
    {
        private int levels;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ResourceManager resourceManager = (ResourceManager)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Levels to add: ");
            levels = EditorGUILayout.IntField("Number of levels", levels);

            if (GUILayout.Button("New Level"))
            {
                resourceManager.AddLevel();
            }

            if (GUILayout.Button(" Add Multiple Levels"))
            {
                resourceManager.AddLevels(levels);
            }

            if (GUILayout.Button("Reset Levels"))
            {
                resourceManager.ResetRecourses();
            }
        }
    }
#endif
}
