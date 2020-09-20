using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public Renderer r;
    public GameObject tree;
    public GameObject plant;
    public GameObject animal;
    public GameObject predator;

    public Transform treeHolder;
    public Transform plantHolder;
    public Transform animalHolder;
    public Transform predatorHolder;

    public List<GameObject> plants = new List<GameObject>();
    public List<GameObject> trees = new List<GameObject>();

    public MapGenerator mapGen;
    LayerMask mask;

    private void Awake()
    {
        mapGen = FindObjectOfType<MapGenerator>();
        mask = LayerMask.GetMask("Mesh");
    }

    private void Start()
    {
        r = GetComponent<Renderer>();
    }

    public void SpawnTrees(float x, float y)
    {
        RaycastHit hit;
        Debug.DrawRay(new Vector3(x * 10f, 100f, y * 10f), Vector3.down * Mathf.Infinity, Color.red, 30f);
        if (Physics.Raycast(new Vector3(-(x * 10f - 320f), 100f, -(y * 10f - 320f)), Vector3.down, out hit, 101f, mask)) //RayCast nach unten schießen
        {
            if (hit.transform.tag == "World")
            {
                int randomIndex = Random.Range(0, trees.Count);
                GameObject randomTree = trees[randomIndex];
                GameObject spawnedTree = Instantiate(randomTree, hit.point, Quaternion.identity);
                spawnedTree.transform.parent = treeHolder.transform;
            }
        }
    }

    public void SpawnPlants(float x, float y)
    {
        RaycastHit hit;
        Debug.DrawRay(new Vector3(x * 10f, 100f, y * 10f), Vector3.down * Mathf.Infinity, Color.red, 30f);
        if (Physics.Raycast(new Vector3(-(x * 10f - 320f), 100f, -(y * 10f - 320f)), Vector3.down, out hit, 101f, mask))
        {
            if (hit.transform.tag == "World")
            {
                int randomIndex = Random.Range(0, plants.Count);
                GameObject randomPlant = plants[randomIndex];
                GameObject spawnedPlant = Instantiate(randomPlant, hit.point, Quaternion.identity);
                spawnedPlant.transform.parent = plantHolder.transform;
            }
        }
        else
        {
            Debug.Log("No Ray!");
            return;
        }

    }

    //NOT IN USE
    public void RespawnPlants(float x, float y)
    {
        RaycastHit hit;
        Debug.DrawRay(new Vector3(x * 10f, 100f, y * 10f), Vector3.down * Mathf.Infinity, Color.red, 30f);
        if (Physics.Raycast(new Vector3(-(x * 10f - 320f), 100f, -(y * 10f - 320f)), Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag != "Plant" && hit.transform.tag != "Tree" && hit.transform.tag != "Animal" && hit.transform.tag != "Predator" && hit.transform.tag != "PredatorViewDistance" && !MapGenerator.waterList.Contains(new Vector3(hit.point.x, 0, hit.point.y)))
            {
                Instantiate(plant, hit.point + new Vector3(0, 5f, 0), Quaternion.identity);
            }
        }
        else
        {
            Debug.Log("No Ray!");
            return;
        }
    }


    public void SpawnAnimals(float x, float y)
    {
        RaycastHit hit;
        Debug.DrawRay(new Vector3(x * 10f, 100f, y * 10f), Vector3.down * Mathf.Infinity, Color.red, 30f);
        if (Physics.Raycast(new Vector3(-(x * 10f - 320f), 100f, -(y * 10f - 320f)), Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "World")
            {
                GameObject spawnedAnimal = Instantiate(animal, hit.point + new Vector3(0, 5f, 0), Quaternion.identity);
                spawnedAnimal.transform.parent = animalHolder.transform;
            }
        }
        else
        {
            Debug.Log("No Ray!");
            return;
        }
    }

    public void SpawnPredators(float x, float y)
    {
        RaycastHit hit;
        Debug.DrawRay(new Vector3(x * 10f, 100f, y * 10f), Vector3.down * Mathf.Infinity, Color.red, 30f);
        if (Physics.Raycast(new Vector3(-(x * 10f - 320f), 100f, -(y * 10f - 320f)), Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "World")
            {
                GameObject spawnedPredator = Instantiate(predator, hit.point + new Vector3(0, 5f, 0), Quaternion.identity);
                spawnedPredator.transform.parent = predatorHolder.transform;
            }
        }
        else
        {
            Debug.Log("No Ray!");
            return;
        }
    }
}
