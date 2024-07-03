using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameInfo;

public class BuildingPlacer : MonoBehaviour
{
    // Building components
    [SerializeField]
    private Transform tf_BuildingsParent;
    protected BuildingAssets buildingLayoutAsset;
    [HideInInspector]
    public GameObject go_BuildingLayout;
    private SpriteRenderer sr_BuildingSpriteRenderer;
    private ObjectPlacesCheck sc_BuildingCheck;
    private BuildingColliders sc_BuildingColliders;

    // Variables for the building indicator refresh.
    private float placementIndicatorUpdateRate = 0.1f;
    private float lastUpdateTime;

    public static Vector3 curPlacamentPos;
    private Vector3 previousPlacamentPos = new Vector3();
    private Color spriteColor = Color.white;

    public static bool currentlyPlacing;
    public static bool isPlaced;
    public static bool isPath;

    private void OnEnable()
    {
        currentlyPlacing = false;
        isPlaced = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }

        if (Time.time - lastUpdateTime > placementIndicatorUpdateRate && currentlyPlacing)
        {
            lastUpdateTime = Time.time;

            if (!RouteMaker.routing)
            {
                MoveEntity();
            }
        }      

        if (Input.GetMouseButtonUp(0) && currentlyPlacing && sc_BuildingCheck.CanBePlaced() && !EventSystem.current.IsPointerOverGameObject())
        {
            PlaceBuilding();
        }
    }

    public void BeginNewPlacement(BuildingAssets buildingAsset)
    {
        bool isAfforable = CostCheck.CostChecking(buildingAsset);

        if (!currentlyPlacing && isAfforable)
        {
            currentlyPlacing = true;
            isPlaced = false;
            buildingLayoutAsset = buildingAsset;

            // Take the components from the entity
            go_BuildingLayout = Instantiate(buildingLayoutAsset.buildingPrefab, curPlacamentPos, Quaternion.identity, tf_BuildingsParent);
            go_BuildingLayout.name = buildingAsset.name;

            sr_BuildingSpriteRenderer = go_BuildingLayout.GetComponent<SpriteRenderer>();
            ChangeSprite(buildingLayoutAsset.buildingFloating);

            sc_BuildingCheck = go_BuildingLayout.GetComponent<ObjectPlacesCheck>();
            sc_BuildingColliders = go_BuildingLayout.GetComponent<BuildingColliders>();
            isPath = go_BuildingLayout.GetComponent<PathCheck>() ? true : false;

            if (sc_BuildingCheck != null)
            {
                sc_BuildingCheck.height = buildingLayoutAsset.height;
                sc_BuildingCheck.width = buildingLayoutAsset.width;
            }

            if (sc_BuildingColliders != null)
            {
                sc_BuildingColliders.height = buildingLayoutAsset.height;
                sc_BuildingColliders.width = buildingLayoutAsset.width;

                sc_BuildingColliders.PlaceColliders();
            }
        }

        else if (currentlyPlacing)
        {
            CancelPlacement();
        }
    }

    // Cancel entity placament.
    private void CancelPlacement()
    {
        currentlyPlacing = false;
        Destroy(go_BuildingLayout);
    }

    // Move the entity.
    private void MoveEntity()
    {
        curPlacamentPos = GetCellCenterPosition();
        if (curPlacamentPos != previousPlacamentPos)
        {
            go_BuildingLayout.transform.position = curPlacamentPos;
            previousPlacamentPos = curPlacamentPos;

            if (sc_BuildingCheck.CanBePlaced())
            {
                SpriteTransparency(1f);
            }

            else if (!sc_BuildingCheck.CanBePlaced())
            {
                SpriteTransparency(0.5f);
            }
        }
    }

    // Placed the entity.
    private void PlaceBuilding()
    {
        CostCheck.RemoveCost(buildingLayoutAsset);

        currentlyPlacing = false;

        isPlaced = true;

        if (!isPath)
        {
            ChangeSprite(buildingLayoutAsset.buildingPlaced);
        }

        if (sc_BuildingCheck != null)
        {
            sc_BuildingCheck.PlaceOccupiedTiles();
        }

        BuildingsData buildingData = SaveBuildingData(curPlacamentPos, buildingLayoutAsset);
        DataManager.Instance.SaveData(buildingData);
    }

    private BuildingsData SaveBuildingData(Vector3 position, BuildingAssets building)
    {
        BuildingsData data = new BuildingsData();
        data.buildingPosition = position;
        data.buildingName = building.name;
        data.buildingID = go_BuildingLayout.GetInstanceID();
        return data;
    }

    private void SpriteTransparency(float alphaLevel)
    {
        spriteColor.a = alphaLevel;
        sr_BuildingSpriteRenderer.color = spriteColor;
    }

    private void ChangeSprite(List<Sprite> spriteList, int index = 0)
    {
        sr_BuildingSpriteRenderer.sprite = spriteList[index];
    }

    // Returns the cell under the cursor.
    public static Vector3Int GetCellPosition()
    {
        Camera cam = GameManager.Instance.cam;
        Vector3Int cellPosition = GameManager.Instance.objectTilemap.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));
        cellPosition.z = 0;

        return cellPosition;
    }

    // Returns the center of the cell under the cursor.
    public static Vector3 GetCellCenterPosition()
    {
        Vector3Int _cellPosition = GetCellPosition();
        Vector3 tileCenterPosition = GameManager.Instance.objectTilemap.GetCellCenterLocal(_cellPosition);
        return tileCenterPosition;
    }
}