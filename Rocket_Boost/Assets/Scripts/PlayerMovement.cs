using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Note: These create input actions in the script itself. The action map is not being referenced here

    [SerializeField]
    [Tooltip("Input that enables the rocket thrust")]
    InputAction thrust;

    [SerializeField]
    [Tooltip("Input that enables the rocket rotation")]
    InputAction rotation;

    private void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
    }

    private void OnDisable()
    {
        thrust.Disable();
        rotation.Disable();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (thrust.IsPressed())
        {
            Debug.Log("Thrusting");
        }
        if (rotation.ReadValue<float>() < 0)
        {
            Debug.Log("A-pressed");
        }
        if (rotation.ReadValue<float>() > 0)
        {
            Debug.Log("D-pressed");
        }
    }
}
