using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCheck : MonoBehaviour
{
    [SerializeField]
    private BuildingAssets pathAsset;
    private SpriteRenderer pathSprite;

    [SerializeField, Header("Paths sprites")]
    private List<Sprite> pathSprites;
    private List<PathCheck> sc_PathChecksList = new List<PathCheck>();

    private List<Vector3> adjacentPositionsList = new List<Vector3>();

    private Vector3Int lastPos;

    private int indexControl;
    private int[] adjPathsControl = { 1, 2, 4, 8 };
    private int checks = 0;
    public bool isPlaced = false;

    private void Awake()
    {
        pathSprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        isPlaced = false;
    }

    private void Update()
    {
        if (!isPlaced)
        {
            Vector3Int curPos = GameManager.Instance.objectTilemap.WorldToCell(transform.position);

            if (curPos != lastPos)
            {
                UpdatePaths();
            }

            if (!BuildingPlacer.currentlyPlacing)
            {
                isPlaced = true;
            }
        }
    }

    public void UpdatePaths()
    {
        AdjacentPos();
        AdjacentPaths();
        UpdatePath();
        UpdateAdjacentPaths();
    }

    // Collect cells adjacent to the path.
    private void AdjacentPos()
    {
        adjacentPositionsList.Clear();

        Vector3Int _curPos = GameManager.Instance.objectTilemap.WorldToCell(transform.position);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (Mathf.Abs(x) != Mathf.Abs(y))
                {
                    Vector3Int _cellPos = _curPos + new Vector3Int(x, y, 0);
                    Vector3 _cellCenterPos = GameManager.Instance.objectTilemap.GetCellCenterLocal(_cellPos);
                    adjacentPositionsList.Add(_cellCenterPos);
                }
            }
        }
    }

    // Checks if the cell has a path and collects it.
    private void AdjacentPaths()
    {
        // Shoots a raycast to check for adjacents paths.
        for (int i = 0; i < adjacentPositionsList.Count; i++)
        {
            RaycastHit2D hits = Physics2D.Raycast(adjacentPositionsList[i], Vector2.zero);

            if (hits.collider != null)
            {
                PathCheck sc_PathCeck = hits.collider.gameObject.GetComponentInParent<PathCheck>();

                if (sc_PathCeck != null && sc_PathCeck != this)
                {
                    sc_PathChecksList.Add(sc_PathCeck);
                    sc_PathCeck.checks = 0;
                }
            }
        }
    }

    // Updates all the adjacents paths.
    private void UpdateAdjacentPaths()
    {
        if (sc_PathChecksList.Count > 0)
        {
            foreach (var pathCheck in sc_PathChecksList)
            {
                pathCheck.UpdatePath();

                if (checks > 2) //Updates twice in case the path is removed.
                {
                    sc_PathChecksList.Remove(pathCheck);
                }
            }
        }
    }

    // Updates the path sprite.
    private void UpdatePath()
    {

        if (BuildingPlacer.isPlaced)
        {
            checks++;
        }

        indexControl = 0;

        // Shoots a raycast to check for adjacents paths.
        for (int i = 0; i < adjacentPositionsList.Count; i++)
        {
            RaycastHit2D hits = Physics2D.Raycast(adjacentPositionsList[i], Vector3.forward);
            if (hits.collider != null)
            {
                PathCheck sc_PathCeck = hits.collider.gameObject.GetComponentInParent<PathCheck>();

                if (sc_PathCeck != null)
                {
                    indexControl += adjPathsControl[i];
                }
            }
        }

        pathSprite.sprite = pathAsset.buildingPlaced[indexControl];
    }
}