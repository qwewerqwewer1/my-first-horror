using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Idle Breathing")]
    public float breathSpeed = 1.5f;
    public float breathAmount = 0.006f;

    [Header("Walking Bob")]
    public float walkBobSpeed = 10f;
    public float walkBobAmountY = 0.01f;
    public float walkBobAmountX = 0.005f;

    [Header("Tilt and Sway")]
    public float tiltAmount = 0.1f;
    public float swayAmount = 1f;

    [Header("Smoothing")]
    public float transitionSpeed = 12f;

    [Header("References")]
    public PlayerController playerController;

    private float timer = 0f;
    private Vector3 defaultPos;
    private float currentBobY = 0f;
    private float currentBobX = 0f;
    private float currentTiltX = 0f;
    private float currentSwayZ = 0f;

    void Start()
    {
        defaultPos = transform.localPosition;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool isMoving = h != 0 || v != 0;

        if (isMoving)
        {
            timer += Time.deltaTime * walkBobSpeed;
            currentBobY = Mathf.Lerp(currentBobY,
                Mathf.Sin(timer) * walkBobAmountY,
                Time.deltaTime * transitionSpeed);
            currentBobX = Mathf.Lerp(currentBobX,
                Mathf.Cos(timer / 2f) * walkBobAmountX,
                Time.deltaTime * transitionSpeed);
        }
        else
        {
            timer += Time.deltaTime * breathSpeed;
            currentBobY = Mathf.Lerp(currentBobY,
                Mathf.Sin(timer) * breathAmount,
                Time.deltaTime * transitionSpeed);
            currentBobX = Mathf.Lerp(currentBobX, 0f,
                Time.deltaTime * transitionSpeed);
        }

        float targetTiltX = isMoving ? Mathf.Sin(timer * 0.3f) * tiltAmount : 0f;
        float targetSwayZ = isMoving ? Mathf.Sin(timer * 0.5f) * swayAmount : 0f;

        currentTiltX = Mathf.Lerp(currentTiltX, targetTiltX, Time.deltaTime * transitionSpeed);
        currentSwayZ = Mathf.Lerp(currentSwayZ, targetSwayZ, Time.deltaTime * transitionSpeed);

        // Мышь + bob вместе
        Quaternion mouseRotation = Quaternion.Euler(playerController.verticalRotation, 0f, 0f);
        Quaternion bobRotation = Quaternion.Euler(currentTiltX, 0f, currentSwayZ);
        transform.localRotation = mouseRotation * bobRotation;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            new Vector3(defaultPos.x + currentBobX, defaultPos.y + currentBobY, defaultPos.z),
            Time.deltaTime * transitionSpeed * 2f
        );
    }
}