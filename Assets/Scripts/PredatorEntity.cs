﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class PredatorEntity : MonoBehaviour
{
    public NavMeshAgent agent;
    public StateMachine<PredatorEntity> stateMachine;

    public PredatorViewDistance viewDistanceScript;
    public SphereCollider viewDistanceCollider;
    public float viewDistance;

    public Predator predator;
    
    public List<GameObject> animalsInRange = new List<GameObject>();

    public TextMeshProUGUI stateText;

    public float slowSpeed; //Idle
    public float mediumSpeed; //SearchPrey
    public float fastSpeed; //HuntPrey

    public bool preyFound = false;


    public class IdleState : State<PredatorEntity> //WanderAround wenn energy low and predator cant hunt
    {
        //Wander Around with reduced speed until energy is at a threshhold to hunt again
        //Low move speed

        public float wanderRadius = 50f; //not tested, probably needs to be set much lower 
        public float wanderTimer = 1f; //not tested

        private Transform target;
        private float timer;

        public override void Entered()
        {
            timer = wanderTimer;
            objectReference.agent.speed = objectReference.slowSpeed;
        }

        public override void Update()
        {
            objectReference.predator.energy += Time.deltaTime / objectReference.predator.timeToFullEnergy;
            timer += Time.deltaTime;
            if (timer >= wanderTimer)
            {
                Vector3 newPos = Helper.RandomNavSphere(objectReference.transform.position, wanderRadius, -1);
                objectReference.agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }

    //ÜBERDENKEN
    public class SearchPreyState : State<PredatorEntity> //wenn energy high enough and no prey in viewDistance, wander around searching for pray
    {
        //Wander around until prey is in viewDistance then transition to HuntPrey
        //either the predator has to be much faster or the predator still hunts when he initially found a animal in view distance even if it still leaves the distance
        //Medium move Speed

        public float wanderRadius = 50f; //not tested, probably needs to be set much lower 
        public float wanderTimer = 1f; //not tested

        private Transform target;
        private float timer;

        public override void Entered()
        {
            timer = wanderTimer;
            objectReference.agent.speed = objectReference.mediumSpeed;
        }

        public override void Update()
        {
            objectReference.predator.energy += Time.deltaTime / objectReference.predator.timeToFullEnergy;

            timer += Time.deltaTime;
            if (timer >= wanderTimer)
            {
                Vector3 newPos = Helper.RandomNavSphere(objectReference.transform.position, wanderRadius, -1);
                objectReference.agent.SetDestination(newPos);
                timer = 0;
            }

            objectReference.predator.FindClosestPrey();
            if (objectReference.predator.FindClosestPrey() != null)
            {
                objectReference.preyFound = true;
            }

            //float bestDistance = Mathf.Infinity;
            //GameObject bestPrey = null; //bestPlant = closest plant
            //foreach (var prey in objectReference.animalsInRange)
            //{
            //    if (prey != null)
            //    {
            //        float dist = Vector3.Distance(prey.transform.position, objectReference.transform.position);
            //        if (dist < bestDistance)
            //        {
            //            bestDistance = dist;
            //            bestPrey = prey;
            //        }
            //    }
            //}
        }
    }

    public class HuntPreyState : State<PredatorEntity> //found prey and runs to eat and its eat while using energy
    {
        //such den nähesten animal
        //laufe zu ihm
        //wenn distance klein genug essen --> animal destroyen
        //verbrauche energy während des huntPrey states, wenn kein energy dann wieder in idle sonst in serachPrey
        //High move Speed

        public override void Entered()
        {
            objectReference.preyFound = true;
            objectReference.agent.speed = objectReference.fastSpeed;
        }

        public override void Update()
        {
            //float bestDistance = Mathf.Infinity;
            //GameObject bestPrey = null; //bestPlant = closest plant
            //foreach (var prey in objectReference.animalsInRange)
            //{
            //    if (prey != null)
            //    {
            //        float dist = Vector3.Distance(prey.transform.position, objectReference.transform.position);
            //        if (dist < bestDistance)
            //        {
            //            bestDistance = dist;
            //            bestPrey = prey;
            //        }
            //    }
            //}

            //ODER
            GameObject bestPrey = objectReference.predator.FindClosestPrey();

            if (bestPrey != null)
            {
                //Debug.Log(bestPrey.transform.position);
                objectReference.agent.SetDestination(bestPrey.transform.position);
                objectReference.predator.ReduceEnergy();
            }
        }

        //public override void Exited()
        //{
        //    objectReference.preyFound = false;
        //}
    }

    public class EatPreyState : State<PredatorEntity>
    {
        //wenn distance klein genug isst er ihn und transitioned entweder wieder zu serachfood wenn energy high enough oder zu idle wenn nicht high enough
        public override void Update()
        {
            //float bestDistance = Mathf.Infinity;
            //GameObject bestPrey = null; //bestPlant = closest plant
            //foreach (var prey in objectReference.animalsInRange)
            //{
            //    if (prey != null)
            //    {
            //        float dist = Vector3.Distance(prey.transform.position, objectReference.transform.position);
            //        if (dist < bestDistance)
            //        {
            //            bestDistance = dist;
            //            bestPrey = prey;
            //        }
            //    }
            //}

            GameObject bestPrey = objectReference.predator.FindClosestPrey();

            if (bestPrey != null)
            {
                Debug.Log("bestPrey transPos: " + bestPrey.transform.position);
                objectReference.predator.EatPreyFunction(bestPrey);
            }
            if (bestPrey == null)
            {
                objectReference.preyFound = false; //THIS WAS IT check if its null instead of in Exited beacuse it will never reach Exited() !!!! (maybe the same in statemachineEntity
            }
        }

        //public override void Exited()
        //{
        //    objectReference.preyFound = false;
        //}
    }

    public class NoEnergyTransition : Transition<PredatorEntity> //wenn er keine energy mehr hat geht er wieder in Idle
    {
        public override bool GetIsAllowed()
        {
            if (objectReference.predator.energy <= 0f && objectReference.preyFound == false) //wenn energy unter energythreshhold liegt
            {
                return true;
            }

            return false;
        }
    }

    public class HasEnergyTransition : NoEnergyTransition //oder auch einfach transition<PredatorEntity>, Wenn er energy hat geht er von idle in SearchPrey
    {
        public override bool GetIsAllowed()
        {
            if (objectReference.predator.energy > objectReference.predator.energyThreshhold && objectReference.preyFound == false)
            {
                return true;
            }

            return false;
        }
    }

    public class HasFoundPrey : Transition<PredatorEntity> //wenn er prey gefunden hat geht er von SearchPrey zu HuntPrey
    {
        public override bool GetIsAllowed()
        {
            if (objectReference.preyFound == true)
            {
                return true;
            }

            return false;
        }
    }
    
    public class IsCloseEnoughToPrey : Transition<PredatorEntity>
    {
        public override bool GetIsAllowed()
        {
            GameObject bestPrey = objectReference.predator.FindClosestPrey();
            if (bestPrey != null)
            {
                if (Vector3.Distance(objectReference.transform.position, bestPrey.transform.position) <= 20f)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        predator = GetComponent<Predator>();
        viewDistanceCollider = GetComponentInChildren<SphereCollider>();
        viewDistance = viewDistanceCollider.radius * 10f;

        stateMachine = new StateMachine<PredatorEntity>();

        stateMachine.AddState(new IdleState() { objectReference = this }, "Idle");
        stateMachine.AddState(new SearchPreyState() { objectReference = this }, "SearchPrey");
        stateMachine.AddState(new HuntPreyState() { objectReference = this }, "HuntPrey");
        stateMachine.AddState(new EatPreyState() { objectReference = this }, "EatPrey");
        //maybe another state EatPrey after HuntPrey?

        stateMachine.SetInitialState("Idle"); //von da aber direkt in SearchPrey gehen wenn genug energy und hunger groß genug

        stateMachine.AddTransition(new NoEnergyTransition() { objectReference = this }, "HuntPrey", "Idle");
        stateMachine.AddTransition(new HasEnergyTransition() { objectReference = this }, "Idle", "SearchPrey");
        stateMachine.AddTransition(new HasFoundPrey() { objectReference = this }, "SearchPrey", "HuntPrey");
        stateMachine.AddTransition(new IsCloseEnoughToPrey() { objectReference = this }, "HuntPrey", "EatPrey");
        stateMachine.AddTransition(new HasEnergyTransition() { objectReference = this }, "EatPrey", "SearchPrey");
        stateMachine.AddTransition(new NoEnergyTransition() { objectReference = this }, "EatPrey", "Idle");

        //MAYBE
        //stateMachine.AddTransition(new NoEnergyTransition() { objectReference = this }, "SearchPrey", "Idle");


    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceCollider.radius * 10f);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        Debug.Log(stateMachine.currentState);
        stateText.SetText(stateMachine.currentState.ToString());
    }
}
