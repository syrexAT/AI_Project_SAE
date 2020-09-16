using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    public float energy; //uses energy when running after rabbits, if no energy left, no chasing will happen until a threshhold
    public float hunger; //needs to eat animals to not die

    public float timeToDeathByHunger;

    public float energyThreshhold; //when energy has reached this value the predator can chase again (if the chase takes a long time he may not eat the animal and has to wait on energy again)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
