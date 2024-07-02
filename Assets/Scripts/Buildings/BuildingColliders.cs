using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingColliders : CellList
{
    [SerializeField]
    private GameObject buildingCollider;

    private List<GameObject> buildingColliderList = new List<GameObject>();

    // Places the building collider.
    public void PlaceColliders()
    {
        CellToCenterList();

        foreach (Vector3 cellPos in cellCenterPosList)
        {
            GameObject _collider;
            _collider = Instantiate(buildingCollider, cellPos, Quaternion.identity, transform);
            buildingColliderList.Add(_collider);
        }
    }
}
