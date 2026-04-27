using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;
    private bool isOn = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
        }
    }
}