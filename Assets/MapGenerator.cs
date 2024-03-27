using System.Net.NetworkInformation;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode{NoiseMap, ColorMap, FalloffMap};
    public DrawMode drawMode;

    public int size;
    public float noiseScale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    [Range(0, 1)]
    public float falloffPersistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public TerrainType[] regions;
    public LandType[] colors;

    int mapWidth;
    int mapHeight;

    System.Random prng;


    public void GenerateMap()
    {
        mapWidth = size;
        mapHeight = mapWidth;
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] falloffMap = FalloffMapGenerator.GenerateFalloffMap(mapWidth);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - (falloffMap[x, y] * falloffPersistance));
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;

                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffMapGenerator.GenerateFalloffMap(mapWidth)));
        }
    }
    private void OnValidate()
    {
        if(size < 1)
        {
            size = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

[System.Serializable]
public struct LandType
{
    public string name;
    public float height;
    public Color color;
}
