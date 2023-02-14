using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    public NavMeshAgent agent;
    public VisionCone visionCone;

    [Header("Reacts to:")]
    public GameObject[] targets = new GameObject[1];
    private GameObject currentTarget = null;
    [Space]
    [Tooltip("Change target every X seconds")]
    public float resetTargetTimer;
    private float currentResetTimer;
    void Start()
    {
        targets[0] = Player.m.gameObject.GetComponentInChildren<CapsuleCollider>().gameObject;
    }

    void Update()
    {
        if (currentTarget == null)
        {
            foreach (GameObject target in targets)
            {
                if (visionCone.IsInView(target))
                {
                    currentTarget = target;
                    break;
                }
            }
        }
        else
        {
            if (visionCone.IsInView(currentTarget))
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
