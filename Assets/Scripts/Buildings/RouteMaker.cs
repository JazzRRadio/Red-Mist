using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteMaker : MonoBehaviour
{

    [SerializeField]
    private Transform tf_PathPool;
    private BuildingPlacer sc_BuildingPlacer;
    private GameObject pathPrefab;

    private Vector3Int pathCellStart;
    private Vector3Int pathRouteVector;
    private Vector3Int pathRouteVectorCheck;

    private List<GameObject> pathList = new List<GameObject>();

    public static bool routing = false;

    private void Awake()
    {
        sc_BuildingPlacer = GetComponent<BuildingPlacer>();
    }

    private void OnEnable()
    {
        routing = false;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && BuildingPlacer.currentlyPlacing && BuildingPlacer.isPath)
        {
            PathRoute();
        }

        if (Input.GetMouseButtonUp(0) && routing)
        {
            PathPooling();
        }
    }

    // Route maker.
    private void PathRoute()
    {
        if (!routing)
        {
            pathCellStart = BuildingPlacer.GetCellPosition();
            pathPrefab = sc_BuildingPlacer.go_BuildingLayout;
        }

        routing = true;

        int pathCount = 0;

        Vector3Int directionPath = new Vector3Int();
        Vector3Int pathCellEnd = BuildingPlacer.GetCellPosition();

        pathRouteVector = pathCellEnd - pathCellStart;

        if (pathRouteVector != pathRouteVectorCheck)
        {
            // Checks the direction of the route based on the position of the cursor.
            int pathRouteHorizontal = Mathf.Abs(pathRouteVector.x);
            int pathRouteVertical = Mathf.Abs(pathRouteVector.y);

            if (pathRouteVector != Vector3Int.zero)
            {
                pathCount = pathRouteHorizontal >= pathRouteVertical ? pathRouteVector.x : pathRouteVector.y;
                directionPath = pathRouteHorizontal >= pathRouteVertical ? Vector3Int.right : Vector3Int.up;
            }

            // Makes the components of the route (lenght and direction)
            if (pathCount != 0)
            {
                int routeLenght = Mathf.Abs(pathCount);
                int orientationPath = pathCount / routeLenght;
                directionPath *= orientationPath;

                if (routeLenght > pathList.Count)
                {
                    int pathNeed = routeLenght - pathList.Count;

                    for (int path = 0; path <= pathNeed; path++)
                    {
                        GameObject newPath = Instantiate(pathPrefab, transform.position, Quaternion.identity, tf_PathPool);
                        newPath.SetActive(false);
                        BuildingColliders sc_BuildingColliders = newPath.GetComponent<BuildingColliders>();
                        if (sc_BuildingColliders != null)
                        {
                            sc_BuildingColliders.PlaceColliders();
                        }

                        pathList.Add(newPath);
                    }
                }

                // Places the paths. If the path can't be placed, it disables it.
                for (int index = 0; index < pathList.Count; index++)
                {
                    if (index < routeLenght - 1)
                    {
                        Vector3Int _pathCell = pathCellStart + (directionPath * (index + 1));
                        Vector3 _pathPos = GameManager.Instance.objectTilemap.GetCellCenterLocal(_pathCell);
                        pathList[index].transform.position = _pathPos;

                        bool _canPlaced = ObjectPlacesCheck.CanBePlacedHere(_pathCell);
                        pathList[index].SetActive(_canPlaced);
                        PathCheck sc_PathCheck = pathList[index].GetComponent<PathCheck>();
                        if (sc_PathCheck != null)
                        {
                            sc_PathCheck.UpdatePaths();
                            sc_PathCheck.isPlaced = true;
                        }
                    }

                    else
                    {
                        pathList[index].SetActive(false);
                    }
                }
            }

            pathRouteVectorCheck = pathRouteVector;
        }
    }

    // Manages the paths pooling and completes path placement.
    private void PathPooling()
    {
        foreach (var path in pathList)
        {
            Transform tf = path.activeSelf ? GameManager.Instance.tf_Buildings : tf_PathPool;

            path.transform.SetParent(tf);

            if (path.activeSelf)
            {
                ObjectPlacesCheck.PlaceOccupiedTitesHere(GameManager.Instance.objectTilemap.WorldToCell(path.transform.position));
            }
        }

        pathList.Clear();

        routing = false;
    }
}
