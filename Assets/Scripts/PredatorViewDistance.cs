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
        if (other.gameObject.tag == "Animal")
        {
            predator.animalsInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Animal")
        {
            if (other != null)
            {
                predator.animalsInRange.Remove(other.gameObject);
            }

        }

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
