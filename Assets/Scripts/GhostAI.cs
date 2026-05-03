using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    [Header("References")] public Transform target;
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

    private NavMeshAgent _agent;
    private SkinnedMeshRenderer[] _skinnedRenderers;
    private AudioSource _audioSource;
    private Transform _currentRoom;
    private float _phaseTimer;
    private float _wanderTimer;
    private bool _flickerState;

    private enum State { Habitat, Hunt }

    private State _state = State.Habitat;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _agent.speed = wanderSpeed;
        SetVisibility(false);
        GoToRandomRoom();
        _phaseTimer = habitatDuration;
    }

    void Update()
    {
        if (!_agent.enabled) return;

        _phaseTimer -= Time.deltaTime;

        switch (_state)
        {
            case State.Habitat:
                HandleHabitat();
                if (_phaseTimer <= 0f)
                    StartHunt();
                break;

            case State.Hunt:
                HandleHunt();
                if (_phaseTimer <= 0f)
                    StartHabitat();
                break;
        }
    }

   

    void HandleHabitat()
    {
        _wanderTimer += Time.deltaTime;
        if (_wanderTimer >= 8f || _agent.remainingDistance < 0.5f)
        {
            var randomPoint = _currentRoom.position +
                              Random.insideUnitSphere * 3f;
            randomPoint.y = transform.position.y;
            if (NavMesh.SamplePosition(randomPoint, out var hit, 3f, 1))
                _agent.SetDestination(hit.position);
            _wanderTimer = 0f;
        }
    }

    void HandleHunt()
    {
        if (CanSeePlayer())
        {
            _agent.SetDestination(target.position);
        }
        else if (_agent.remainingDistance < 0.5f)
        {
            Vector3 randomDir = Random.insideUnitSphere * 20f;
            randomDir += transform.position;
            if (NavMesh.SamplePosition(randomDir, out var hit, 20f, 1))
                _agent.SetDestination(hit.position);
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
        _state = State.Hunt;
        _agent.speed = huntSpeed;
        _phaseTimer = huntDuration;
        _wanderTimer = 0f;
        _agent.ResetPath();
        StopAllCoroutines();
        StartCoroutine(FlickerLoop());

        if (_audioSource != null)
        {
            _audioSource.loop = true;
            _audioSource.clip = attackSound;
            _audioSource.Play();
        }

        Debug.Log("👻 HUNT");
    }

    void StartHabitat()
    {
        _state = State.Habitat;
        _agent.speed = wanderSpeed;
        _phaseTimer = habitatDuration;
        _wanderTimer = 0f;
        StopAllCoroutines();
        SetVisibility(false);
        GoToRandomRoom();

        if (_audioSource != null)
        {
            _audioSource.loop = false;
            _audioSource.Stop();
            if (cryingSound != null)
                _audioSource.PlayOneShot(cryingSound);
        }

        Debug.Log("🏠 HABITAT");
    }

    void GoToRandomRoom()
    {
        if (roomPoints == null || roomPoints.Length == 0) return;
        _currentRoom = roomPoints[Random.Range(0, roomPoints.Length)];
        _agent.SetDestination(_currentRoom.position);
        _wanderTimer = 0f;
    }

    IEnumerator FlickerLoop()
    {
        while (_state == State.Hunt)
        {
            _flickerState = !_flickerState;
            SetVisibility(_flickerState);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.6f));
        }
        SetVisibility(false);
    }

    

    void SetVisibility(bool visible)
    {
        foreach (var r in _skinnedRenderers)
            r.enabled = visible;
    }

    public bool IsHunting()
    {
        return _state == State.Hunt;
    }
}