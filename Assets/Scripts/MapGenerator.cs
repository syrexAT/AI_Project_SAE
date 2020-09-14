using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Range (0,6)]
    public int levelOfDetail; //increment value, 1 if no simplification, otherwise 2,4,8,12 for increasing levels of simplification

    //values that define the map
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
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

    public BoundsTest boundsTest;

    float randomX;
    float randomZ;
    public int numberOfTrees;
    int currentTrees;

    public GameObject tree;

    public void GenerateMap()
    {
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

                    if (regions[i].height >= 0.8f)
                    {
                        if (Random.value <= 0.025f)
                        {
                            //Debug.Log(x + "    " + y);
                            //if (Random.Range(0,101) < 20)
                            //{
                            Debug.Log(x + "    " + y);
                            boundsTest.SpawnTrees(x, y);
                            //}
                        }
                        if (Random.value <= 0.03f)
                        {
                            boundsTest.SpawnPlants(x, y);
                        }
                    }

                    if (currentHeight <= regions[i].height)//we found the region that it falls within
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color; //now all colors are saved in the array

                        //Spawning Plants on grass only
                        //TODO: Fix the alignment, blocks should be set left and top half the amount, see ingame
                        //FIXED: line above, but only for 64 chunksize

                        //boundsTest.SpawnTrees(regions[i].height);

                        //if (regions[i].height >= 0.8f)
                        //{
                        //    if (Random.value <= 0.5f)
                        //    {
                        //        if (currentTrees <= numberOfTrees)
                        //        {
                        //            RaycastHit hit;
                        //            if (Physics.Raycast(new Vector3(x, y), -Vector3.up, out hit))
                        //            {
                        //                Instantiate(tree, hit.point + new Vector3(0, 5f, 0), Quaternion.identity);
                        //                currentTrees += 1;
                        //            }
                        //        }


                        //    }
                        //}




                        //if (regions[i].height >= 0.8f)
                        //{
                        //    if (Random.value <= 0.025f)
                        //    {
                        //        Instantiate(plant, new Vector3(-(x * 10 - 315), 2, -(y * 10 - 315)), Quaternion.identity);
                        //    }
                        //}



                        //if (regions[i].height >= 0.8f)
                        //{

                        //}

                        break;
                    }

                    //float chunkX = Random.Range((float)x / (float)mapChunkSize, (float)(x + 1) / (float)mapChunkSize);
                    //float chunkY = Random.Range((float)y / (float)mapChunkSize, (float)(y + 1) / (float)mapChunkSize);



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
