using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class CleaningMist : MonoBehaviour
{
    [SerializeField]
    private int mistRange;
    [SerializeField]
    private int clearRange;
    [SerializeField, Range(0, 9)]
    private int blurSteps;

    private void Start()
    {
        ClearMist();
    }

    private int mistTotalRange
    {
        get { return mistRange + clearRange; }
    }

    private void ClearMist()
    {
        Vector3Int centralPosition = GameManager.Instance.terrainTilemap.WorldToCell(transform.position);
        for (int x = -mistTotalRange; x < mistTotalRange; x++)
        {
            for (int y = -mistTotalRange; y < mistTotalRange; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0) + centralPosition;
                float distToCenter = Vector3.Distance(centralPosition, tilePosition);
                Color mistIntensity = MistColor(distToCenter);
                Color tileColor = GameManager.Instance.terrainTilemap.GetColor(tilePosition);

                if (mistIntensity.r < tileColor.r)
                {
                    mistIntensity = tileColor;
                }

                if (GameManager.Instance.terrainTilemap.HasTile(tilePosition))
                {
                    GameManager.Instance.terrainTilemap.SetColor(tilePosition, mistIntensity);
                }

                if (GameManager.Instance.forestTilemap.HasTile(tilePosition))
                {
                    GameManager.Instance.forestTilemap.SetColor(tilePosition, mistIntensity);
                }
            }
        }
    }

    private Color MistColor(float dist)
    {
        Color mistcolor = Color.white;
        if (dist >= clearRange && dist < mistTotalRange)
        {
            // Distancia del tile al comienzo de la difuminación.
            float distBlur = dist - clearRange;
            // Ver que valor se le asigna (de  0 a 1)
            float stepNumber = Mathf.FloorToInt((distBlur * blurSteps) / mistRange);

            mistcolor.r = 1 - (stepNumber / blurSteps);
            mistcolor.g = mistcolor.b = Mathf.Pow(mistcolor.r, 2);
        }
        else if (dist >= mistTotalRange)
        {
            mistcolor = Color.black;
        }

        return mistcolor;
    }
}
