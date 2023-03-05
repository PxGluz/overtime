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

    [Header("States")]
    public bool canSeePlayer;
    public bool isChasingPlayer;
    public string currentMovementAnimation;

    [Header("Ranges: ")]
    public float announceRange;
    public float PreferedDistanceToPlayer;
    public float chaseDuration = 5f;

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
        myColliders = gameObject.GetComponentsInChildren<Collider>() ;

        if (patrolPoints.Length == 0)
            enablePatrol = false;

        currentMovementAnimation = "Idle";
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
        if (isChasingPlayer)
        {
           // RotateAtTarget(gameObject.transform, Player.m.playerCam.orientation, 5);

            if (Vector3.Distance(transform.position, Player.m.transform.position) >= PreferedDistanceToPlayer)
            {
                agent.SetDestination(Player.m.transform.position);
            }
            else
            {
                if (canSeePlayer)
                    agent.SetDestination(transform.position);
                else
                    agent.SetDestination(Player.m.transform.position);
            }
        }
        else if (enablePatrol)
        {
            // Check if the enemy reached the destination.
            if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) <= 2f)
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
                canTransition = false;
                Invoke(nameof(TransitionEnded), 0.26f);
                animator.SetTrigger(currentMovementAnimation = "Idle");
            }
        }
        else
        {
            if (currentMovementAnimation != "Run" && canTransition)
            {
                canTransition = false;
                Invoke(nameof(TransitionEnded), 0.26f);
                animator.SetTrigger(currentMovementAnimation = "Run");
            }
        }
    }
    public IEnumerator ChasePlayer(float duration)
    {
        isChasingPlayer = true;
       // agent.updateRotation = false;

        float time = 0.0f;
        do
        {
            time += Time.deltaTime;

            if (canSeePlayer)
                time = 0.0f;

            Announce();

            yield return 0;

        } while (time < duration);

     //   isChasingPlayer = false;
        agent.updateRotation = true;

        print ("stoppedChasingPlayer");
    }
    public void Announce(bool INeedToSeePlayerToAnnounce = true)
    {
        // Announce to all other enemies in range.
        Collider[] enemies = Physics.OverlapSphere(gameObject.transform.position, announceRange, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            // Condition to prevent announcing to himself.
            if (Array.IndexOf(myColliders, enemy) > -1)
                continue;

            EnemyMovement enemyMovement = enemy.transform.parent.GetComponent<EnemyMovement>();
            if (enemyMovement == null)
                continue;

            if (!enemyMovement.isChasingPlayer )
            {
                if (INeedToSeePlayerToAnnounce) {
                    if (canSeePlayer)
                        StartCoroutine(enemyMovement.ChasePlayer(enemyMovement.chaseDuration));
                } else
                    StartCoroutine(enemyMovement.ChasePlayer(enemyMovement.chaseDuration));
            }
        }
    }

    private bool canTransition = true;
    private void TransitionEnded() { canTransition = true; }

    private void RotateAtTarget(Transform objectToRotate, Transform target,float speed)
    {
        Vector3 relativePos = target.position - objectToRotate.position;
        Quaternion rotation = new Quaternion(0, Quaternion.LookRotation(relativePos).y,0, Quaternion.LookRotation(relativePos).w);

        Quaternion current = new Quaternion (0,objectToRotate.localRotation.y,0, objectToRotate.localRotation.w);

        objectToRotate.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime* speed);
    }



    
    /*
    private void Patrol()
    {
        // Get next position to patrol.
        if (!walkPointSet)
        {
            if (randomPatrolling)
                SearchWalkPoint();
            else
            {
                walkPoint = walkPoints[walkPointIndex];
                walkPointIndex = (walkPointIndex + 1) % walkPoints.Length;
            }

            walkPointSet = true;
        }

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        // Check if the enemy reached the destination.
        if (Vector3.Distance(gameObject.transform.position, walkPoint) <= 1f)
            walkPointSet = false;
    }

    // Generates random point for patrolling.
    private void SearchWalkPoint()
    {
        do
        {
            float randomX = Random.Range(-walkPointRange, walkPointRange);
            float randomZ = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = gameObject.transform.position + new Vector3(randomX, 0, randomZ);
        } while (!Physics.Raycast(walkPoint, -gameObject.transform.up, 2f, groundLayer));
    }
    */

    /*
    private void Chase(Vector3 position, bool sawPlayer)
    {
        agent.SetDestination(position);

        // If the enemy didn't see the player himself, discards announced information when arriving.
        if (!sawPlayer && Vector3.Distance(gameObject.transform.position, position) < 1f)
        {
            gotAnnounced = false;
            rememberPlayer = false;
        }
    }
    */

    /*
    private void Attack()
    {
        // Stop the enemy.
        agent.SetDestination(gameObject.transform.position);

        // Attack.
        if (!alreadyAttacked)
        {
            //animator.SetLayerWeight(1, 1);
            //animator.Play(attack, 1);
            alreadyAttacked = true;
            if (enemyType == EnemyType.Ranged)
            {
                enemyShooting.Shoot(Player.m.transform, damage);
            }
            else
            {
                Player.m.TakeDamage(damage);
            }

            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }
    

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    */

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(gameObject.transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, announceRange);
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(gameObject.transform.position, proximityRange);
        
        //Vector3 fwd = gameObject.transform.forward * visionCone.visionDistance;
        //Gizmos.color = Color.white;
        //Gizmos.DrawRay(visionCone.gameObject.transform.position, fwd);
        //Gizmos.DrawRay(visionCone.gameObject.transform.position, rotateVector(fwd, visionCone.visionAngle));
        //Gizmos.DrawRay(visionCone.gameObject.transform.position, rotateVector(fwd, -visionCone.visionAngle));
    }
}
