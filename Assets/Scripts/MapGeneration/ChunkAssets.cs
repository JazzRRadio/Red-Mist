using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Chunks", menuName = "Chunks", order = 1)]

public class ChunkAssets : ScriptableObject
{
    [Header("Map Var")]
    private Vector3Int mapCenter = new Vector3Int(0, 0, 0);
    [SerializeField]
    private int generatedRadius = 1;

    [Header("Forest Var")]
    [SerializeField, Range(0, 1)]
    private float forestRatio;
    [SerializeField]
    private int forestLimit;

    [HideInInspector]
    public List<Chunk> unassignedChunkList;
    public List<Chunk> assignedChunkList;
    //[HideInInspector]
    public List<Chunk> riverStartsChunks;

    // Clears chunk lists.
    public void ChunkReset()
    {
        unassignedChunkList.Clear();
        assignedChunkList.Clear();
        riverStartsChunks.Clear();
    }

    //Create a square of chunks with side equal to the radius.
    public void ChunkInitiation(int generatedRadius, Vector3Int chunkCenter)
    {
        for (int x = -generatedRadius; x <= generatedRadius; x++)
        {
            for (int y = -generatedRadius; y <= generatedRadius; y++)
            {
                Vector3Int chunkOrder = new Vector3Int(x, y, 0);
                Vector3Int _chunkPosition = chunkCenter + chunkOrder;

                if (!assignedChunkList.Exists(_chunk => _chunk.chunkPosition == _chunkPosition))
                {
                    Chunk _chunk = new Chunk();
                    _chunk.chunkPosition = _chunkPosition;

                    unassignedChunkList.Add(_chunk);
                }
                else { };
            }
        }
    }

    // Assigns the type to the list chunks.
    public void chunkTypeAssignment()
    {
        int _forestLimit = 0; // With this prevents that many chunks of forests are created in a row if the ratio increases.
        int unassignedChunkListCount = unassignedChunkList.Count;
        riverStartsChunks.Clear();

        for (int i = 0; i < unassignedChunkListCount; i++) // The chunks are randomly assigned to make the forests more organic.
        {
            int randomIndex = Random.Range(0, unassignedChunkList.Count);

            Chunk _chunk = unassignedChunkList[randomIndex];

            if (_chunk.chunkPosition == mapCenter)
            {
                _chunk.startChunk = true; // The center chunk.
            }

            else if (Vector3Int.Distance(_chunk.chunkPosition, mapCenter) <= Mathf.Sqrt(2.0f))
            {
                _chunk.chunkType = Chunk.ChunkTypes.Plain; //The chunks surrounding the center chunk are always plains. 
                riverStartsChunks.Add(_chunk); // This chunks can also initiated a river.
            }

            else if (_chunk.chunkPosition != Vector3Int.zero)
            {
                float _forestRatio = Random.Range(0.0f, 1.0f);
                if (_forestRatio < forestRatio && _forestLimit <= forestLimit)
                {
                    _chunk.chunkType = Chunk.ChunkTypes.Forest;
                    _forestLimit++;
                }
                else
                {
                    _chunk.chunkType = Chunk.ChunkTypes.Plain;
                    _forestLimit--;
                }
            }
            assignedChunkList.Add(_chunk);
            unassignedChunkList.Remove(_chunk);
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ChunkAssets))]
    public class ChunkAssetsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ChunkAssets chunkAssets = (ChunkAssets)target;

            if (GUILayout.Button("Map Generator"))
            {
                chunkAssets.ChunkReset();
                chunkAssets.ChunkInitiation(chunkAssets.generatedRadius, chunkAssets.mapCenter);
                chunkAssets.chunkTypeAssignment();
            }
        }
    }
#endif
}

[System.Serializable]
public class Chunk
{
    public Vector3Int chunkPosition;
    public Vector3Int chunkCenter
    {
        get { return chunkPosition * MapManager.chunkSize; }
    }

    public bool isPrepared = false;
    public bool startChunk = false;

    public enum ChunkTypes
    {
        Plain,
        Forest,
        Water,
    }

    public ChunkTypes chunkType;
}
