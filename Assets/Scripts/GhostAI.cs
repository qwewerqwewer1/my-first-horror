using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    [Header("References")] 
    public Transform target;

    [Header("Movement")]
    public float wanderSpeed = 1.5f;
    public float huntSpeed = 3f;

    [Header("Phases")]
    public float habitatDuration = 300f;
    public float huntDuration = 30f;

    [Header("Vision")]
    public float visionRange = 8f;
    public LayerMask wallMask;

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip cryingSound;

    private NavMeshAgent agent;
    private SkinnedMeshRenderer[] skinnedRenderers;
    private AudioSource audioSource;
    private Transform currentRoom;
    private float phaseTimer;
    private float wanderTimer;
    private bool flickerState;

    private enum State { Habitat, Hunt }

    private State state = State.Habitat;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        agent.speed = wanderSpeed;
        SetVisibility(false);
        phaseTimer = habitatDuration;
    }

    void Update()
    {
        if (!agent.enabled) return;

        phaseTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Habitat:
                HandleHabitat();
                if (phaseTimer <= 0f)
                    StartHunt();
                break;

            case State.Hunt:
                HandleHunt();
                if (phaseTimer <= 0f)
                    StartHabitat();
                break;
        }
    }

   

    void HandleHabitat()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= 8f || agent.remainingDistance < 0.5f)
        {
            var randomPoint = currentRoom.position +
                              Random.insideUnitSphere * 3f;
            randomPoint.y = transform.position.y;
            if (NavMesh.SamplePosition(randomPoint, out var hit, 3f, 1))
                agent.SetDestination(hit.position);
            wanderTimer = 0f;
        }
    }

    void HandleHunt()
    {
        if (CanSeePlayer())
        {
            agent.SetDestination(target.position);
        }
        else if (agent.remainingDistance < 0.5f)
        {
            Vector3 randomDir = Random.insideUnitSphere * 20f;
            randomDir += transform.position;
            if (NavMesh.SamplePosition(randomDir, out var hit, 20f, 1))
                agent.SetDestination(hit.position);
        }
    }

    bool CanSeePlayer()
    {
        var dir = target.position - transform.position;
        float dist = dir.magnitude;
        if (dist > visionRange) return false;
        if (Physics.Raycast(transform.position + Vector3.up,
            dir.normalized, dist, wallMask))
            return false;
        return true;
    }

    void StartHunt()
    {
        state = State.Hunt;
        agent.speed = huntSpeed;
        phaseTimer = huntDuration;
        wanderTimer = 0f;
        agent.ResetPath();
        StopAllCoroutines();
        StartCoroutine(FlickerLoop());

        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.clip = attackSound;
            audioSource.Play();
        }

        Debug.Log("👻 HUNT");
    }

    void StartHabitat()
    {
        state = State.Habitat;
        agent.speed = wanderSpeed;
        phaseTimer = habitatDuration;
        wanderTimer = 0f;
        StopAllCoroutines();
        SetVisibility(false);

        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
            if (cryingSound != null)
                audioSource.PlayOneShot(cryingSound);
        }

        Debug.Log("🏠 HABITAT");
    }

    IEnumerator FlickerLoop()
    {
        while (state == State.Hunt)
        {
            flickerState = !flickerState;
            SetVisibility(flickerState);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.6f));
        }
        SetVisibility(false);
    }

    

    void SetVisibility(bool visible)
    {
        foreach (var r in skinnedRenderers)
            r.enabled = visible;
    }

    public bool IsHunting()
    {
        return state == State.Hunt;
    }
}