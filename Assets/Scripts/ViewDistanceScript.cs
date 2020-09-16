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

    private void Update()
    {
        //WORKING, Detects water tile when in range and removes it when out of range
        //FUNKTIONIERT FAST, er detect am rand der map die water tiles nicht????ß whyyyyyyyyyyyyyyyyyyyyyyyyyyyy
        // musste vector3 sein weil sonst hat er noch x und höhe y !
        foreach (var water in MapGenerator.waterList)
        {
            if (Vector3.Distance(new Vector3 (water.x,0,water.y), transform.position) <= stateMachineEntity.viewDistance)
            {
                AddVector2ToListOnce(water, stateMachineEntity.waterInRange);
            }
            if (Vector3.Distance(new Vector3(water.x, 0, water.y), transform.position) >= stateMachineEntity.viewDistance)
            {
                if (stateMachineEntity.waterInRange.Contains(water))
                {
                    stateMachineEntity.waterInRange.Remove(water);
                }
            }
            //if (Vector3.Distance(new Vector2(water.x, water.y), transform.position) >= stateMachineEntity.viewDistance)
            //{
            //    stateMachineEntity.waterInRange.Remove(water);
            //}
        }
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

    public void AddVector2ToListOnce(Vector2 vector, List<Vector2> myList)
    {
        if (!myList.Contains(vector))
        {
            myList.Add(vector);
        }
    }

    //public static GameObject FindClosestPlant()
    //{

    //}

    //public static Vector3 FindClosesetWaterTile()
    //{

    //}

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
