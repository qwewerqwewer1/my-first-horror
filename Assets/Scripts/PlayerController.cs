using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Basics Movements")] public float basicSpeed = 5f;
    public float basicSens = 2f;
    private float _mouseXRotation;

    [Header("Links")] public Transform cameraRoot;
    private Rigidbody _rigidbodyPlayer;

    void Start()
    {
        _rigidbodyPlayer = GetComponent<Rigidbody>();
        _rigidbodyPlayer.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        var ws = Input.GetAxis("Vertical");
        var ad = Input.GetAxis("Horizontal");

        var moveInput = transform.forward * ws + transform.right * ad;

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
        var mouseX = Input.GetAxis("Mouse X") * basicSens;
        var mouseY = Input.GetAxis("Mouse Y") * basicSens;

        transform.Rotate(Vector3.up * mouseX);

        _mouseXRotation -= mouseY;
        _mouseXRotation = Mathf.Clamp(_mouseXRotation, -90f, 90f);
        cameraRoot.localRotation = Quaternion.Euler(_mouseXRotation, 0f, 0f);
    }
}