using System.Collections.Generic;
using UnityEngine;

public class ResourceMenuUI : LevelsUI
{
    [Header("Resource")]
    [SerializeField]
    private GameObject resourceLevelMenu;
    [SerializeField]
    private GameObject resourcesMenu;

    private List<ResourceUI> scList_lvlResources = new List<ResourceUI>();
    private List<ResourceUI> scList_Resources = new List<ResourceUI>();


    private void Awake()
    {
        TakeResourceUIScript(scList_lvlResources, resourceLevelMenu);
        TakeResourceUIScript(scList_Resources, resourcesMenu);
    }

    public void InitResources()
    {
       UpdateAllResources(scList_Resources);
       UpdateAllTooltips(scList_Resources);
    }

    private void OnEnable()
    {
        OnClickAction += UpdateMenuResources;
    }
    private void OnDisable()
    {
        OnClickAction -= UpdateMenuResources;
    }

    public void UpdateMenuResources()
    {
        UpdateAllResources(scList_lvlResources, actualLevel);
        UpdateAllTooltips(scList_lvlResources, actualLevel);
    }

    // Update all resource in resourceUI list by level
    public void UpdateAllResources(List<ResourceUI> resourceUIList, int level = 0)
    {
        foreach (ResourceUI resource in resourceUIList)
        {
            resource.UpdateResource(level);
        }
    }

    // Update all tooltip in resourceUI list by level
    public void UpdateAllTooltips(List<ResourceUI> resourceUIList, int level = 0)
    {
        foreach (ResourceUI resource in resourceUIList)
        {
            resource.ResourceStats(level);
        }
    }

    // Takes all resource assets from a ResourceUI list
    private void TakeResourceUIScript(List<ResourceUI> resourceUIList, GameObject go)
    {
        ResourceUI[] sc_Resource = go.GetComponentsInChildren<ResourceUI>();
        resourceUIList.AddRange(sc_Resource);
    }

}
