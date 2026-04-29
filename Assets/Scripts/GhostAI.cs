using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GhostAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform[] roomPoints;

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
    private float phaseTimer = 0f;
    private float wanderTimer = 0f;
    private bool flickerState = false;

    private enum State { Habitat, Hunt }
    private State state = State.Habitat;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        agent.speed = wanderSpeed;
        SetVisibility(false);
        GoToRandomRoom();
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
            Vector3 randomPoint = currentRoom.position +
                Random.insideUnitSphere * 3f;
            randomPoint.y = transform.position.y;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 3f, 1))
                agent.SetDestination(hit.position);
            wanderTimer = 0f;
        }
    }

    void HandleHunt()
    {
        if (CanSeePlayer())
        {
            agent.SetDestination(player.position);
        }
        else if (agent.remainingDistance < 0.5f)
        {
            Vector3 randomDir = Random.insideUnitSphere * 20f;
            randomDir += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDir, out hit, 20f, 1))
                agent.SetDestination(hit.position);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dir = player.position - transform.position;
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

        audioSource.loop = true;
        audioSource.clip = attackSound;
        audioSource.Play();

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
        GoToRandomRoom();

        audioSource.loop = false;
        audioSource.Stop();
        if (cryingSound != null)
            audioSource.PlayOneShot(cryingSound);

        Debug.Log("🏠 HABITAT");
    }

    void GoToRandomRoom()
    {
        if (roomPoints == null || roomPoints.Length == 0) return;
        currentRoom = roomPoints[Random.Range(0, roomPoints.Length)];
        agent.SetDestination(currentRoom.position);
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