using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    public float thirst;
    public float hunger;
    public float timeToDeathByThirst = 10;
    public float timeToDeathByHunger = 200;

    public float thirstReductionPerSecond;
    public float hungerReductionPerSecond;

    public bool moreHungry = false;
    public bool moreThirsty = false;

    public float drinkDuration = 6f;
    public float eatDuraton = 8f;

    public float criticalPercent = 0.7f; //crticialpercent where animal will head to water/plant regardless of other stuff?

    public int viewDistance; //Distance the animal can see, if neither water or a plant is in viewDistance the animal will wander around
    public float timeBetweenActionsChoices;

    public float moveSpeed;

    public bool isDrinking;
    public bool isEating;

    public Image hungerBar;
    public Image thirstBar;

    public StateMachineEntity animalEntity;

    public GameObject currentlyBestPlant;

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
    }

    public GameObject FindClosestPlant()
    {
        float bestDistance = Mathf.Infinity;
        GameObject bestPlant = null; //bestPlant = closest plant
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

        return bestPlant;
    }
}
