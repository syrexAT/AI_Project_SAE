using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PredatorEntity : MonoBehaviour
{
    public NavMeshAgent agent;
    public StateMachine<PredatorEntity> stateMachine;

    public PredatorViewDistance viewDistanceScript;
    public SphereCollider viewDistanceCollider;
    public float viewDistance;

    public List<GameObject> animalsInRange = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }
}
