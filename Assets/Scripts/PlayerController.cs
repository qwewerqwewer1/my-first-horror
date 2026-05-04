using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Basics Movements")] 
    public float basicSpeed = 5f;
    public float basicSens = 2f;

    [Header("Links")] 
    public Transform cameraRoot;
    public AudioSource audioSource;

    private float _ws;
    private float _ad;
    private float _mouseXRotation;
    private Rigidbody _rigidbodyPlayer;
    
    private void Start()
    {
        _rigidbodyPlayer = GetComponent<Rigidbody>();
        _rigidbodyPlayer.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        audioSource.Stop();
    }

    private void Update()
    {
        // ------------- ЗВУК ---------------------------------
        bool isMoving = _rigidbodyPlayer.linearVelocity.magnitude > 0.1f;
        
        if (isMoving && !audioSource.isPlaying)
            audioSource.Play();
        else if (!isMoving)
            audioSource.Stop();
    }

    private void FixedUpdate()
    {
        // ------------- Движение Игрока! ---------------------------------
        _ws = Input.GetAxis("Vertical");
        _ad = Input.GetAxis("Horizontal");
        var moveInput = transform.forward * _ws + transform.right * _ad;

        if (moveInput.magnitude > 1f) moveInput.Normalize();
        
        _rigidbodyPlayer.linearVelocity = new Vector3(
            moveInput.x * basicSpeed,
            _rigidbodyPlayer.linearVelocity.y,
            moveInput.z * basicSpeed
        );

        _rigidbodyPlayer.angularVelocity = Vector3.zero;
    }

    private void LateUpdate()
    {
        // --------------МЫШЬ---------------------------------
        // Мышь
        var mouseX = Input.GetAxis("Mouse X") * basicSens;
        var mouseY = Input.GetAxis("Mouse Y") * basicSens;

        // Шампур для башки
        transform.Rotate(Vector3.up * mouseX);

        // Инверсия поворота (МЫ НЕ В ВЕРТОЛ ЕТЕ)
        _mouseXRotation -= mouseY;
        // Лочим
        _mouseXRotation = Mathf.Clamp(_mouseXRotation, -90f, 90f);
        // Применяем
        cameraRoot.localRotation = Quaternion.Euler(_mouseXRotation, 0f, 0f);
    }
}