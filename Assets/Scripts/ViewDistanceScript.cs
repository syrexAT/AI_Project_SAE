using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDistanceScript : MonoBehaviour
{
    public StateMachineEntity stateMachineEntity;

    private void Awake()
    {
        stateMachineEntity = GetComponentInParent<StateMachineEntity>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Predator")
        {
            stateMachineEntity.predatorInRange.Add(other.gameObject);
        }
        if (other.gameObject.tag == "Plant")
        {
            stateMachineEntity.plantsInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Predator")
        {
            stateMachineEntity.predatorInRange.Remove(other.gameObject);
        }
        if (other.gameObject.tag == "Plant")
        {
            stateMachineEntity.plantsInRange.Remove(other.gameObject);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Plant")
    //    {
    //        stateMachineEntity.
    //    }
    //    if (other.gameObject.tag == "Tree")
    //    {

    //    }
    //}
}
