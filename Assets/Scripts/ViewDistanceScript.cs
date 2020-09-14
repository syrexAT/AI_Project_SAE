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
