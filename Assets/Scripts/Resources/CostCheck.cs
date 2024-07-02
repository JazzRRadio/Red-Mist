using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuildingAssets;

public class CostCheck : MonoBehaviour
{
    public static bool CostChecking(BuildingAssets building)
    {
        bool affordable = true;

        foreach (ResourcesAmount _resource in building.recousesCost)
        {
            affordable = _resource.resource.CheckCost(_resource.amount, _resource.level);

            if (affordable == false) break;
        }

        return affordable;
    }

    public static void RemoveCost(BuildingAssets building)
    {
        foreach (ResourcesAmount _resource in building.recousesCost)
        {
            // IMPORTANT: The cost must be positive, and in this void, it must have a minus sign ( - ).
            _resource.resource.ChangeAmount(-_resource.amount, _resource.level);
        }
    }

    public static void Purchase(BuildingAssets building)
    {
        bool isAfforable = CostChecking(building);

        if (isAfforable)
        {
            RemoveCost(building);
        }
    }
}
