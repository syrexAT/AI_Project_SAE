using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

//STATE MACHINE FOR ANIMAL
public class StateMachineEntity : MonoBehaviour
{
    public NavMeshAgent agent;

    public StateMachine<StateMachineEntity> stateMachine;

    public ViewDistanceScript viewDistanceScript;
    public SphereCollider viewDistanceCollider;
    public float viewDistance;

    public List<GameObject> plantsInRange = new List<GameObject>();
    public List<GameObject> predatorInRange = new List<GameObject>();
    public List<Vector2> waterInRange = new List<Vector2>();

    public Animal animal;

    public TextMeshProUGUI stateText;

    #region States
    public class SearchWaterState : State<StateMachineEntity>
    {
        public override void Entered()
        {
            objectReference.animal.currentlyBestWaterTile = objectReference.animal.FindClosestWaterTile();

            if (objectReference.animal.currentlyBestWaterTile != Vector3.zero)
            {
                objectReference.agent.SetDestination(new Vector3(objectReference.animal.currentlyBestWaterTile.x, 0, objectReference.animal.currentlyBestWaterTile.y));
            }
        }

        public override void Update()
        {

        }
    }

    public class DrinkWaterState : State<StateMachineEntity>
    {
        public override void Update()
        {
            objectReference.animal.thirst -= Time.deltaTime * objectReference.animal.thirstReductionPerSecond;
            objectReference.animal.thirst = Mathf.Clamp01(objectReference.animal.thirst);
        }

    }

    public class SearchFoodState : State<StateMachineEntity>
    {
        public override void Entered()
        {
            objectReference.animal.currentlyBestPlant = objectReference.animal.FindClosestPlant();

            if (objectReference.animal.currentlyBestPlant != null)
            {
                objectReference.agent.SetDestination(objectReference.animal.currentlyBestPlant.transform.position);
            }
        }

        public override void Update()
        {

        }
    }
    
    public class EatFoodState : State<StateMachineEntity>
    {

        public override void Update()
        {

            PlantScript foodTarget;
            if (objectReference.animal.currentlyBestPlant != null)
            {
                foodTarget = objectReference.animal.currentlyBestPlant.GetComponent<PlantScript>();
                float eatAmount = Time.deltaTime * 1 / objectReference.animal.eatDuraton;
                eatAmount = foodTarget.Consume(eatAmount);
                objectReference.animal.hunger -= eatAmount;
            }
            
        }

    }

    public class EvadePredatorState : State<StateMachineEntity>
    {
        //Find closest predator in range, run away from him

        public override void Update()
        {
            float bestDistance = Mathf.Infinity;
            GameObject bestPredator = null; //to prioritze the predator who is the nearest
            foreach (var predator in objectReference.predatorInRange) //to find out which predator is the nearest
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
                float distance = Vector3.Distance(objectReference.transform.position, bestPredator.transform.position);

                if (distance < objectReference.viewDistance)
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
                Vector3 newPos = Helper.RandomNavSphere(objectReference.transform.position, wanderRadius, -1);
                objectReference.agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }
    #endregion

    #region Transitions
    //When a predator is in range and the animal needs to run away
    public class PredatorInRangeTransition : Transition<StateMachineEntity> //hier soll er ins EvadePredator gehen
    {
        public override bool GetIsAllowed()
        {
            float bestDistance = Mathf.Infinity;
            GameObject bestPredator = null; //to prioritze the predator who is the nearest
            foreach (var predator in objectReference.predatorInRange) //to find out which predator is the nearest
            {
                if (predator != null)
                {
                    float dist = Vector3.Distance(objectReference.transform.position, predator.transform.position);
                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        bestPredator = predator;
                    }
                }
            }

            if (bestPredator != null)
            {
                float distance = Vector3.Distance(objectReference.transform.position, bestPredator.transform.position);
                return distance < objectReference.viewDistance; //so passts!
            }

            return false;
        }
    }

    public class PredatorOutOfRangeTransition : PredatorInRangeTransition //hier soll er ins WanderAround gehen
    {
        public override bool GetIsAllowed()
        {
            return !base.GetIsAllowed();
        }
    }

    public class FoodInRangeTransition : Transition<StateMachineEntity> //fehlt hier noch ein return für true? oderso? //hier soll er in SearchFood gehen
    {
        public override bool GetIsAllowed()
        {
            if (objectReference.animal.FindClosestPlant() != null && objectReference.animal.moreHungry)
            {
                return true;
            }


            return false;
        }
    }

