using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    public NavMeshAgent agent;
    public VisionCone visionCone;

    [Header("Reacts to:")]
    public GameObject[] targets;
    private GameObject currentTarget = null;
    [Space]
    [Tooltip("Change target every X seconds")]
    public float resetTargetTimer;
    private float currentResetTimer;
    void Awake()
    {

    }

    void Update()
    {
        if (currentTarget == null)
        {
            foreach (GameObject target in targets)
            {
                if (visionCone.isInView(target))
                {
                    currentTarget = target;
                    break;
                }
            }
        }
        else
        {
            if (visionCone.isInView(currentTarget))
            {
                currentResetTimer = resetTargetTimer;
                agent.SetDestination(currentTarget.transform.position);
            }

            currentResetTimer -= Time.deltaTime;
            if (currentResetTimer <= 0f)
            {
                currentTarget = null;
            }
        }
    }
}
