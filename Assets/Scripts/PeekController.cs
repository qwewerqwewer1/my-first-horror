using UnityEngine;

public class PeekController : MonoBehaviour
{
    [Header("Peek Settings")]
    public float peekDistance = 0.5f;
    public float peekAngle = 15f;
    public float peekSpeed = 6f;

    private Vector3 defaultPos;
    private float targetPeekX = 0f;
    private float targetPeekZ = 0f;
    private float currentPeekX = 0f;
    private float currentPeekZ = 0f;

    void Start()
    {
        defaultPos = transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            targetPeekX = -peekDistance;
            targetPeekZ = peekAngle;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            targetPeekX = peekDistance;
            targetPeekZ = -peekAngle;
        }
        else
        {
            targetPeekX = 0f;
            targetPeekZ = 0f;
        }

        currentPeekX = Mathf.Lerp(currentPeekX, targetPeekX, Time.deltaTime * peekSpeed);
        currentPeekZ = Mathf.Lerp(currentPeekZ, targetPeekZ, Time.deltaTime * peekSpeed);

        transform.localPosition = new Vector3(
            defaultPos.x + currentPeekX,
            defaultPos.y,
            defaultPos.z
        );
    }
}