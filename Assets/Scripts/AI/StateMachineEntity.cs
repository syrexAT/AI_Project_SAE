using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class StateMachineEntity : MonoBehaviour
{
    //Vielleicht in singelton umwandeln, und static vermeiden?
    public static StateMachineEntity _instance;


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
            //float bestDistance = Mathf.Infinity;

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
        public float wanderRadius = 50f; //not tested, probably needs to be set much lower 
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

    //When a predator is in range and the animal needs to run away
    //public class PredatorInRangeTransition : Transition<StateMachineEntity>
    //{
    //    public override bool GetIsAllowed()
    //    {

    //        //if (predatorInRange.)
    //        //{

    //        //}
    //    }
    //}

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine<StateMachineEntity>();

        //StateMachine
        stateMachine.AddState(new SearchFoodState() { objectReference = this }, "SearchFood");
        stateMachine.AddState(new EvadePredatorState() { objectReference = this }, "EvadePredator");
        stateMachine.AddState(new WanderAroundState() { objectReference = this }, "WanderAround");
        stateMachine.AddState(new IdleState() { objectReference = this }, "Idle");

        stateMachine.SetInitialState("Idle");

        //Transitions (stateMachine.AddTransition)
        //stateMachine.AddTransition(new)

    }

    private void Update()
    {
        stateMachine.Update();
        ViewDistanceCheck();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 100);
    }


    //IT WILL NOT DETECT STUFF THAT LEFT THE OVERLAPSPHERE
    //Either way, https://answers.unity.com/questions/1180873/can-you-use-oncollisionexit-with-an-overlapsphere.html
    //or just replace the overlapshere in general with a spherical collider in the same size as the overlapsphere,
    //so I can use OnCollisionEnter/Exit methods, will probably be the simplier solution
    //OR
    //keep a current collection of in-range objects, and a previous collection of inrange objects
    //then compare the objects in current collection to those in previous collection
    //objects that are in previous collection and not in current collection have moved out of range
    public void ViewDistanceCheck()
    {
        //List<Collider> currentCollection = new List<Collider>();
        //List<Collider> previousCollection = new List<Collider>();

        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100);
        //foreach (var hit in hitColliders)
        //{
        //    currentCollection.Add(hit);
        //    Debug.Log(hit.gameObject.name + " " + hit.gameObject.transform.position);
        //    if (hit.gameObject.tag == "Plant")
        //    {
        //        AddObjectToListOnce(hit.gameObject, plantsInRange);
        //        //if (hitColliders.)
        //        //{

        //        //}
        //    }
        //    if (hit.gameObject.tag == "Predator")
        //    {
        //        AddObjectToListOnce(hit.gameObject, predatorInRange);
        //    }

        //    //one more for water but not with colliders obviously, but with detecting noise map heights under 0.4f
        //}
        Debug.Log("Plants: " + plantsInRange.Count);
        Debug.Log("Predator " + predatorInRange.Count);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Plant")
        {
            plantsInRange.Add(other.gameObject);
        }
        if (other.gameObject.tag == "Predator")
        {
            predatorInRange.Add(other.gameObject);
        }
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
