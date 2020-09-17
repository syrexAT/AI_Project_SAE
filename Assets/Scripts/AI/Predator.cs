using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Predator : MonoBehaviour
{
    //Maybe give predators different amount of energy/hunger so not every predator dies if the ratio between energy and hunger isn't totally right
    public float energy; //uses energy when running after rabbits, if no energy left, no chasing will happen until a threshhold
    public float hunger; //needs to eat animals to not die

    public float startEnergy;
    public float startHunger; //not in use yet

    public float hungerReductionByEating;

    public float timeToDeathByHunger;
    public float timeToFullEnergy;
    public float energyReductionPerSecond;

    public float energyThreshhold; //when energy has reached this value the predator can chase again (if the chase takes a long time he may not eat the animal and has to wait on energy again)
    public float hungerThreshhold; //under a specific threshhold the predator wont search for prey, he is full!

    public Image energyBar;
    public Image hungerBar;

    public PredatorEntity predatorEntity;

    public GameObject currentlyHuntedAnimal;

    // Start is called before the first frame update
    void Start()
    {
        predatorEntity = GetComponent<PredatorEntity>();
        energy = startEnergy;
        hunger = startHunger;
    }

    // Update is called once per frame
    void Update()
    {
        hunger += Time.deltaTime / timeToDeathByHunger;
        energy += Time.deltaTime / timeToFullEnergy;

        if (energy > 1) //or Mathf.Clamp?
        {
            energy = 1f;
        }

        if (hunger > 1)
        {
            hunger = 1f;
        }

        energyBar.fillAmount = energy;
        hungerBar.fillAmount = hunger;
    }

    public void EatPreyFunction(GameObject bestPrey)
    {
        //if (Vector3.Distance(transform.position, bestPrey.transform.position) < 0.5f)
        //{
            Destroy(bestPrey);
            hunger -= hungerReductionByEating;
        //}
    }

    public GameObject FindClosestPrey()
    {
        float bestDistance = Mathf.Infinity;
        GameObject bestPrey = null; //bestPlant = closest plant
        foreach (var prey in predatorEntity.animalsInRange)
        {
            if (prey != null)
            {
                float dist = Vector3.Distance(prey.transform.position, predatorEntity.transform.position);
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestPrey = prey;
                }
            }
        }

        return bestPrey;
    }

    public void ReduceEnergy()
    {
        energy -= Time.deltaTime * energyReductionPerSecond;
    }


}
