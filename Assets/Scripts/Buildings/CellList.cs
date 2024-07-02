using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellList : MonoBehaviour
{
    public int height, width;

    protected List<Vector3Int> cellPosList = new List<Vector3Int>();
    protected List<Vector3> cellCenterPosList = new List<Vector3>();

    // Makes the list of cell that the building occupies.
    public void CellListMaker()
    {
        cellPosList.Clear();

        // Adjust the position of the object to the grind
        Vector3Int posCorrection = GameManager.Instance.objectTilemap.WorldToCell(transform.position);
        transform.position = GameManager.Instance.objectTilemap.GetCellCenterLocal(posCorrection);

        if (height > 0 && width > 0)
        {
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    Vector3Int _cellPos = posCorrection + new Vector3Int(x, y, 0);

                    cellPosList.Add(_cellPos);
                }
            }
        }
    }

    // Take out the center of the building cell.
    protected void CellToCenterList()
    {
        cellCenterPosList.Clear();

        // Adjust the position of the object to the grind
        Vector3Int posCorrection = GameManager.Instance.objectTilemap.WorldToCell(transform.position);
        transform.position = GameManager.Instance.objectTilemap.GetCellCenterLocal(posCorrection);

        if (height > 0 && width > 0)
        {
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    Vector3Int _cellPos = posCorrection + new Vector3Int(x, y, 0);

                    Vector3 _cellCenterPos = GameManager.Instance.objectTilemap.GetCellCenterLocal(_cellPos);

                    cellCenterPosList.Add(_cellCenterPos);
                }
            }
        }          
    }
}