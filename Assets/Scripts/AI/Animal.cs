using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public float thirst;
    public float hunger;
    public float timeToDeathByThirst;
    public float timeToDeathByHunger;

    public int viewDistance; //Distance the animal can see, if neither water or a plant is in viewDistance the animal will wander around
    public float timeBetweenActionsChoices;

    public float moveSpeed;
}
