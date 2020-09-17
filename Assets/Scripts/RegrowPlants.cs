using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class RegrowPlants : MonoBehaviour
{
    public List<GameObject> currentPlantsList = new List<GameObject>();
    public int initialPlants;
    public int currentPlants;

    public bool gotPlants = false;

    public Spawner spawner;

    public MapGenerator mapGen;
    public int chunkSize;

    public TerrainType[] regions;

    // Start is called before the first frame update
    void Start()
    {
        spawner = GetComponent<Spawner>();
        chunkSize = mapGen.Chunksize;
        regions = mapGen.regions;
    }

    // Update is called once per frame
    void Update()
    {
        if (gotPlants == false)
        {
            initialPlants = GameObject.FindGameObjectsWithTag("Plant").Length;
            gotPlants = true;
        }

        currentPlants = GameObject.FindGameObjectsWithTag("Plant").Length;

        //it obviously doesnt detect water or not because I dont give him noise values, need to fix that / figure out how to do it

        if (currentPlants <= 20)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    for (int i = 0; i < regions.Length; i++)
                    {

                        if (regions[i].height <= 0.8f)
                        {
                            if (Random.value <= 0.03f)
                            {
                                spawner.SpawnPlants(chunkSize - x - 1, y + 1);
                            }

                        }


                    }
                    //float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, mapGen.seed, mapGen.noiseScale, mapGen.octaves, mapGen.persistance, mapGen.lacunarity, mapGen.offset);


                }
            }
        }

        print(initialPlants);
        print(currentPlants);

    }
}
