using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    private Light light;

    void Start()
    {
        light = GetComponent<Light>();
        light.enabled = false;
    }

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
            light.enabled = !light.enabled;
    }
}