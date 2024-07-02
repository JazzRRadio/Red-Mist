using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Game Assests/Building Preset")]

public class BuildingAssets : ScriptableObject
{
    [Header("Level")]
    public int level;

    [Header("Dimensions")]
    public int height;
    public int width;

    [Header("Cost")]
    [Header("The cost amount must be positive. If a gain, must be negative")]
    public List<ResourcesAmount> recousesCost;

    [Header("BuildingPrefab")]
    public GameObject buildingPrefab;
    public List <Sprite> buildingPlaced;
    public List <Sprite> buildingFloating;

    [System.Serializable]
    public struct ResourcesAmount
    {
        public ResourceAssets resource;
        public int level;
        public int amount;
        [HideInInspector]
        public float modifiers;
    }
}
