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

    private void Awake()
    {
        mapGen = FindObjectOfType<MapGenerator>();
    }

    private void Start()
    {
        r = GetComponent<Renderer>();
    }

    private void Update()
    {
        //SpawnTrees();
    }

    public void SpawnTrees(float x, float y)
    {
        //Debug.Log("XMin: " + r.bounds.min.x + " XMax: " + r.bounds.max.x);
        //Debug.Log("ZMin: " + r.bounds.min.z + " ZMax: " + r.bounds.max.z);

        //randomX = Random.Range(r.bounds.min.x, r.bounds.max.x);
        //randomZ = Random.Range(r.bounds.min.z, r.bounds.max.z);

        RaycastHit hit;
        //if (currentTrees <= numberOfTrees)
        //{
        Debug.DrawRay(new Vector3(x * 10f, 100f, y * 10f), Vector3.down * Mathf.Infinity, Color.red, 30f);
        if (Physics.Raycast(new Vector3(-(x * 10f - 320f), 100f, -(y * 10f - 320f)), Vector3.down, out hit, Mathf.Infinity)/* && Random.value <= 0.5f*/)
        {
            if (hit.transform.tag != "Plant" && hit.transform.tag != "Tree" && hit.transform.tag != "Animal" && hit.transform.tag != "Predator" && hit.transform.tag != "PredatorViewDistance")
            {
                int randomIndex = Random.Range(0, trees.Count);
                GameObject randomTree = trees[randomIndex];
                GameObject spawnedTree = Instantiate(randomTree, hit.point + new Vector3(0, 0, 0), Quaternion.identity); //y ist 0 oder 5f wars nur beim cube weil der size 10 hatte und der pivot genau in der mitte war
                spawnedTree.transform.parent = treeHolder.transform;
            }
            //else
            //{

            //    //Debug.Log("Hit Point: " + hit.point);
            //}
        }

        //}
    }

    public void SpawnPlants(float x, float y)
    {
        RaycastHit hit;
        Debug.DrawRay(new Vector3(x * 10f, 100f, y * 10f), Vector3.down * Mathf.Infinity, Color.red, 30f);
        if (Physics.Raycast(new Vector3(-(x * 10f - 320f), 100f, -(y * 10f - 320f)), Vector3.down, out hit, Mathf.Infinity)/* && Random.value <= 0.5f*/)
        {
            if (hit.transform.tag != "Plant" && hit.transform.tag != "Tree" && hit.transform.tag != "Animal" && hit.transform.tag != "Predator" && hit.transform.tag != "PredatorViewDistance")
            {
                int randomIndex = Random.Range(0, plants.Count);
                GameObject randomPlant = plants[randomIndex];
                GameObject spawnedPlant = Instantiate(randomPlant, hit.point + new Vector3(0, 0, 0), Quaternion.identity);
                spawnedPlant.transform.parent = plantHolder.transform;
            }
            //else
            //{

            //    //Debug.Log("Hit Point: " + hit.point);
            //}

        }
        else
        {
            Debug.Log("No Ray!");
            return;
        }

    }

    public void RespawnPlants(float x, float y)
    {
        RaycastHit hit;
        Debug.DrawRay(new Vector3(x * 10f, 100f, y * 10f), Vector3.down * Mathf.Infinity, Color.red, 30f);
        if (Physics.Raycast(new Vector3(-(x * 10f - 320f), 100f, -(y * 10f - 320f)), Vector3.down, out hit, Mathf.Infinity)/* && Random.value <= 0.5f*/)
        {
            if (hit.transform.tag != "Plant" && hit.transform.tag != "Tree" && hit.transform.tag != "Animal" && hit.transform.tag != "Predator" && hit.transform.tag != "PredatorViewDistance" && !MapGenerator.waterList.Contains(new Vector3(hit.point.x, 0, hit.point.y)))
            {
                Instantiate(plant, hit.point + new Vector3(0, 5f, 0), Quaternion.identity);
            }
            //else
            //{

            //    //Debug.Log("Hit Point: " + hit.point);
            //}

        }
        else
        {
            Debug.Log("No Ray!");
            return;
        }
    }


    //need to work on that
    public void SpawnAnimals(float x, float y)
    {
        RaycastHit hit;
        Debug.DrawRay(new Vector3(x * 10f, 100f, y * 10f), Vector3.down * Mathf.Infinity, Color.red, 30f);
        if (Physics.Raycast(new Vector3(-(x * 10f - 320f), 100f, -(y * 10f - 320f)), Vector3.down, out hit, Mathf.Infinity)/* && Random.value <= 0.5f*/)
        {
            if (hit.transform.tag != "Plant" && hit.transform.tag != "Tree" && hit.transform.tag != "Animal" && hit.transform.tag != "Predator" && hit.transform.tag != "PredatorViewDistance")
            {
                GameObject spawnedAnimal = Instantiate(animal, hit.point + new Vector3(0, 5f, 0), Quaternion.identity);
                spawnedAnimal.transform.parent = animalHolder.transform;
            }
            //else
            //{

            //    //Debug.Log("Hit Point: " + hit.point);
            //}

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
        if (Physics.Raycast(new Vector3(-(x * 10f - 320f), 100f, -(y * 10f - 320f)), Vector3.down, out hit, Mathf.Infinity)/* && Random.value <= 0.5f*/)
        {
            if (hit.transform.tag != "Plant" && hit.transform.tag != "Tree" && hit.transform.tag != "Animal" && hit.transform.tag != "Predator" && hit.transform.tag != "PredatorViewDistance")
            {
                GameObject spawnedPredator = Instantiate(predator, hit.point + new Vector3(0, 5f, 0), Quaternion.identity);
                spawnedPredator.transform.parent = predatorHolder.transform;
            }
            //else
            //{

            //    //Debug.Log("Hit Point: " + hit.point);
            //}

        }
        else
        {
            Debug.Log("No Ray!");
            return;
        }
    }
}
