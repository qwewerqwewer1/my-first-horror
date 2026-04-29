using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Idle Breathing")]
    public float breathSpeed = 1.0f;
    public float breathAmount = 0.003f;

    [Header("Walking Bob")]
    public float walkBobSpeed = 6f;
    public float walkBobAmountY = 0.05f;
    public float walkBobAmountX = 0.025f;

    [Header("Tilt")]
    public float tiltAmount = 1.5f;

    [Header("Smoothing")]
    public float transitionSpeed = 4f;

    private float timer = 0f;
    private Vector3 defaultPos;
    private Vector3 targetPos;
    private float currentBobY = 0f;
    private float currentBobX = 0f;

    void Start()
    {
        defaultPos = transform.localPosition;
        targetPos = defaultPos;
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

        // Наклон — при движении качается, в покое возвращается в 0
        float swayZ = isMoving ? Mathf.Sin(timer * 0.5f) * tiltAmount : 0f;
        transform.localRotation = Quaternion.Lerp(
        transform.localRotation,
        Quaternion.Euler(0f, 0f, swayZ),
        Time.deltaTime * transitionSpeed
);

        targetPos = new Vector3(
            defaultPos.x + currentBobX,
            defaultPos.y + currentBobY,
            defaultPos.z
        );

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPos,
            Time.deltaTime * transitionSpeed * 2f
        );
    }
}