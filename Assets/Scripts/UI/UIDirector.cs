using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDirector : MonoBehaviour
{
    public ResourceMenuUI sc_ResourceMenuList;
    public BuildingsMenuUI sc_BuildingsMenuList;
    public GameObject gameUI;
    public GameObject pauseMenu;

    public static UIDirector Instance { get; private set; }
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InGameUI();
            pauseMenu.SetActive(!pauseMenu.activeSelf);   
        }
    }

    public void InGameUI()
    {
        bool state = gameUI.activeSelf;
        gameUI.SetActive(!state);
    }
}