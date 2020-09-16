using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

//STATE MACHINE FOR ANIMAL(RABBIT)
public class StateMachineEntity : MonoBehaviour
{
    //Vielleicht in singelton umwandeln, und static vermeiden?
    public static StateMachineEntity _instance;


    public NavMeshAgent agent;

    public StateMachine<StateMachineEntity> stateMachine;

    public float moveSpeed = 2f;

   /* public static float viewRadius = 10f;*/ //this needs to be the radius of the view radius detection collider on the child of statemachinentity!!!!
    public ViewDistanceScript viewDistanceScript;
    public SphereCollider viewDistanceCollider;
    public float viewDistance;

    public List<GameObject> plantsInRange = new List<GameObject>(); //Static weil ich sonst nicht im SearchFoodState unter Update drauf zugreifen kann
    public List<GameObject> predatorInRange = new List<GameObject>();
    public List<Vector2> waterInRange = new List<Vector2>();

    public Animal animal;


    public float drinkRemainingTime;

    public bool drinkTimesUp;
    public bool drinkTimerIsRunning = false;
    public bool drinkTimerIsUp = false;

    public bool finishedDrinking = false;
    public float drinkTime;
    public bool finishedEating = false;
    public float eatTime;

    public bool inWaterState = false;
    public bool inFoodState = false;

    public TextMeshProUGUI stateText;


    #region States
    //Need to detect water tiles, they have a noise float below 0.4 (see inspector -> MapGenerator)
    //Get an array/list? of all water tiles in view Distance, go to nearest one
    public class SearchWaterState : State<StateMachineEntity>
    {
        public override void Entered()
        {
            objectReference.drinkTimesUp = false;
            objectReference.drinkTime = Time.time;
            objectReference.finishedDrinking = false;
            //objectReference.inWaterState = true;
            

        }

        public override void Update()
        {
            //Find Water in View Distance --> gehört wo anders hin? weil er hierfür in dem state sein muss


            //float bestDistance = Mathf.Infinity;
            //waterlist durchgehen abfragen ob die distance zwischen animal un punkt kleiner als viewdistance
            //von allen pnktne kriegt man die distance,  --> genauso wie unten
            Debug.Log("WaterInRange: " + objectReference.waterInRange);
            Debug.Log("IN SEARCHWATERSTATE");

            float bestDistance = Mathf.Infinity; //glaub ich?
            Vector3 bestWater = Vector3.zero;
            foreach (var water in objectReference.waterInRange)
            {
                Debug.Log("IN SEARCHWATERSTATE");
                float dist = Vector3.Distance(new Vector3(water.x, 0, water.y), objectReference.transform.position);
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestWater = water;
                }
            }

            if (bestWater != Vector3.zero)
            {
                objectReference.agent.SetDestination(new Vector3(bestWater.x, 0, bestWater.y)); //Vector3?
                //if (!objectReference.agent.pathPending)
                //{
                //    if (objectReference.agent.remainingDistance <= objectReference.agent.stoppingDistance)
                //    {
                //        if (!objectReference.agent.hasPath || objectReference.agent.velocity.sqrMagnitude == 0f)
                //        {
                //            //here he reached the destination/waterTile
                //            if (objectReference.animal.thirst > 0)
                //            {
                //                //float drinkDuration = objectReference.animal.drinkDuration;
                //                //drinkDuration -= Time.deltaTime;
                //                Debug.Log("Inside Drinking");
                //                //Debug.Log(objectReference.agent.velocity.sqrMagnitude);
                //                //objectReference.animal.isDrinking = true;
                //                objectReference.animal.thirst -= Time.deltaTime * 1 / objectReference.animal.drinkDuration;
                //                objectReference.animal.thirst = Mathf.Clamp01(objectReference.animal.thirst);
                //                if (Mathf.Approximately(objectReference.animal.thirst, 0f))
                //                {
                //                    //objectReference.animal.isDrinking = false;
                //                    //objectReference.finishedDrinking = true;
                //                }

                //                //Debug.Log(Time.deltaTime * 1 / objectReference.animal.drinkDuration);
                //            }
                //        }
                //    }
                //}
            }
            Debug.Log("WaterInRange: " + objectReference.waterInRange);

