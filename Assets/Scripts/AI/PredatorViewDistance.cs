using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorViewDistance : MonoBehaviour
{
    public PredatorEntity predator;

    private void Awake()
    {
        predator = GetComponentInParent<PredatorEntity>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Animal" && other.gameObject.name != "ViewDistance")
        {
            predator.animalsInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Animal" && other.gameObject.name != "ViewDistance")
        {
            if (other != null)
            {
                predator.animalsInRange.Remove(other.gameObject);
            }

        }

    }
}
