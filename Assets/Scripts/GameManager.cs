using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Camera cam;

    public bool randomSeed;
    public int seed = 0;

    [Header("Scripts and Assets")]
    public ChunkAssets chunks;
    public MapManager mapManager;
    public UIDirector UIDirector;
    public BuildingPlacer buildingPlacer;
    public ResourceMenuUI sc_ResourceMenuList;
    public BuildingsMenuUI sc_BuildingsMenuList;
    public ResourceManager sc_ResourceManager;

    [Header("Transforms")]
    public Transform tf_Buildings;
    public Transform tf_Nature;

    [Header("Tilemaps")]
    public Grid grid;
    public Tilemap terrainTilemap;
    public Tilemap forestTilemap;
    public Tilemap guideTilemap;
    public Tilemap objectTilemap;

    [Header("Tiles")]
    public TileBase grassTiles;
    public TileBase waterTiles;
    public TileBase guideTile;
    public TileBase trees;
    public TileBase emptyTile;

    public void SaveGame()
    {
        sc_ResourceManager?.SaveResourceData(DataManager.Instance.currentGameData.data.info);
        DataManager.Instance.SaveData();
    }

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }

        buildingPlacer = GetComponent<BuildingPlacer>();
    }


    public static void GeneratorSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }

#if UNITY_EDITOR
    public void GeneratorSeed(bool randomSeed = true)
    {
        if (randomSeed)
        {
            seed = RandomSeed();
        }

        UnityEngine.Random.InitState(seed);
    }

    public static int RandomSeed()
    {
        int seed;
        System.Random _seed = new System.Random();
        seed = _seed.Next();
        return seed;
    }

    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GameManager manager = (GameManager)target;
            EditorGUILayout.Space();
            if (GUILayout.Button("Generate Seed"))
            {
                manager.GeneratorSeed();
            }
        }
    }
#endif
}
