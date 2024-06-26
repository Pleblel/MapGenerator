using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //Modes to draw map
    public enum DrawMode{NoiseMap, ColorMap, FalloffMap, GrassNoise};
    public DrawMode drawMode;

    //Noise settings
    public int size;
    public float noiseScale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    [Range(0, 1)]
    public float falloffPersistance;
    public float lacunarity;
    public int seed;
    public int grassSeed;
    public Vector2 offset;

    public bool autoUpdate;

    //Map settings
    public TerrainType[] regions;
    public LandType[] colors;

    int mapWidth;
    int mapHeight;

    public void GenerateMap(int randSeed, int grassRandSeed)
    {
        
        //Editor logic
        if(randSeed != 100001)
        {
            seed = randSeed;
        }
        if (grassRandSeed != 100001)
        {
            grassSeed = grassRandSeed;
        }

        //All types of map
        mapWidth = size;
        mapHeight = mapWidth;
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] grassNoiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, grassSeed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] falloffMap = FalloffMapGenerator.GenerateFalloffMap(mapWidth);

        //Creates color array and loops through pixels
        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                //Puts the noisemaps 2D array between 1 and 0 and makes it blend with the falloff map
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - (falloffMap[x, y] * falloffPersistance));
                grassNoiseMap[x, y] = Mathf.Clamp01(grassNoiseMap[x,y]);
                float currentHeight = noiseMap[x, y];
                float currentGrassHeight = grassNoiseMap[x, y];

                //Loops through all regions and checks what color it falls into
                for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {

                        //Does the same but if it is grass it uses another map to get grass color
                        if (regions[i].name == "Land")
                        {
                            for (int j = 0; j < colors.Length; j++)
                            {
                                if (currentGrassHeight <= colors[j].height)
                                {
                                    colorMap[y * mapWidth + x] = colors[j].color;
                                    break;
                                }
                            }
                        }

                        //Sets color to the coresponding color
                        else
                        {
                            colorMap[y * mapWidth+ x] = regions[i].color;
                           
                        }
                        break;
                    }
                }
            }
        }

        //All the different modes to draw the map
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
        else if (drawMode == DrawMode.GrassNoise)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(grassNoiseMap));
        }
    }

    //Limit ranges on some variabvles
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


//Structs with info on the regions names and colors
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
