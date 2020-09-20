using System.Collections;
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

    #region States
    public class IdleState : State<PredatorEntity> //WanderAround wenn energy low and predator cant hunt
    {
        //Wander Around with reduced speed until energy is at a threshhold to hunt again
        //Low move speed

        public float wanderRadius = 100f;
        public float wanderTimer = 3f;

        private Transform target;
        private float timer;

        public override void Entered()
        {
            timer = wanderTimer;
            objectReference.agent.speed = objectReference.slowSpeed;
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

    public class SearchPreyState : State<PredatorEntity> //wenn energy high enough and no prey in viewDistance, wander around searching for pray
    {
        public float wanderRadius = 100f; 
        public float wanderTimer = 1f; 

        private Transform target;
        private float timer;

        public override void Entered()
        {
            timer = wanderTimer;
            objectReference.agent.speed = objectReference.mediumSpeed;
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

    public class HuntPreyState : State<PredatorEntity> //found prey and runs to eat and its eat while using energy
    {
        public override void Entered()
        {
            objectReference.preyFound = true;
            objectReference.agent.speed = objectReference.fastSpeed;
            objectReference.predator.currentlyHuntedAnimal = objectReference.predator.FindClosestPrey();

        }

        public override void Update()
        {
            if (objectReference.predator.currentlyHuntedAnimal != null)
            {
                objectReference.agent.SetDestination(objectReference.predator.currentlyHuntedAnimal.transform.position);
                objectReference.predator.ReduceEnergy();
            }
        }
    }

    public class EatPreyState : State<PredatorEntity>
    {
        public override void Entered()
        {

            if (objectReference.predator.currentlyHuntedAnimal != null)
            {
                objectReference.predator.EatPreyFunction(objectReference.predator.currentlyHuntedAnimal);
            }
        }
    }

    #endregion

    #region Transitions
    public class NoEnergyTransition : Transition<PredatorEntity> //wenn er keine energy mehr hat geht er wieder in Idle
    {
        public override bool GetIsAllowed()
        {
            if (objectReference.predator.energy <= 0f/* && objectReference.preyFound == false*/ || objectReference.predator.currentlyHuntedAnimal == null) //wenn energy unter energythreshhold liegt ODER der currentlyHuntedAnimal null ist (z.B. wenn 2 predators auf selben animal gehen und der eine isst ihn)
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
            if (objectReference.predator.energy > objectReference.predator.energyThreshhold && objectReference.predator.hunger > objectReference.predator.hungerThreshhold)
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
            if (objectReference.predator.FindClosestPrey() != null)
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
            if (objectReference.predator.currentlyHuntedAnimal != null)
            {
                if (Vector3.Distance(objectReference.transform.position, objectReference.predator.currentlyHuntedAnimal.transform.position) <= 20f)
                {
                    return true;
                }
            }

            return false;
        }
    }
    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        predator = GetComponent<Predator>();
        viewDistanceCollider = GetComponentInChildren<SphereCollider>();
        viewDistance = viewDistanceCollider.radius * 10f;

        stateMachine = new StateMachine<PredatorEntity>();
        #region States
        stateMachine.AddState(new IdleState() { objectReference = this }, "Idle");
        stateMachine.AddState(new SearchPreyState() { objectReference = this }, "SearchPrey");
        stateMachine.AddState(new HuntPreyState() { objectReference = this }, "HuntPrey");
        stateMachine.AddState(new EatPreyState() { objectReference = this }, "EatPrey");


        stateMachine.SetInitialState("Idle"); //von da aber direkt in SearchPrey gehen wenn genug energy und hunger groß genug
        #endregion

        #region Transitions
        stateMachine.AddTransition(new NoEnergyTransition() { objectReference = this }, "HuntPrey", "Idle");
        stateMachine.AddTransition(new HasEnergyTransition() { objectReference = this }, "Idle", "SearchPrey");
        stateMachine.AddTransition(new HasFoundPrey() { objectReference = this }, "SearchPrey", "HuntPrey");
        stateMachine.AddTransition(new IsCloseEnoughToPrey() { objectReference = this }, "HuntPrey", "EatPrey");
        stateMachine.AddTransition(new HasEnergyTransition() { objectReference = this }, "EatPrey", "SearchPrey");
        stateMachine.AddTransition(new NoEnergyTransition() { objectReference = this }, "EatPrey", "Idle");
        #endregion

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceCollider.radius * 10f);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        //Debug.Log(stateMachine.currentState);
        stateText.SetText(stateMachine.currentState.ToString().Split('+')[1]);
    }
}
