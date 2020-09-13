using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachineEntity : MonoBehaviour
{
    //Vielleicht in singelton umwandeln, und static vermeiden?
    private static StateMachineEntity instance = null;

    public NavMeshAgent agent;

    public StateMachine<StateMachineEntity> stateMachine;

    public float moveSpeed = 2f;

    public static float viewRadius = 10f;

    public static List<GameObject> plantsInRange = new List<GameObject>(); //Static weil ich sonst nicht im SearchFoodState unter Update drauf zugreifen kann
    public static List<GameObject> predatorInRange = new List<GameObject>();



    //Need to detect water tiles, they have a noise float below 0.4 (see inspector -> MapGenerator)
    //Get an array/list? of all water tiles in view Distance, go to nearest one
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
                float dist = Vector3.Distance(plant.transform.position, objectReference.transform.position);
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

    //Vielleicht auch ander lösbar https://answers.unity.com/questions/868003/navmesh-flee-ai-flee-from-player.html
    //Muss so gemacht werden das predator aufjedenfall das tier einholt vom moveSpeed her
    public class EvadePredatorState : State<StateMachineEntity>
    {
        //Find closest predator in range, run away from him

        public override void Update()
        {
            float bestDistance = Mathf.Infinity;
            GameObject bestPredator = null; //to prioritze the predator who is the nearest
            foreach (var predator in predatorInRange) //to find out which predator is the nearest
            {
                float dist = Vector3.Distance(objectReference.transform.position, predator.transform.position);
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestPredator = predator;
                }
            }

            if (bestPredator != null) //wenn er den nähesten gefunden hat
            {
                float distance = Vector3.Distance(objectReference.transform.position, bestPredator.transform.position); //distance between predator and animal
                Debug.Log("Distance: " + distance);

                if (distance < viewRadius * 2) //*2 weil radius 100? von mitte aus geht in eine richtung 50 und in die andere nochmal 50?
                {
                    Vector3 dirToPredator = objectReference.transform.position - bestPredator.transform.position;

                    Vector3 newPos = objectReference.transform.position + dirToPredator;

                    objectReference.agent.SetDestination(newPos);
                }

            }
        }
    }

    public class IdleState : State<StateMachineEntity> //Animal/rabbit should wander around searching for water/plant, as it can only detect water/plant in its view Radius
    {

    }

    public class WanderAroundState : State<StateMachineEntity>
    {
        public float wanderRadius = 50f; //not tested
        public float wanderTimer = 1f; //not tested

        private Transform target;
        private float timer;

        public override void Entered()
        {
            timer = wanderTimer;
        }

        public override void Update()
        {
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(objectReference.transform.position, wanderRadius, -1);
                objectReference.agent.SetDestination(newPos);
                timer = 0;
            }
        }
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
            if (hit.gameObject.tag == "Predator")
            {
                AddObjectToListOnce(hit.gameObject, predatorInRange);
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

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

}
