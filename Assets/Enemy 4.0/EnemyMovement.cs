using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("References: ")]
    public NavMeshAgent agent;
    public EnemyMaster enemy;
    public LayerMask enemyLayer;
    public Animator animator;
    private Collider[] myColliders;
    public Vector3 lastAccesiblePlayerLocation;

    [Header("States")]
    public bool canSeePlayer;
    public bool isChasingPlayer;
    public string currentMovementAnimation;

    [Header("Ranges: ")]
    public float runSpeed;
    public float patrolSpeed;
    public float announceRange;
    public float PreferedDistanceToPlayer;
    public float chaseDuration = 5f;
    public float rotateAfterPlayerSpeed = 5f;

    [Header("Patrolling")]
    public bool enablePatrol;
    public Transform[] patrolPoints;
    private int currentPatrolPoint = 0;


    private void Awake()
    {
        enemy = gameObject.GetComponent<EnemyMaster>();
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        myColliders = gameObject.GetComponentsInChildren<Collider>();

        if (patrolPoints.Length == 0)
            enablePatrol = false;

        currentMovementAnimation = "Idle";

        canSeePlayer = false;
        isChasingPlayer = false;
    }

    private void Update()
    {
        // Check if the enemy can see the player.
        canSeePlayer = enemy.visionCone.IsInView(Player.m.playerObject);//|| Vector3.Distance(transform.position,Player.m.transform.position) <= proximityRange

        if (canSeePlayer && !isChasingPlayer)
            StartCoroutine(ChasePlayer(chaseDuration));

        Move();
    }

    private void Move()
    {
        if (enemy.isStunned)
        {
            RotateAtTarget(gameObject.transform, Player.m.playerCam.orientation, rotateAfterPlayerSpeed);

            if (currentMovementAnimation != "Idle" && canTransition)
            {
                enemy.soundManager.Stop("enemyFootSteps");
                canTransition = false;
                Invoke(nameof(TransitionEnded), 0.26f);
                animator.SetTrigger(currentMovementAnimation = "Idle");
            }

            agent.SetDestination(transform.position);
            return;
        }

        agent.speed = patrolSpeed;
        enemy.animator.SetFloat("RunAnimSpeed", 1);

        if (isChasingPlayer)
        {
            agent.speed = runSpeed;
            enemy.animator.SetFloat("RunAnimSpeed", 1.2f);
            //create empty path
            NavMeshPath navMeshPath = new NavMeshPath();
            // check if navMeshAgent can reach player

            RaycastHit hit;
            Physics.Raycast(Player.m.transform.position, Player.m.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, Player.m.groundLayer);

            if (agent.CalculatePath(hit.point, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                lastAccesiblePlayerLocation = Player.m.transform.position;
                if (Vector3.Distance(enemy.EnemyCenter.position, lastAccesiblePlayerLocation) >= PreferedDistanceToPlayer)
                {
                    agent.SetDestination(lastAccesiblePlayerLocation);
                }
                else
                {
                    if (canSeePlayer)
                    {
                        agent.SetDestination(transform.position);
                        RotateAtTarget(gameObject.transform, Player.m.playerCam.orientation, rotateAfterPlayerSpeed);
                    }
                    else
                        agent.SetDestination(lastAccesiblePlayerLocation);
                }
            }
            else
            {
                //Fail condition here
                if (lastAccesiblePlayerLocation != Vector3.zero)
                    agent.SetDestination(lastAccesiblePlayerLocation);
                if (canSeePlayer)
                    RotateAtTarget(gameObject.transform, Player.m.playerCam.orientation, rotateAfterPlayerSpeed);
            }

        }
        else if (enablePatrol)
        {
            // Check if the enemy reached the destination.
            if (Vector3.Distance(enemy.EnemyCenter.position, patrolPoints[currentPatrolPoint].position) <= 2f)
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;

            agent.SetDestination(patrolPoints[currentPatrolPoint].position);
        }
        else
            agent.SetDestination(transform.position);

        // For the movement animation
        if (agent.velocity.magnitude <= 0.5f)
        {
            if (currentMovementAnimation != "Idle" && canTransition)
            {
                enemy.soundManager.Stop("enemyFootSteps");
                canTransition = false;
                Invoke(nameof(TransitionEnded), 0.26f);
                animator.SetTrigger(currentMovementAnimation = "Idle");
            }
        }
        else
        {
            if (currentMovementAnimation != "Run" && canTransition)
            {
                enemy.soundManager.StandardPlay("enemyFootSteps");
                canTransition = false;
                Invoke(nameof(TransitionEnded), 0.26f);
                animator.SetTrigger(currentMovementAnimation = "Run");
            }
        }
    }

    public void StartChasePlayer()
    {
        if (!isChasingPlayer)
            StartCoroutine(ChasePlayer(chaseDuration));
    }

    public IEnumerator ChasePlayer(float duration)
    {
        isChasingPlayer = true;

        float time = 0.0f;
        do
        {
            if (this == null)
                yield break;

            time += Time.deltaTime;

            if (canSeePlayer)
                time = 0.0f;

            Announce();

            yield return 0;

        } while (time < duration);

        isChasingPlayer = false;

        print(this.name + " stoppedChasingPlayer");
    }

    public void Announce(bool INeedToSeePlayerToAnnounce = true)
    {
        // Announce to all other enemies in range.
        Collider[] enemies = Physics.OverlapSphere(enemy.EnemyCenter.position, announceRange, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            // Condition to prevent announcing to himself.
            if (Array.IndexOf(myColliders, enemy) > -1)
                continue;

            EnemyMovement enemyMovement = enemy.gameObject.GetComponentInParent<EnemyMovement>();
            if (enemyMovement == null)
                continue;
            if (enemyMovement.enabled == false)
                continue;

            if (!enemyMovement.isChasingPlayer)
            {
                if (INeedToSeePlayerToAnnounce)
                {
                    if (canSeePlayer)
                    {
                        enemyMovement.StartChasePlayer();
                        //StartCoroutine(enemyMovement.ChasePlayer(enemyMovement.chaseDuration));
                    }
                }
                else
                {
                    enemyMovement.StartChasePlayer();
                    //StartCoroutine(enemyMovement.ChasePlayer(enemyMovement.chaseDuration));
                }
            }
        }
    }

    private bool canTransition = true;
    private void TransitionEnded() { canTransition = true; }

    private void RotateAtTarget(Transform objectToRotate, Transform target, float speed)
    {
        Vector3 relativePos = target.position - objectToRotate.position;
        Quaternion rotation = new Quaternion(0, Quaternion.LookRotation(relativePos).y, 0, Quaternion.LookRotation(relativePos).w);

        Quaternion current = new Quaternion(0, objectToRotate.localRotation.y, 0, objectToRotate.localRotation.w);

        objectToRotate.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime * speed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(enemy.EnemyCenter.position, announceRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(enemy.EnemyCenter.position, PreferedDistanceToPlayer);
    }
}
