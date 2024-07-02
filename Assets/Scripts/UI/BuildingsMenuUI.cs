using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsMenuUI : LevelsUI
{
    [Header("Buildings")]
    [SerializeField]
    private GameObject buildingButton;
    [SerializeField]
    private GameObject buildingTooltip;
    [SerializeField]
    private Transform tf_buildingButtonMenu;

    [SerializeField]
    private List<BuildingPerLevel> buildingsList;
    private List<GameObject> buildingButtonsList = new List<GameObject>();

    [System.Serializable]
    private struct BuildingPerLevel
    {
        public List<BuildingAssets> buildingsPerLevel;
    }

    private void OnEnable()
    {
        OnClickAction += ShowBuilding;
    }

    private void OnDisable()
    {
        OnClickAction -= ShowBuilding;
    }

    private void ShowBuilding()
    {
        ShowBuilding(actualLevel);
    }

    // Put the buildings placement void on the buttons.
    private void ShowBuilding(int level = 0)
    {
        ResetBuildingButton();
        level--;

        BuildingPerLevel buildings = buildingsList[level];

        for (int i = 0; i < buildings.buildingsPerLevel.Count; i++)
        {
            GameObject _buildingButton;
            BuildingAssets _buildingAssets = buildings.buildingsPerLevel[i];
            if (i < buildingButtonsList.Count)
            {
                _buildingButton = buildingButtonsList[i];
            }
            else
            {
                _buildingButton = Instantiate(buildingButton, tf_buildingButtonMenu);
                buildingButtonsList.Add(_buildingButton);
                Tooltip buildingTooltips = _buildingButton.GetComponent<Tooltip>();
                buildingTooltips.tooltip = buildingTooltip;
            }

            var buttonSprite = _buildingButton.GetComponent<Image>();
            buttonSprite.sprite = _buildingAssets.buildingPlaced[0];
            
            var _button = _buildingButton.GetComponent<Button>();
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => GameManager.Instance.buildingPlacer.BeginNewPlacement(_buildingAssets));

            buildingButton.SetActive(true);
        }


    }

    private void ResetBuildingButton()
    {
        if (buildingButtonsList != null)
        {
            foreach (GameObject button in buildingButtonsList)
            {
                button.SetActive(false);
            }
        }
    }
}
