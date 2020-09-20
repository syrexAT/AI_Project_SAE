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

    private void Start()
    {
        InvokeRepeating("WaterTileCheck", 0, 1.0f);
    }

    private void Update()
    {

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

    public void WaterTileCheck()
    {
        foreach (var water in MapGenerator.waterList)
        {
            if (Vector3.Distance(new Vector3(water.x, 0, water.y), transform.position) <= stateMachineEntity.viewDistance)
            {
                AddVector2ToListOnce(water, stateMachineEntity.waterInRange);
            }
            else
            {
                if (stateMachineEntity.waterInRange.Contains(water))
                {
                    stateMachineEntity.waterInRange.Remove(water);
                }
            }
        }
    }
}
