using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
    public float huntDuration = 8f;
    public float cooldownDuration = 15f;
    public float cooldownFreezeTime = 5f;

    private NavMeshAgent agent;
    private SkinnedMeshRenderer[] skinnedRenderers;
    private MeshRenderer[] meshRenderers;
    private float wanderTimer = 0f;
    private float phaseTimer = 0f;
    private bool isFrozen = false;

    private enum State { Wander, Hunt, Cooldown }
    private State state = State.Wander;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        Debug.Log($"Skinned: {skinnedRenderers.Length} Mesh: {meshRenderers.Length}");
        agent.speed = wanderSpeed;
        SetVisibility(false);
        if (agent.enabled)
        {
            SetRandomDestination();
            phaseTimer = Random.Range(10f, 20f);
        }
    }

    void Update()
    {
        if (!agent.enabled) return;
        if (isFrozen) return;

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
        StopAllCoroutines();
        SetVisibility(true);
        Debug.Log("👻 HUNT START");
    }

    void StartCooldown()
    {
        state = State.Cooldown;
        agent.speed = wanderSpeed;
        phaseTimer = cooldownDuration;
        StopAllCoroutines();
        StartCoroutine(CooldownSequence());
        Debug.Log("😴 COOLDOWN");
    }

    void StartWander()
    {
        state = State.Wander;
        StopAllCoroutines();
        SetVisibility(false);
        phaseTimer = Random.Range(10f, 20f);
        Debug.Log("🚶 WANDER");
    }

    IEnumerator CooldownSequence()
    {
        isFrozen = true;
        agent.ResetPath();
        float freezeTimer = 0f;
        while (freezeTimer < cooldownFreezeTime)
        {
            bool current = skinnedRenderers[0].enabled;
            SetVisibility(!current);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
            freezeTimer += 0.25f;
        }
        isFrozen = false;
        SetVisibility(false);
    }

    void SetVisibility(bool visible)
    {
        foreach (var r in skinnedRenderers)
            r.enabled = visible;
        foreach (var r in meshRenderers)
            r.enabled = visible;
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