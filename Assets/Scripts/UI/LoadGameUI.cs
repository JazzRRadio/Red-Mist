using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameUI : MonoBehaviour
{
    [SerializeField]
    private GameObject loadFile;
    [SerializeField]
    private Transform tf_ScrollContent;
    private List<GameObject> fileTabsList = new List<GameObject>();


    private void OnEnable()
    {
        if (DataManager.dataList.Count > 0)
        {
            LoadFileUpdate();
        }
    }

    public void LoadFileUpdate()
    {
        if (DataManager.dataList.Count > 0)
        {
            var tempLocalData = DataManager.dataList;

            if (DataManager.dataList.Count > fileTabsList.Count)
            {
                int newTabs = DataManager.dataList.Count - fileTabsList.Count;
                for (int tab = 0; tab < newTabs; tab++)
                {
                    GameObject fileTab = Instantiate(loadFile, tf_ScrollContent);
                    fileTabsList.Add(fileTab);
                    fileTab.SetActive(false);
                }
            }

            for (int index = 0; index < fileTabsList.Count; index++)
            {
                if (index < DataManager.dataList.Count)
                {
                    DataInfo dataInfo = DataManager.dataList[index];
                    GameData fileData = DataInfo.ReadData(dataInfo);
                    Meta fileMeta = fileData.meta;

                    LoadFilesUI loadFilesUI = fileTabsList[index].GetComponent<LoadFilesUI>();

                    if (loadFilesUI != null)
                    {
                        string filPath = DataManager.dataList[index].path;
                        GameObject fileTab = fileTabsList[index];

                        loadFilesUI.InfoUpdate(fileMeta.cityName, fileMeta.daysPlayed, fileMeta.lastPlayDate, dataInfo);
                        loadFilesUI.button_Delete.onClick.RemoveAllListeners();
                        loadFilesUI.button_Delete.onClick.AddListener(() => DeleteFile(dataInfo, fileTab));
                    }

                    fileTabsList[index].SetActive(true);
                }

                else if (index >= DataManager.dataList.Count)                
                {
                    fileTabsList[index].SetActive(false);
                }
            }
        }
    }

    public void LoadMap()
    {
        if (DataManager.Instance.currentGameData != null)
        {
            DataManager.Instance.gameState = DataManager.GameState.LoadMap;
            SceneManager.LoadScene(0);
        }
    }

    public void DeleteFile(DataInfo pathFile, GameObject gameFileUI)
    {
        DataManager.dataList.Remove(pathFile);
        File.Delete(pathFile.path);
        gameFileUI.SetActive(false);
    }
}