            //Debug.Log(objectReference.agent.velocity.sqrMagnitude);
        }
    }

    public class DrinkWaterState : State<StateMachineEntity>
    {
        public override void Entered()
        {
            objectReference.finishedDrinking = false;
            objectReference.drinkTime = 0;
        }

        public override void Update()
        {
            //Debug.Log("WaterInRange: " + objectReference.waterInRange);
            //Debug.Log("IN SEARCHWATERSTATE");

            //float bestDistance = Mathf.Infinity; //glaub ich?
            //Vector3 bestWater = Vector3.zero;
            //foreach (var water in objectReference.waterInRange)
            //{
            //    Debug.Log("IN SEARCHWATERSTATE");
            //    float dist = Vector3.Distance(new Vector3(water.x, 0, water.y), objectReference.transform.position);
            //    if (dist < bestDistance)
            //    {
            //        bestDistance = dist;
            //        bestWater = water;
            //    }
            //}
            
            if (!objectReference.agent.pathPending)
            {
                if (objectReference.agent.remainingDistance <= objectReference.agent.stoppingDistance)
                {
                    if (!objectReference.agent.hasPath || objectReference.agent.velocity.sqrMagnitude == 0f)
                    {
                        //here he reached the destination/waterTile
                        if (objectReference.animal.thirst > 0)
                        {
                            objectReference.drinkTime += Time.deltaTime;
                            Debug.Log("Inside Drinking");
                            objectReference.animal.thirst -= Time.deltaTime * 1 / objectReference.animal.drinkDuration;
                            objectReference.animal.thirst = Mathf.Clamp01(objectReference.animal.thirst);
                            if (objectReference.drinkTime >= objectReference.animal.drinkDuration)
                            {
                                objectReference.finishedDrinking = true;
                            }


                            if (Mathf.Approximately(objectReference.animal.thirst, 0f))
                            {
                                //objectReference.animal.isDrinking = false;
                                //objectReference.finishedDrinking = true;
                            }

                            //Debug.Log(Time.deltaTime * 1 / objectReference.animal.drinkDuration);
                        }
                    }
                }
            }
        }

        public override void Exited()
        {
            objectReference.finishedDrinking = false;
        }

    }

    //Sollte funktionieren, es muss noch hunger eingefügt werden
    public class SearchFoodState : State<StateMachineEntity>
    {
        public override void Update()
        {
            float bestDistance = Mathf.Infinity;
            GameObject bestPlant = null; //bestPlant = closest plant
            foreach (var plant in objectReference.plantsInRange)
            {
                if (plant != null)
                {
                    float dist = Vector3.Distance(plant.transform.position, objectReference.transform.position);
                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        bestPlant = plant;
                    }
                }
            }
            Debug.Log(bestPlant);

            if (bestPlant != null)
            {
                //zur nähesten pflanze gehen
                objectReference.agent.SetDestination(bestPlant.transform.position);
                //hier food essen, code ausführen das pflanze kleiner wird und dann deleted wird und er währenddessen isst

                //if (!objectReference.agent.pathPending)
                //{
                //    if (objectReference.agent.remainingDistance <= objectReference.agent.stoppingDistance)
                //    {
                //        if (!objectReference.agent.hasPath || objectReference.agent.velocity.sqrMagnitude == 0f)
                //        {
                //            //here he reached the destination/waterTile
                //            if (objectReference.animal.hunger != 0)
                //            {
                //                float eatAmount = Mathf.Min(objectReference.animal.hunger, Time.deltaTime * 1 / objectReference.animal.eatDuraton);
                //                PlantScript foodTarget;
                //                foodTarget = bestPlant.GetComponent<PlantScript>();
                //                eatAmount = foodTarget.Consume(eatAmount); //wird nichtmehr ausgeführt? warum? nach transition serachFood->WanderAround vielleicht wegen dem moreHungry?
                //                objectReference.animal.hunger -= eatAmount;
                //                //objectReference.animal.hunger = Mathf.Clamp01(objectReference.animal.hunger);
                //                if (foodTarget.AmountRemaining <= 0)
                //                {
                //                    bestPlant = null;
                //                }
                //            }
                //        }
                //    }
                //}
            }
        }
    }

    public class EatFoodState : State<StateMachineEntity>
    {
        public override void Entered()
        {
            objectReference.finishedEating = false;
            objectReference.eatTime = 0;
        }

        public override void Update()
        {
            float bestDistance = Mathf.Infinity;
            GameObject bestPlant = null; //bestPlant = closest plant
            foreach (var plant in objectReference.plantsInRange)
            {
                if (plant != null)
                {
                    float dist = Vector3.Distance(plant.transform.position, objectReference.transform.position);
                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        bestPlant = plant;
                    }
                }
            }

            if (bestPlant != null)
            {
                if (!objectReference.agent.pathPending)
                {
                    if (objectReference.agent.remainingDistance <= objectReference.agent.stoppingDistance)
                    {
                        if (!objectReference.agent.hasPath || objectReference.agent.velocity.sqrMagnitude == 0f)
                        {
                            //here he reached the destination/waterTile
                            if (objectReference.animal.hunger > 0)
                            {
                                objectReference.eatTime += Time.deltaTime;
                                float eatAmount = Mathf.Min(objectReference.animal.hunger, Time.deltaTime * 1 / objectReference.animal.eatDuraton);
                                PlantScript foodTarget;
                                foodTarget = bestPlant.GetComponent<PlantScript>();
                                eatAmount = foodTarget.Consume(eatAmount); //wird nichtmehr ausgeführt? warum? nach transition serachFood->WanderAround vielleicht wegen dem moreHungry?
                                objectReference.animal.hunger -= eatAmount;
                                //objectReference.animal.hunger = Mathf.Clamp01(objectReference.animal.hunger);
                                if (/*foodTarget.AmountRemaining <= 0 || */objectReference.eatTime >= objectReference.animal.eatDuraton)
                                {
                                    objectReference.finishedEating = true;
                                    bestPlant = null;
                                }

                            }
                        }
                    }
                }
            }

        }

        public override void Exited()
        {
            objectReference.finishedEating = false;
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
                float distance = Vector3.Distance(objectReference.transform.position, bestPredator.transform.position); //distance between predator and animal
                //Debug.Log("Distance: " + distance);

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
                Vector3 newPos = RandomNavSphere(objectReference.transform.position, wanderRadius, -1);
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
            //if (predatorInRange.)
            //{

            //}
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
            float bestDistance = Mathf.Infinity;
            GameObject bestPlant = null; //bestPlant = closest plant
            foreach (var plant in objectReference.plantsInRange)
            {
                if (plant != null)
                {
                    float dist = Vector3.Distance(plant.transform.position, objectReference.transform.position);
                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        bestPlant = plant;
                    }
                }

            }

            if (bestPlant != null && (objectReference.animal.moreHungry/*|| objectReference.animal.criticalPercent > objectReference.animal.hunger*/))
            {
                //zur nähesten pflanze gehen
                float distance = Vector3.Distance(objectReference.transform.position, bestPlant.transform.position);
                objectReference.agent.SetDestination(bestPlant.transform.position);
                return distance < objectReference.viewDistance/* && objectReference.animal.moreHungry*/;/* && objectReference.animal.isDrinking == false && objectReference.finishedDrinking == true;*//* && objectReference.animal.moreHungry;*/ //Testing it with the && statement
            }

            return false;
        }
    }

    public class FoodOutOfRangeTransition : FoodInRangeTransition //hier soll er in WanderAround gehen
    {
        public override bool GetIsAllowed()
        {
            return !base.GetIsAllowed();
        }
    }

    public class AtFoodSourceTransition : Transition<StateMachineEntity>
    {
        public override bool GetIsAllowed()
        {
            if (!objectReference.agent.pathPending)
            {
                if (objectReference.agent.remainingDistance <= objectReference.agent.stoppingDistance)
                {
                    if (!objectReference.agent.hasPath || objectReference.agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
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
                    if (!objectReference.agent.hasPath || objectReference.agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    

    public class FinishedEatingTransition : Transition<StateMachineEntity> //hier entweder bis die zeit vergangen ist(die pflanze aufgegessen ist) oder hunger bei einem kleinen wert ist
    {
        public override bool GetIsAllowed()
        {
            //float bestDistance = Mathf.Infinity;
            //GameObject bestPlant = null; //bestPlant = closest plant
            //foreach (var plant in objectReference.plantsInRange)
            //{
            //    if (plant != null)
            //    {
            //        float dist = Vector3.Distance(plant.transform.position, objectReference.transform.position);
            //        if (dist < bestDistance)
            //        {
            //            bestDistance = dist;
            //            bestPlant = plant;
            //        }
            //    }
            //}

            //if (bestPlant != null)
            //{
            //    if (!objectReference.agent.pathPending)
            //    {
            //        if (objectReference.agent.remainingDistance <= objectReference.agent.stoppingDistance)
            //        {
            //            if (!objectReference.agent.hasPath || objectReference.agent.velocity.sqrMagnitude == 0f)
            //            {
            //                //here he reached the destination/waterTile
            //                if (objectReference.animal.hunger != 0)
            //                {
            //                    float eatAmount = Mathf.Min(objectReference.animal.hunger, Time.deltaTime * 1 / objectReference.animal.eatDuraton);
            //                    PlantScript foodTarget;
            //                    foodTarget = bestPlant.GetComponent<PlantScript>();
            //                    eatAmount = foodTarget.Consume(eatAmount); //wird nichtmehr ausgeführt? warum? nach transition serachFood->WanderAround vielleicht wegen dem moreHungry?
            //                    objectReference.animal.hunger -= eatAmount;
            //                    //objectReference.animal.hunger = Mathf.Clamp01(objectReference.animal.hunger);
            //                    if (foodTarget.AmountRemaining <= 0)
            //                    {
            //                        bestPlant = null;
            //                    }

            //                    return foodTarget.AmountRemaining <= 0 || objectReference.animal.hunger <= 0;//geht das weils ja nie ganz 0 ist? weils konstant raufgeht?

            //                }
            //            }
            //        }
            //    }
            //}

            //return false;

            if (objectReference.finishedEating) //MUSS NOCH DEFINIERT WERDEN WANN IST ER FERTIG MIT ESSEN? PSEUDO CODE!
            {
                return true;
            }

            return false;

        }
    }

    public class NotFinishedEatingTransition : FinishedEatingTransition
    {
        public override bool GetIsAllowed()
        {
            return !base.GetIsAllowed();
        }
    }

    public class WaterInRangeTransition : Transition<StateMachineEntity> //state muss noch geschrieben werden //Hier soll er in SearchWater gehen
    {
        public override bool GetIsAllowed()
        {
            float bestDistance = Mathf.Infinity; //glaub ich?
            Vector3 bestWater = Vector3.zero;
            foreach (var water in objectReference.waterInRange)
            {
                float dist = Vector3.Distance(new Vector3(water.x, 0 ,water.y), objectReference.transform.position);
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestWater = water;
                }
            }

            if (bestWater != Vector3.zero && objectReference.animal.moreThirsty)
            {
                /*objectReference.agent.SetDestination(new Vector2(bestWater.x, bestWater.y));*/ //Vector3?
                float distance = Vector3.Distance(objectReference.transform.position, bestWater);
                return distance < objectReference.viewDistance/* && objectReference.animal.moreThirsty*/;
            }

            return false;
        }
    }

    public class WaterOutOfRangeTransition : WaterInRangeTransition //state muss noch geschrieben werden //Hier soll er ins WanderAround gehen
    {
        public override bool GetIsAllowed()
        {
            return !base.GetIsAllowed();
        }
    }

    public class FinishedDrinkingTransition : Transition<StateMachineEntity> //hier sagen wenn er fertig ist mit trinken also entweder nach zeit oder wenn durst einen kleinen wert erreicht hat
    {
        public override bool GetIsAllowed()
        {
            //float bestDistance = Mathf.Infinity; //glaub ich?
            //Vector3 bestWater = Vector3.zero;
            //foreach (var water in objectReference.waterInRange)
            //{
            //    Debug.Log("IN SEARCHWATERSTATE");
            //    float dist = Vector3.Distance(new Vector3(water.x, 0, water.y), objectReference.transform.position);
            //    if (dist < bestDistance)
            //    {
            //        bestDistance = dist;
            //        bestWater = water;
            //    }
            //}

            //if (!objectReference.agent.pathPending)
            //{
            //    if (objectReference.agent.remainingDistance <= objectReference.agent.stoppingDistance)
            //    {
            //        if (!objectReference.agent.hasPath || objectReference.agent.velocity.sqrMagnitude == 0f)
            //        {
            //            return true;
            //            //here he reached the destination/waterTile
            //            if (objectReference.animal.thirst > 0)
            //            {
            //                Debug.Log("Inside Drinking");
            //                objectReference.animal.thirst -= Time.deltaTime * 1 / objectReference.animal.drinkDuration;
            //                objectReference.animal.thirst = Mathf.Clamp01(objectReference.animal.thirst);
            //                if (Mathf.Approximately(objectReference.animal.thirst, 0f))
            //                {
            //                    //objectReference.animal.isDrinking = false;
            //                    //objectReference.finishedDrinking = true;
            //                }

            //               /* return objectReference.animal.thirst = 0;*///???? geht das so??

            //                //Debug.Log(Time.deltaTime * 1 / objectReference.animal.drinkDuration);
            //            }
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //}

            //return false;

            if (objectReference.finishedDrinking) //MUSS NOCH DEFINIERT WERDEN WANN IST ER FERTIG MIT TRINKEN? PSEUDO CODE!
            {
                return true;
            }

            return false;

        }
    }

    public class NotFinishedDrinkingTransition : FinishedDrinkingTransition
    {
        public override bool GetIsAllowed()
        {
            return !base.GetIsAllowed();
        }
    }

    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animal = GetComponent<Animal>();
        viewDistanceCollider = GetComponentInChildren<SphereCollider>();
        viewDistance = viewDistanceCollider.radius * 10f; //noch * 2 ????, *10f weil der viewDistanceCollider 1x1x1 ist und nicht 10x10x10 wie der Animal selber
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

/*        stateMachine.SetInitialState("Idle");*/ //IDLE ODER WANDERAROUND?
        stateMachine.SetInitialState("WanderAround");
        #endregion

        #region Transitions
        //Transitions (stateMachine.AddTransition)
        //!!! Vielleicht immer das WanderAround dazwische haben und nicht direkt von SearchFood zu SearchWater / und andersrum gehen !!!
        //stateMachine.AddTransition(new)
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "SearchFood", "EvadePredator");
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "SearchWater", "EvadePredator");
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "WanderAround", "EvadePredator");
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "Idle", "EvadePredator"); //may be removed
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "EatFood", "EvadePredator");
        stateMachine.AddTransition(new PredatorInRangeTransition() { objectReference = this }, "DrinkWater", "EvadePredator");


        /*stateMachine.AddTransition(new PredatorOutOfRangeTransition() { objectReference = this }, "EvadePredator", "Idle");*/ //may be removed, idle is the wanderAround state probably
        stateMachine.AddTransition(new PredatorOutOfRangeTransition() { objectReference = this }, "EvadePredator", "WanderAround");

        //wenn mehr durst als hunger und kein wasser in sicht, befindet er sich in WanderAround und wenn wasser in Sicht macht er SearchWater
        stateMachine.AddTransition(new FoodInRangeTransition() { objectReference = this }, "WanderAround", "SearchFood"); //wenn er nicht direkt eine Pflanze in Range hat ist er ja im WanderAround, da soll er dann ins Searchfood gehen wenns in Range ist und er den hunger hat

        /*stateMachine.AddTransition(new FoodInRangeTransition() { objectReference = this }, "SearchWater", "SearchFood");*/ //wenn er den hunger hat kann er direkt von SearchWater zu SearchFood WENN eine pflanze in range ist

        //wenn mehr hunger als durst und keine Pflanze in sicht, befindet er sich in WanderAround und wenn Pflanze in Sicht macht er SearchFood
        /*stateMachine.AddTransition(new FoodOutOfRangeTransition() { objectReference = this }, "SearchFood", "WanderAround");*/ //wenn er keine pflanze in view distance hat soll er wanderAround bis er food findet
        //stateMachine.AddTransition(new FoodOutOfRangeTransition() { objectReference = this }, "SearchFood", "WanderAround");
        stateMachine.AddTransition(new FinishedEatingTransition() { objectReference = this }, "EatFood", "WanderAround");


        stateMachine.AddTransition(new WaterInRangeTransition() { objectReference = this }, "WanderAround", "SearchWater"); //wenn es nicht in range war und er wandered und findet wasser --> dann searchWater
        /*stateMachine.AddTransition(new WaterOutOfRangeTransition() { objectReference = this }, "SearchWater", "WanderAround");*/ //wenn er direkt water in view distance hat (und durst hat)

        stateMachine.AddTransition(new FinishedDrinkingTransition() { objectReference = this }, "DrinkWater", "WanderAround");

        stateMachine.AddTransition(new AtFoodSourceTransition() { objectReference = this }, "SearchFood", "EatFood");
        stateMachine.AddTransition(new AtWaterSourceTransition() { objectReference = this }, "SearchWater", "DrinkWater");
        /*stateMachine.AddTransition(new WaterOutOfRangeTransition() { objectReference = this }, "SearchFood", "WanderAround");*/ // Bei dem moved er dann garnichtmehr bei Gamestart
        #endregion


    }

    

    private void Update()
    {
        stateMachine.Update();
        ViewDistanceCheck();
        stateText.SetText(stateMachine.currentState.ToString());
        Debug.Log("States " + stateMachine.currentState);
        Debug.Log("Transitions " + stateMachine.currentTransitions);
        //foreach (var water in MapGenerator.waterList)
        //{
        //    if (Vector3.Distance(new Vector2(water.x, water.y), objectReference.transform.position) <= objectReference.viewDistance)
        //    {
        //        objectReference.waterInRange.Add(water);
        //    }
        //}


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
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

        //Checking is handled in ViewDistanceScript
        //Debug.Log("Plants: " + plantsInRange.Count);
        //Debug.Log("Predator " + predatorInRange.Count);
        //Debug.Log("WaterTiles " + waterInRange.Count);
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
