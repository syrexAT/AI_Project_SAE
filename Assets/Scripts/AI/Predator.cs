﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Predator : MonoBehaviour
{
    [Header("Hunger")]
    public float hunger; //needs to eat animals to not die
    public float startHunger;
    public float hungerReductionByEating;
    public float timeToDeathByHunger;
    public float hungerThreshhold; //under a specific threshhold the predator wont search for prey, he is full!

    [Header("Energy")]
    public float energy; //uses energy when running after rabbits, if no energy left, no chasing will happen until a threshold
    public float startEnergy;
    public float energyReductionPerSecond;
    public float timeToFullEnergy;
    public float energyThreshhold; //when energy has reached this value the predator can chase again (if the chase takes a long time he may not eat the animal and has to wait on energy again)

    [Header("Other")]
    public Image energyBar;
    public Image hungerBar;
    public Image energyThresholdBar;
    public Image hungerThresholdBar;
    public PredatorEntity predatorEntity;
    public GameObject currentlyHuntedAnimal;

    // Start is called before the first frame update
    void Start()
    {
        predatorEntity = GetComponent<PredatorEntity>();
        energy = startEnergy;
        hunger = startHunger;
        energyThresholdBar.fillAmount = energyThreshhold;
        hungerThresholdBar.fillAmount = hungerThreshhold;

    }

    // Update is called once per frame
    void Update()
    {
        hunger += Time.deltaTime / timeToDeathByHunger;
        energy += Time.deltaTime / timeToFullEnergy;

        if (energy > 1)
        {
            energy = 1f;
        }

        if (hunger > 1)
        {
            hunger = 1f;
        }

        if (hunger >= 1)
        {
            Destroy(gameObject);
        }

        energyBar.fillAmount = energy;
        hungerBar.fillAmount = hunger;
    }

    public void EatPreyFunction(GameObject bestPrey)
    {
        Destroy(bestPrey);
        hunger -= hungerReductionByEating;
    }

    public GameObject FindClosestPrey()
    {
        float bestDistance = Mathf.Infinity;
        GameObject bestPrey = null;
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

    public void ReduceEnergy() //reduce energy while hunting
    {
        energy -= Time.deltaTime * energyReductionPerSecond;
    }


}
