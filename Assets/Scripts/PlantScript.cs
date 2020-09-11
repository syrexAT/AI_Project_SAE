﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantScript : MonoBehaviour
{
    float amountRemaining = 1;
    const float consumeSpeed = 8;

    public float Consume(float amount)
    {
        float amountConsumed = Mathf.Max(0, Mathf.Min(amountRemaining, amount)); //amount is the amount the animal needs to eat before reaching 0 on hunger level
        amountRemaining -= amount * consumeSpeed;

        transform.localScale = Vector3.one * amountRemaining;

        if (amountRemaining <= 0)
        {
            Destroy(gameObject);
        }

        return amountConsumed;
    }

    //public float AmountRemaining
    //{
    //    get
    //    {
    //        return amountRemaining;
    //    }
    //}
}
