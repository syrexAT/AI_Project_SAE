using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorViewDistance : MonoBehaviour
{
    public PredatorEntity predator;

    private void Awake()
    {
        predator = GetComponent<PredatorEntity>();
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
        predator.animalsInRange.Add(other.gameObject);
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
