using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
        Mesh
    }
    public DrawMode drawMode;

    [SerializeField]
    private int mapChunkSize = 241;
    public int Chunksize { get { return mapChunkSize; } }

    [Range(0, 6)]
    public int levelOfDetail; //increment value, 1 if no simplification, otherwise 2,4,8,12 for increasing levels of simplification

    [Header("MapValues")]
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public bool autoUpdate; //to auto update the height and width and scale automatically and not by pressing generate (EDITOR ONLY)

    [Header("Other")]
    public TerrainType[] regions;
    public Spawner spawner;
    public static List<Vector2> waterList = new List<Vector2>();
    public static MapGenerator instance;
    float animalAmount;
    float predatorAmount;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        //Getting the settings from MainMenu
        #region GetPlayerPrefs
        if (PlayerPrefExtension.GetBool("smallAnimalBool") != false)
        {
            animalAmount = 0.015f;
        }
        else if (PlayerPrefExtension.GetBool("mediumAnimalBool") != false)
        {
            animalAmount = 0.020f;
        }
        else if (PlayerPrefExtension.GetBool("largeAnimalBool") != false)
        {
            animalAmount = 0.025f;
        }
        Debug.Log("ANIMALAMOUNT: " + animalAmount);

        if (PlayerPrefExtension.GetBool("smallPredatorBool") != false)
        {
            predatorAmount = 0.002f;
        }
        else if (PlayerPrefExtension.GetBool("mediumPredatorBool") != false)
        {
            predatorAmount = 0.004f;
        }
        else if (PlayerPrefExtension.GetBool("largePredatorBool") != false)
        {
            predatorAmount = 0.006f;
        }
        Debug.Log("PREDATORAMOUNT: " + predatorAmount);


        #endregion

    }
    public void GenerateMap()
    {
        waterList.Clear();
        //fetching the 2D noise map from the noise class
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        Animal.reservedWaterTiles = new List<Vector3>();

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)//we found the region that it falls within
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color; //now all colors are saved in the array
                     
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    float height = regions[i].height;
                    if (currentHeight <= height)
                    {
                        if (height <= 0.4f && height > 0.3f)
                        {
                            //* 10 weil tiles 10 groß sind, -320 weil alles verschieben, +5 um in den tilecenter zu kommen
                            waterList.Add(new Vector2((x) * 10f - 320f + 5f, (mapChunkSize - y) * 10f - 320f - 5f)); //Adding all waterTiles to waterList
                        }

                        //Spawning stuff on Land with a chance
                        if (height >= 0.8f)
                        {

                            if (Random.value <= 0.025f)
                            {
                                spawner.SpawnTrees(mapChunkSize - x - 1, y + 1);
                            }
                            if (Random.value <= 0.03f)
                            {
                                spawner.SpawnPlants(mapChunkSize - x - 1, y + 1);
                            }

                            if (Random.value <= animalAmount)
                            {
                                spawner.SpawnAnimals(mapChunkSize - x - 1, y + 1);
                            }

                            if (Random.value <= predatorAmount)
                            {
                                spawner.SpawnPredators(mapChunkSize - x - 1, y + 1);
                            }
                        }
                        break;
                    }
                }
            }
        }

    }

    //Called when script variable is changed in inspector
    private void OnValidate()
    {
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

public class Biome
{
    public string name;
    public Color color;
}
