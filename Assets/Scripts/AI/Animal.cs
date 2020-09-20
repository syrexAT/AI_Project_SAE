using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    [Header("Thirst")]
    public float thirst;
    public float timeToDeathByThirst = 10;
    public float thirstReductionPerSecond;
    public bool moreThirsty = false;

    [Header("Hunger")]
    public float hunger;
    public float timeToDeathByHunger = 200;
    public float hungerReductionPerSecond;
    public bool moreHungry = false;

    [Header("Other")]
    public Image hungerBar;
    public Image thirstBar;
    public StateMachineEntity animalEntity;
    public GameObject currentlyBestPlant;
    public Vector3 currentlyBestWaterTile;
    public static List<Vector3> reservedWaterTiles;

    private void Start()
    {
        animalEntity = GetComponent<StateMachineEntity>();
    }

    private void Update()
    {
        hunger += Time.deltaTime * 1 / timeToDeathByHunger;
        thirst += Time.deltaTime * 1 / timeToDeathByThirst;

        hungerBar.fillAmount = hunger;
        thirstBar.fillAmount = thirst;
        //Destroy/Die gameobject when hunger/thirst is >= 1

        if (hunger >= thirst)
        {
            moreHungry = true;
            moreThirsty = false;
        }
        else
        {
            moreThirsty = true;
            moreHungry = false;
        }
        if (hunger >= 1 || thirst >= 1)
        {
            Destroy(gameObject);
        }
    }

    public GameObject FindClosestPlant()
    {
        float bestDistance = Mathf.Infinity;
        GameObject bestPlant = null; //bestPlant = closest plant
        if (animalEntity.plantsInRange != null || animalEntity.plantsInRange.Count != 0)
        {
            foreach (var plant in animalEntity.plantsInRange)
            {
                if (plant != null)
                {
                    float dist = Vector3.Distance(plant.transform.position, animalEntity.transform.position);
                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        bestPlant = plant;
                    }
                }
            }
        }

        return bestPlant;
    }

    public Vector3 FindClosestWaterTile()
    {
        float bestDistance = Mathf.Infinity; //glaub ich?
        Vector3 bestWater = Vector3.zero;
        foreach (var water in animalEntity.waterInRange)
        {
            //Debug.Log("IN SEARCHWATERSTATE");
            Vector3 waterTile = new Vector3(water.x, 0, water.y);
            float dist = Vector3.Distance(waterTile, animalEntity.transform.position);
            if (dist < bestDistance)
            {
                if (!reservedWaterTiles.Contains(water)) //to not choose a waterTile which is already reserved by another animal
                {
                    bestDistance = dist;
                    bestWater = water;
                }

            }
        }
        reservedWaterTiles.Add(bestWater);
        return bestWater;
    }
}
