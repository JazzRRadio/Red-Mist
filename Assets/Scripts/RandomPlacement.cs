using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlacement : MonoBehaviour
{
    [SerializeField]
    private Transform tf_Elements;

    // Places an element randomly in a chunk.
    public void RandomPlace(Chunk chunk)
    {
        List<Vector3Int> posiblePositions = new List<Vector3Int>();
        bool isPlaced = false;

        for (int x = -MapManager.chunkRadius; x <= MapManager.chunkRadius; x++)
        {
            for (int y = -MapManager.chunkRadius; y <= MapManager.chunkRadius; y++)
            {
                Vector3Int randomPosition = chunk.chunkCenter + new Vector3Int(x, y, 0);
                posiblePositions.Add(randomPosition);
            }
        }

        while (!isPlaced)
        {
            int randomIndex = Random.Range(0, posiblePositions.Count);

            Vector3Int posiblePosition = posiblePositions[randomIndex];

            if (GameManager.Instance.terrainTilemap.GetTile(posiblePosition) == GameManager.Instance.grassTiles && !GameManager.Instance.forestTilemap.HasTile(posiblePosition))
            {
                Vector3 _pos = GameManager.Instance.grid.GetCellCenterWorld(posiblePosition);
                GameObject _object = Instantiate(gameObject, _pos, Quaternion.identity, tf_Elements);
                if (_object != null)
                {
                    ObjectPlacesCheck crystalPlacesCheck = _object.GetComponent<ObjectPlacesCheck>();
                    if (crystalPlacesCheck != null)
                    {
                        crystalPlacesCheck.CellListMaker();
                        crystalPlacesCheck.PlaceOccupiedTiles();
                    }
                }

                isPlaced = true;
            }

            else if (GameManager.Instance.terrainTilemap.GetTile(posiblePosition) != GameManager.Instance.grassTiles)
            {
                posiblePositions.Remove(posiblePosition);
            }
        }
    }
}
