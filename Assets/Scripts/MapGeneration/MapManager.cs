using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [Header("Chunks")]
    [SerializeField]
    private ChunkAssets chunks;

    [Header("Tilemaps")]
    [SerializeField]
    private Vector3Int mapCenter = new Vector3Int(0, 0, 0);

    [Header("Tiles")]
    [SerializeField]
    private Color tileColor;

    [Header("Chunk Var")]
    [SerializeField]
    private int initialGeneratedRadius = 4;
    public static int chunkRadius = 5;
    public static int chunkSize
    {
        get { return chunkRadius * 2 + 1; }
    }
    private Chunk startChunk = new Chunk();


    [Header("Forest Var")]
    [SerializeField]
    private int forestMinRadius = 10;
    [SerializeField]
    private int forestMaxRadius = 14;
    private float forestDispersionRate = 0.05f;

    [Header("River Var")]
    [SerializeField]
    private int riverTurningRange = 3;
    [SerializeField]
    private List<RiversBeds> riverBedsList;
    [SerializeField]
    private int deforestRadius = 2;
    [SerializeField]
    private int riverWidth = 1;

    [Header("Crystal Prefab")]
    [SerializeField]
    private GameObject crystalPrefab;
    private RandomPlacement sc_CrystalPlacement;

    private void Awake()
    {
        sc_CrystalPlacement = crystalPrefab.GetComponent<RandomPlacement>();
    }

    private void OnEnable()
    {
        GameManager.GeneratorSeed(DataManager.Instance.currentGameData.data.info.mapSeed);
        GenerateMap();

        if (DataManager.Instance.currentGameData.data.info.mapExtensions.Count > 0)
        {
            foreach (var extension in DataManager.Instance.currentGameData.data.info.mapExtensions)
            {
                Debug.Log(extension.extensionRadius + " / " + extension.centralChunk);

                MapExtension(extension);
            }
        }
    }

    // Generates the map from the chunks types
    private void chunkGenerator(bool newMap = false)
    {
        foreach (var _chunk in chunks.assignedChunkList)
        {
            if (!_chunk.isPrepared)
            {
                if (_chunk.chunkType == Chunk.ChunkTypes.Forest)
                {
                    ForestGenerator(_chunk);
                }

                switch (_chunk.chunkType)
                {
                    case Chunk.ChunkTypes.Plain:
                    case Chunk.ChunkTypes.Forest:
                        {
                            TileGeneratorFromChunk(_chunk, GameManager.Instance.grassTiles, GameManager.Instance.terrainTilemap);
                            if (_chunk.startChunk)
                            {
                                startChunk = _chunk;
                            }
                            break;
                        }
                    case Chunk.ChunkTypes.Water:
                        {
                            TileGeneratorFromChunk(_chunk, GameManager.Instance.waterTiles, GameManager.Instance.terrainTilemap);
                            break;
                        }
                    default:
                        break;

                }

                _chunk.isPrepared = true;

                // Checks if any of the new chunks are the continuation of a river.
                if (riverBedsList.Exists(_riverCourse => _riverCourse.lastChunkPosition == _chunk.chunkPosition))
                {
                    RiversBeds _riverCourse = riverBedsList.Find(x => x.lastChunkPosition == _chunk.chunkPosition);
                    _riverCourse.finalized = false;
                }

            }
        }

        if (newMap) RiverInitiation();

        RiverExpand();
    }

    // Generates the forest.
    private void ForestGenerator(Chunk _chunk)
    {
        int _forestRadius;
        Vector3Int randomSeedPosition;

        _forestRadius = Random.Range(forestMinRadius, forestMaxRadius + 1);
        randomSeedPosition = RandomPosition(2, -2, _chunk.chunkCenter);

        for (int x = -_forestRadius; x <= _forestRadius; x++)
        {
            for (int y = -_forestRadius; y <= _forestRadius; y++)
            {
                Vector3Int _treePos = randomSeedPosition + new Vector3Int(x, y, 0);

                float _dist = Vector3Int.Distance(_treePos, randomSeedPosition);
                float possibleTree = Random.Range(0.0f, 1.0f);
                float ratioTree = ((_forestRadius - _dist) / _forestRadius) - forestDispersionRate; // The further away the tree is from the seed, the less lykely it will be placed.
                if (ratioTree > possibleTree)
                {
                    GameManager.Instance.forestTilemap.SetTile(_treePos, GameManager.Instance.trees);
                    GameManager.Instance.forestTilemap.SetColor(_treePos, tileColor);
                }
            }
        }
    }

    // Generates two opposite river beds on a new map.
    private void RiverInitiation()
    {
        {
            riverBedsList.Clear();

            int randomIndex = Random.Range(0, chunks.riverStartsChunks.Count);
            Chunk chunkStart = chunks.riverStartsChunks[randomIndex];
            Vector3Int riverStart = RandomPosition(chunkRadius, -chunkRadius, chunkStart.chunkCenter);

            int riverDirection = Random.Range(0, 8); // See possibleDirectionsRange() comments.

            BedsInitiation(riverStart, riverDirection);
            BedsInitiation(riverStart, riverDirection + 4);
        }
    }

    // Generates a new river course.
    private void BedsInitiation(Vector3Int startPosition, int riverDirection)
    {
        RiversBeds riverCourse = new RiversBeds();
        riverCourse.bedsDirections = possibleDirectionsRange(riverDirection);
        riverCourse.startPosition = startPosition;
        riverBedsList.Add(riverCourse);
    }

    // Expands all incomplete river courses.
    private void RiverExpand()
    {
        foreach (var river in riverBedsList)
        {
            if (river.finalized == false)
            {
                if (river.lastPosition == default)
                {
                    river.lastPosition = river.startPosition;
                }

                Vector3Int direction = Vector3Int.zero;
                Vector3Int tilePosisiton = river.lastPosition;

                int courseChangeIndex = 0;
                bool noMoreTerrain = false;

                while (!noMoreTerrain)
                {
                    tilePosisiton += direction;
                    Vector3Int _tileChunkPosition = TileToChunkPosition(tilePosisiton);

                    // Generate the river course until there is no land.
                    if (chunks.assignedChunkList.Exists(_chunk => _chunk.chunkPosition == _tileChunkPosition)) // Check if the next central river tile is on an existing chunk.
                    {
                        TileGeneratorFromLenght(deforestRadius, tilePosisiton, null, GameManager.Instance.forestTilemap);
                        TileGeneratorFromLenght(riverWidth, tilePosisiton, GameManager.Instance.waterTiles, GameManager.Instance.terrainTilemap, false);
                    }

                    else // Save the last river position in case it need to continue in an extension of the map.
                    {
                        noMoreTerrain = true;
                        river.finalized = true;
                        river.lastPosition = tilePosisiton;
                        river.lastChunkPosition = TileToChunkPosition(river.lastPosition);
                    }

                    courseChangeIndex += Random.Range(-1, 2);
                    if (courseChangeIndex < 0) courseChangeIndex = 0;
                    if (courseChangeIndex >= river.bedsDirections.Count) courseChangeIndex = river.bedsDirections.Count - 1;
                    direction = river.bedsDirections[courseChangeIndex];
                }
            }
        }
    }

    // Puts a grid for reference.
    private void TileGuideGenerator()
    {
        foreach (var _chunk in chunks.assignedChunkList)
        {
            TileGeneratorFromChunk(_chunk, GameManager.Instance.guideTile, GameManager.Instance.terrainTilemap);
        }
    }

    private void GenerateMap()
    {
        ClearAllMap();

        chunks.ChunkReset();

        chunks.ChunkInitiation(initialGeneratedRadius, mapCenter);

        chunks.chunkTypeAssignment();

        chunkGenerator(true);

        sc_CrystalPlacement?.RandomPlace(startChunk);
    }

    private void MapExtension(int generationRadius, Vector3Int generationCenterChunkPosition, bool save = false)
    {
        chunks.ChunkInitiation(generationRadius, generationCenterChunkPosition);

        chunks.chunkTypeAssignment();

        chunkGenerator();

        if (save)
        {
            Debug.Log(generationRadius + " + " + generationCenterChunkPosition);
            Extension newExtension = Extension.CreateMapExtension(generationRadius, generationCenterChunkPosition);
            DataManager.Instance.currentGameData.data.info.mapExtensions.Add(newExtension);
            DataManager.Instance.SaveData();
        }
    }

    private void MapExtension(Extension mapExtension)
    {
        MapExtension(mapExtension.extensionRadius, mapExtension.centralChunk);
    }

    private void ClearAllMap()
    {
        Tilemap[] tilemapList = GameManager.Instance.grid.transform.GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemapList)
        {
            tilemap.ClearAllTiles();
        }
    }

    // Lays out a square of n lenght with a tile.
    private void TileGeneratorFromLenght(int radius, Vector3Int centerPos, TileBase _tile, Tilemap _tilemap, bool needTerrain = false)
    {

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                bool forSet = true;
                Vector3Int _tilePos = centerPos + new Vector3Int(x, y, 0);
                if (needTerrain)
                {
                    if (!_tilemap.HasTile(_tilePos))
                    {
                        forSet = false;
                    }
                }

                if (forSet)
                {
                    _tilemap.SetTile(_tilePos, _tile);
                    _tilemap.SetColor(_tilePos, tileColor);
                }

            }
        }
    }

    // Lays the entire chunk with a tile.
    private void TileGeneratorFromChunk(Chunk _chunk, TileBase _tile, Tilemap _tilemap)
    {
        TileGeneratorFromLenght(chunkRadius, _chunk.chunkCenter, _tile, _tilemap);
    }

    // Generates the possible directions of a river in one direction. 
    // 7 = (+1,-1) / 0 = (+1,+0) / 1 = (+1,+1)
    // 6 = (+0,-1) / * = (+0,+0) / 2 = (+0,+1)
    // 5 = (-1,-1) / 4 = (-1,+0) / 3 = (-1,+1)
    private List<Vector3Int> possibleDirectionsRange(int direction)
    {
        List<Vector3Int> directionsRange = new List<Vector3Int>();

        for (int i = 1; i <= riverTurningRange; i++)
        {
            float angle = (Mathf.PI / 4) * direction;
            int x = Mathf.RoundToInt(Mathf.Cos(angle));
            int y = Mathf.RoundToInt(Mathf.Sin(angle));
            Vector3Int possibleDirection = new Vector3Int(x, y, 0);
            directionsRange.Add(possibleDirection);

            direction++;
        }

        return directionsRange;
    }

    public static Vector3Int RandomPosition(int maxValor, int minValor, Vector3Int centerPosition)
    {
        Vector3Int _randomPosition;
        int _randomPositionX = Random.Range(minValor, maxValor + 1);
        int _randomPositionY = Random.Range(minValor, maxValor + 1);
        _randomPosition = centerPosition + new Vector3Int(_randomPositionX, _randomPositionY, 0);
        return _randomPosition;
    }

    private Vector3Int TileToChunkPosition(Vector3Int _tile)
    {
        Vector3 _tileF = _tile; // Need to convert the vector to float so that the division turns out correctly.
        Vector3Int chunkPosition = Vector3Int.RoundToInt(_tileF / chunkSize);
        return chunkPosition;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MapManager))]
    public class MapManagerEditor : Editor
    {
        int expRadius = 0;
        Vector3Int expCenter = new Vector3Int();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            MapManager mapManager = (MapManager)target;

            EditorGUILayout.Space(20);
            if (GUILayout.Button("New Map"))
            {
                mapManager.GenerateMap();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Guide Tiles"))
            {
                mapManager.TileGuideGenerator();
            }

            EditorGUILayout.Space();
            expRadius = EditorGUILayout.IntField("Extension Radius", expRadius);
            expCenter = EditorGUILayout.Vector3IntField("Extension central Chunk", expCenter);

            if (GUILayout.Button("Map Extension"))
            {
                mapManager.MapExtension(expRadius, expCenter, true);
            }

        }
    }
#endif
}

[System.Serializable]
public class RiversBeds
{
    public Vector3Int startPosition;
    public Vector3Int lastPosition;

    public Vector3Int lastChunkPosition;

    public List<Vector3Int> bedsDirections;

    public bool finalized = false;
}

[System.Serializable]
public class Extension
{
    public int extensionRadius;
    public Vector3Int centralChunk;

    public static Extension CreateMapExtension(int _extensionRadius, Vector3Int _centralChunk)
    {
        Extension mapExt = new Extension();
        mapExt.extensionRadius = _extensionRadius;
        mapExt.centralChunk = _centralChunk;
        return mapExt;
    }
}