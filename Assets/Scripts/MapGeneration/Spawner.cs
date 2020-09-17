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

    float randomX;
    float randomZ;

    public int numberOfTrees;
    public int currentTrees;

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
                Instantiate(tree, hit.point + new Vector3(0, 5f, 0), Quaternion.identity);
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

    }

    public void SpawnPredators(float x, float y)
    {

    }
}
