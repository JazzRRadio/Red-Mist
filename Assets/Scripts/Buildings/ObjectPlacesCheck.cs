using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectPlacesCheck : CellList
{
    private void OnDisable()
    {
        CleanObjectsTilemap();
    }

    public static bool CanBePlacedHere(Vector3Int position)
    {
        bool isGrassTile = GameManager.Instance.terrainTilemap.GetTile(position) == GameManager.Instance.grassTiles;
        bool noTrees = !GameManager.Instance.forestTilemap.HasTile(position);
        bool noObjects = !GameManager.Instance.objectTilemap.HasTile(position);
        bool noMist = GameManager.Instance.terrainTilemap.GetColor(position) == Color.white;

        return isGrassTile && noTrees && noObjects && noMist;
    }

    public bool CanBePlaced()
    {
        CellListMaker();

        bool canPlace = false;

        int index = 0;

        foreach (Vector3Int cellPos in cellPosList)
        {
            // Check if the terrain of the tile is grass
            if (GameManager.Instance.terrainTilemap.GetTile(cellPos) != GameManager.Instance.grassTiles)
                break;

            // Check if there isn't a tree in the tile.
            if (GameManager.Instance.forestTilemap.HasTile(cellPos))
                break;

            // Check if the object overlaps another object.
            if (GameManager.Instance.objectTilemap.HasTile(cellPos))
                break;

            if (GameManager.Instance.terrainTilemap.GetColor(cellPos) != Color.white)
                break;

            index++;

            if (index >= cellPosList.Count)
            {
                canPlace = true;
            }
        }

        return canPlace;
    }

    public void PlaceOccupiedTiles()
    {
        foreach (var cellPos in cellPosList)
        {
            PlaceOccupiedTitesHere(cellPos);
        }
    }

    public static void PlaceOccupiedTitesHere(Vector3Int cellPos)
    {
        GameManager.Instance.objectTilemap.SetTile(cellPos, GameManager.Instance.emptyTile);
    }

    private void CleanObjectsTilemap()
    {
        foreach (var cellPos in cellPosList)
        {
            if (GameManager.Instance.objectTilemap != null)
            {
                GameManager.Instance.objectTilemap.SetTile(cellPos, null);
            }
        }
    }


}
