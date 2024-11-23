using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    public Terrain Terrain;
    public float HeightMultiplier = 800f;
    public int NumberOfMountains = 10;
    public float MountainRadius = 50f;

    public void Start()
    {
        var res = Terrain.terrainData.heightmapResolution;
        var heightMap = new float[res, res];

        for (int i = 0; i < NumberOfMountains; i++)
        {
            int centerX = Random.Range(0, res);
            int centerY = Random.Range(0, res);
            float mountainHeight = Random.Range(0.5f, 1f) * HeightMultiplier;

            for (int x = 0; x < res; x++)
            {
                for (int y = 0; y < res; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                    float height = mountainHeight * Mathf.Exp(-Mathf.Pow(distance / MountainRadius, 2));
                    heightMap[x, y] += height / Terrain.terrainData.size.y;
                }
            }
        }

        Terrain.terrainData.SetHeights(0, 0, heightMap);
    }
}