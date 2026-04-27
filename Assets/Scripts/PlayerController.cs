using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float mouseSensitivity = 2f;

    [Header("References")]
    public Transform cameraHolder;

    private Rigidbody rb;
    private float verticalRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
    }

    if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        return;
    }

    if (Cursor.lockState == CursorLockMode.Locked)
        HandleMouseLook();

    HandleFootstepAudio();
}

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = (transform.forward * v + transform.right * h).normalized;
        Vector3 velocity = dir * moveSpeed;
        velocity.y = rb.linearVelocity.y; // гравитация сохраняется

        rb.linearVelocity = velocity;
    }

    void HandleFootstepAudio()
    {
        AudioSource audio = GetComponent<AudioSource>();
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if ((h != 0 || v != 0) && !audio.isPlaying)
            audio.Play();
        else if (h == 0 && v == 0 && audio.isPlaying)
            audio.Stop();
    }
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Тело поворачивается по горизонтали
        transform.Rotate(0f, mouseX, 0f);

        // Камера смотрит вверх/вниз
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}