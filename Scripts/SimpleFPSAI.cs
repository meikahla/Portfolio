using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A simple AI script for NPC behavior, including patrolling, chasing, and attacking a player.
/// </summary>
public class SimpleFPSAI : MonoBehaviour
{
    [Header("Core Components")]
    public NavMeshAgent agent;
    public Animator animator; // Optional animator for NPC animations
    public Transform player;

    [Header("Detection Settings")]
    public LayerMask isGround, isPlayer;
    public float sightRange = 20f;
    public float attackRange = 5f;
    public float fieldOfView = 45f;

    [Header("Patrolling Settings")]
    public Transform[] waypoints;
    public float waypointPauseDuration = 2f;
    private int currentWaypointIndex = 0;
    private bool isWaiting;

    [Header("Attack Settings")]
    public float timeBetweenAttacks = 1.5f;
    private bool alreadyAttacked;

    [Header("Stats")]
    public int healthPoints = 100;

    private string currentAnimationState = ""; // Tracks current animation
    private bool playerInSightRange, playerInAttackRange;

    /// <summary>
    /// Initializes core components.
    /// </summary>
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;
    }

    /// <summary>
    /// Updates the NPC's behavior (patrolling, chasing, or attacking the player).
    /// </summary>
    private void Update()
    {
        UpdateDetection();

        if (!playerInSightRange && !playerInAttackRange)
            Patrol();
        else if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        else if (playerInSightRange && playerInAttackRange)
            AttackPlayer();
    }

    /// <summary>
    /// Updates detection states to check if the player is within sight or attack range.
    /// </summary>
    private void UpdateDetection()
    {
        playerInSightRange = IsPlayerInCone(sightRange);
        playerInAttackRange = IsPlayerInCone(attackRange);
    }

    /// <summary>
    /// Checks if the player is within a detection cone based on range and field of view.
    /// </summary>
    private bool IsPlayerInCone(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, isPlayer);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = (collider.transform.position - transform.position).normalized;
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleToPlayer <= fieldOfView)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Makes the NPC patrol between waypoints if no player is detected.
    /// </summary>
    private void Patrol()
    {
        if (waypoints.Length == 0 || isWaiting) return;

        if (!agent.pathPending && agent.remainingDistance < 1f)
            StartCoroutine(PauseAtWaypoint());

        SetAnimationState("Walk");
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    /// <summary>
    /// Pauses at a waypoint for a set duration before moving to the next waypoint.
    /// </summary>
    private IEnumerator PauseAtWaypoint()
    {
        isWaiting = true;
        agent.isStopped = true;

        SetAnimationState("Idle");

        yield return new WaitForSeconds(waypointPauseDuration);

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        agent.isStopped = false;
        isWaiting = false;
    }

    /// <summary>
    /// Makes the NPC chase the player when within sight range but not in attack range.
    /// </summary>
    private void ChasePlayer()
    {
        if (!player) return;

        agent.SetDestination(player.position);
        SetAnimationState("Walk");

        if (playerInAttackRange)
            agent.isStopped = true;
    }

    /// <summary>
    /// Makes the NPC attack the player when within attack range.
    /// </summary>
    private void AttackPlayer()
    {
        if (alreadyAttacked) return;

        agent.isStopped = true;
        RotateTowards(player.position);

        SetAnimationState("Attack");

        // TODO: Add damage logic here

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    /// <summary>
    /// Rotates the NPC to face the player.
    /// </summary>
    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    /// <summary>
    /// Resets the attack state to allow the NPC to attack again.
    /// </summary>
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    /// <summary>
    /// Reduces the NPC's health when it takes damage, and destroys the NPC if health reaches zero.
    /// </summary>
    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
        if (healthPoints <= 0)
            Destroy(gameObject, 0.5f);
    }

    /// <summary>
    /// Sets the animation state of the NPC based on its current action.
    /// </summary>
    private void SetAnimationState(string newState)
    {
        if (animator == null || currentAnimationState == newState) return;

        animator.ResetTrigger(currentAnimationState);
        animator.SetTrigger(newState);
        currentAnimationState = newState;
    }

    /// <summary>
    /// Draws the detection cones for sight and attack range in the editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        DrawDetectionCone(sightRange, Color.yellow);
        DrawDetectionCone(attackRange, Color.red);
    }

    /// <summary>
    /// Draws a detection cone to visualize the NPC's sight and attack ranges.
    /// </summary>
    private void DrawDetectionCone(float range, Color color)
    {
        Gizmos.color = color;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView, 0) * transform.forward * range;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView, 0) * transform.forward * range;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
