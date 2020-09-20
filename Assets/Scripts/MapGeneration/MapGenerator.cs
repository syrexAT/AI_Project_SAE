using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        //either noisemap or colormap
        NoiseMap,
        ColorMap,
        Mesh
    }
    public DrawMode drawMode;

    [SerializeField]
    private int mapChunkSize = 241; // Sebastian Lague E:06 LOD
    public int Chunksize { get { return mapChunkSize; } }

    [Range(0, 6)]
    public int levelOfDetail; //increment value, 1 if no simplification, otherwise 2,4,8,12 for increasing levels of simplification

    //values that define the map
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate; //to auto update the height and width and scale automatically and not by pressing generate

    public TerrainType[] regions;

    public GameObject cube;

    public GameObject plant;

    public Spawner spawner;

    public int numberOfTrees;

    public GameObject tree;

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

        #region GetPlayerPrefs
        if (PlayerPrefExtension.GetBool("smallAnimalBool") != false)
        {
            animalAmount = 0.05f;
        }
        else if (PlayerPrefExtension.GetBool("mediumAnimalBool") != false)
        {
            animalAmount = 0.010f;
        }
        else if (PlayerPrefExtension.GetBool("largeAnimalBool") != false)
        {
            animalAmount = 0.015f;
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
        //fetching the 2D noise map from the noise class; later on much more stuff to process the noise map to turn it into terrain map
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

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

                        if (regions[i].height <= 0.4f)
                        {
                            //* 10 weil tiles 10 groß sind, -320 weil alles verschieben, +5 um in den tilecenter zu kommen
                            waterList.Add(new Vector2((x) * 10f - 320f + 5f, (mapChunkSize - y) * 10f - 320f - 5f)); //Adding all waterTiles to waterList
                        }

                        if (regions[i].height >= 0.8f)
                        {

                            if (Random.value <= 0.025f)
                            {
                                spawner.SpawnTrees(mapChunkSize - x - 1, y + 1);
                                //Debug.Log(x + "    " + y);
                                //if (Random.Range(0,101) < 20)
                                //{
                                //Debug.Log(x + "    " + y);

                                //}
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

    }

    public Vector3 GetAbsolutePosition(int x, int y)
    {
        return new Vector3((x) * 10f - 320f + 5f, 0f, (mapChunkSize - y) * 10f - 320f - 5f);
    }

    //private void OnDrawGizmos()
    //{
    //    foreach (var water in waterList)
    //    {
    //        Gizmos.DrawSphere(new Vector3(water.x, 0, water.y), 1f);
    //        Debug.Log("DrawWater");

    //    }
    //}

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
