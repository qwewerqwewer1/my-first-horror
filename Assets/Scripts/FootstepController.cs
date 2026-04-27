using UnityEngine;

public class FootstepController : MonoBehaviour
{
    public AudioClip[] footstepSounds;
    
    private AudioSource audioSource;
    private float stepTimer = 0f;
    public float stepInterval = 0.5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 || v != 0)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        audioSource.PlayOneShot(clip, Random.Range(0.8f, 1f));
    }
}