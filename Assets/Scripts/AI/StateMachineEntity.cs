using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachineEntity : MonoBehaviour
{
    public NavMeshAgent agent;

    public StateMachine<StateMachineEntity> stateMachine;

    public float moveSpeed = 2f;

    public float viewRadius = 10f;

    public static List<GameObject> plantsInRange = new List<GameObject>(); //Static weil ich sonst nicht im SearchFoodState unter Update drauf zugreifen kann

    public class SearchWaterState : State<StateMachineEntity>
    {
        public override void Update()
        {
            float bestDistance = Mathf.Infinity;

        }
    }

    //Sollte funktionieren
    public class SearchFoodState : State<StateMachineEntity>
    {
        public override void Update()
        {
            float bestDistance = Mathf.Infinity;
            GameObject bestPlant = null; //bestPlant = closest plant
            foreach (var plant in plantsInRange)
            {
                float dist = Vector2.Distance(plant.transform.position, objectReference.transform.position);
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestPlant = plant;
                }
            }

            if (bestPlant != null)
            {
                //zur nähesten pflanze gehen
                objectReference.agent.SetDestination(bestPlant.transform.position);
            }
        }
    }

    public class EvadePlayerState : State<StateMachineEntity>
    {

    }

    public class IdleState : State<StateMachineEntity>
    {

    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //stateMachine.Update();

        //Physics.OverlapSphere(transform.position, 100);
        ViewDistanceCheck();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 100);
    }

    public void ViewDistanceCheck()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100);
        foreach (var hit in hitColliders)
        {
            Debug.Log(hit.gameObject.name + " " + hit.gameObject.transform.position);
            if (hit.gameObject.tag == "Plant")
            {
                AddObjectToListOnce(hit.gameObject, plantsInRange);
            }
        }
        Debug.Log(plantsInRange.Count);
    }

    public void AddObjectToListOnce(GameObject obj, List<GameObject> myList)
    {
        if (!myList.Contains(obj))
        {
            myList.Add(obj);
        }
    }

}
