using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    public float chaseDistance = 8f;
    public float wanderRadius = 5f;
    private float wanderTimer = 0f;
    public float wanderInterval = 4f;

    private enum State { Wander, Chase }
    private State state = State.Wander;

    void Start()
    {
      agent = GetComponent<NavMeshAgent>();
      if (agent.enabled)
      SetRandomDestination();
    }

   void Update()
    {
      if (!agent.enabled) return;
    
      float distToPlayer = Vector3.Distance(transform.position, player.position);

      if (distToPlayer < chaseDistance)
        state = State.Chase;
      else
        state = State.Wander;

      if (state == State.Chase)
      {
        agent.SetDestination(player.position);
      }
      else
      {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderInterval)
        {
            SetRandomDestination();
            wanderTimer = 0f;
        }
    }
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