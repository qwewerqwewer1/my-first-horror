using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Movement")]
    public float wanderSpeed = 1.5f;
    public float huntSpeed = 4f;

    [Header("Wander")]
    public float wanderRadius = 5f;
    public float wanderInterval = 4f;

    [Header("Hunt")]
    public float huntDuration = 8f;    // секунд охотится
    public float cooldownDuration = 15f; // секунд отдыхает

    private NavMeshAgent agent;
    private float wanderTimer = 0f;
    private float phaseTimer = 0f;

    private enum State { Wander, Hunt, Cooldown }
    private State state = State.Wander;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = wanderSpeed;
        if (agent.enabled)
        {
            SetRandomDestination();
            phaseTimer = Random.Range(10f, 20f); // первая охота через 10-20 сек
        }
    }

    void Update()
    {
        if (!agent.enabled) return;

        phaseTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Wander:
                HandleWander();
                if (phaseTimer <= 0f)
                    StartHunt();
                break;

            case State.Hunt:
                agent.SetDestination(player.position);
                if (phaseTimer <= 0f)
                    StartCooldown();
                break;

            case State.Cooldown:
                HandleWander();
                if (phaseTimer <= 0f)
                    StartWander();
                break;
        }
    }

    void HandleWander()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderInterval)
        {
            SetRandomDestination();
            wanderTimer = 0f;
        }
    }

    void StartHunt()
    {
        state = State.Hunt;
        agent.speed = huntSpeed;
        phaseTimer = huntDuration;
        Debug.Log("👻 HUNT START");
    }

    void StartCooldown()
    {
        state = State.Cooldown;
        agent.speed = wanderSpeed;
        phaseTimer = cooldownDuration;
        Debug.Log("😴 COOLDOWN");
    }

    void StartWander()
    {
        state = State.Wander;
        phaseTimer = Random.Range(10f, 20f);
        Debug.Log("🚶 WANDER");
    }

    void SetRandomDestination()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, 1))
            agent.SetDestination(hit.position);
    }
}