    public class AtFoodSourceTransition : Transition<StateMachineEntity>
    {
        public override bool GetIsAllowed()
        {
            //if (!objectReference.agent.pathPending)
            //{
            //    if (objectReference.agent.remainingDistance <= objectReference.agent.stoppingDistance)
            //    {
            //        //if (!objectReference.agent.hasPath || objectReference.agent.velocity.sqrMagnitude == 0f)
            //        //{
            //            return true;
            //        //}
            //    }
            //}

            if (Vector3.Distance(objectReference.transform.position, objectReference.animal.currentlyBestPlant.transform.position) <= 8f && objectReference.animal.currentlyBestPlant != null && objectReference != null)
            {
                objectReference.agent.isStopped = true;
                objectReference.agent.velocity = Vector3.zero;
                return true;
            }

            return false;
        }
    }

    public class AtWaterSourceTransition : Transition<StateMachineEntity>
    {
        public override bool GetIsAllowed()
        {
            if (!objectReference.agent.pathPending)
            {
                if (objectReference.agent.remainingDistance <= objectReference.agent.stoppingDistance)
                {
                    return true;
                }
            }

            //if (Vector3.Distance(objectReference.transform.position, objectReference.animal.currentlyBestWaterTile) <= 30f)
            //{
            //    objectReference.agent.isStopped = true;
            //    objectReference.agent.velocity = Vector3.zero;
            //    return true;
            //}

            return false;
        }
    }



    public class FinishedEatingTransition : Transition<StateMachineEntity>
    {
        public override bool GetIsAllowed()
        {
            if (objectReference.animal.currentlyBestPlant == null || objectReference.animal.hunger <= 0.01f)
            {
                objectReference.agent.isStopped = false;
                return true;
            }

            return false;


        }
    }

    public class WaterInRangeTransition : Transition<StateMachineEntity>
    {
        public override bool GetIsAllowed()
        {
            if (objectReference.animal.FindClosestWaterTile() != Vector3.zero && objectReference.animal.moreThirsty)
            {
                return true;
            }

            return false;
        }
    }


    public class FinishedDrinkingTransition : Transition<StateMachineEntity>
    {
        public override bool GetIsAllowed()
        {
            objectReference.agent.isStopped = false;
            return objectReference.animal.thirst <= 0.01f;
        }
    }

    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animal = GetComponent<Animal>();
        viewDistanceCollider = GetComponentInChildren<SphereCollider>();
        viewDistance = viewDistanceCollider.radius * 10f;
        print(viewDistance);
        print(viewDistanceCollider.radius);
        stateMachine = new StateMachine<StateMachineEntity>();

        #region States

        //StateMachine
        stateMachine.AddState(new SearchFoodState() { objectReference = this }, "SearchFood");
        stateMachine.AddState(new EatFoodState() { objectReference = this }, "EatFood");
        stateMachine.AddState(new SearchWaterState() { objectReference = this }, "SearchWater");
        stateMachine.AddState(new DrinkWaterState() { objectReference = this }, "DrinkWater");
        stateMachine.AddState(new EvadePredatorState() { objectReference = this }, "EvadePredator");
        stateMachine.AddState(new WanderAroundState() { objectReference = this }, "WanderAround");
        stateMachine.AddState(new IdleState() { objectReference = this }, "Idle");

        stateMachine.SetInitialState("WanderAround");
        #endregion

        #region Transitions
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "SearchFood", "EvadePredator");
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "SearchWater", "EvadePredator");
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "WanderAround", "EvadePredator");
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "Idle", "EvadePredator"); //may be removed
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "EatFood", "EvadePredator");
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "DrinkWater", "EvadePredator");

        stateMachine.AddTransition(new PredatorOutOfRangeTransition() { objectReference = this }, "EvadePredator", "WanderAround");

        stateMachine.AddTransition(new FoodInRangeTransition() { objectReference = this }, "WanderAround", "SearchFood");
        stateMachine.AddTransition(new FinishedEatingTransition() { objectReference = this }, "EatFood", "WanderAround");

        stateMachine.AddTransition(new WaterInRangeTransition() { objectReference = this }, "WanderAround", "SearchWater");
        stateMachine.AddTransition(new FinishedDrinkingTransition() { objectReference = this }, "DrinkWater", "WanderAround");

        stateMachine.AddTransition(new AtFoodSourceTransition() { objectReference = this }, "SearchFood", "EatFood");
        stateMachine.AddTransition(new AtWaterSourceTransition() { objectReference = this }, "SearchWater", "DrinkWater");
        #endregion
    }



    private void Update()
    {
        stateMachine.Update();
        ViewDistanceCheck();
        stateText.SetText(stateMachine.currentState.ToString().Split('+')[1]);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, viewDistance);
    }

    public void ViewDistanceCheck()
    {
        #region //Function with OverlapSphere(not done/COMMENTED)
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
        #endregion
    }

    public void AddObjectToListOnce(GameObject obj, List<GameObject> myList)
    {
        if (!myList.Contains(obj))
        {
            myList.Add(obj);
        }
    }

}
