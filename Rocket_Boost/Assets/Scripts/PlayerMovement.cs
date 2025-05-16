using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    //Note: These create input actions in the script itself. The action map is not being referenced here

    [SerializeField]
    [Tooltip("Input that enables the rocket thrust")]
    InputAction thrust;

    [SerializeField]
    [Tooltip("Input that enables the rocket rotation")]
    InputAction rotation;

    [SerializeField]
    float forceAmount = 10f;

    [SerializeField]
    float torqueAmount = 100f;

    Rigidbody rb;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        ThrustShip();
        RotateShip();
    }

    private void ThrustShip()
    {
        if (thrust.IsPressed())
        {
            rb.AddRelativeForce(Vector3.up * forceAmount * Time.fixedDeltaTime);
        }
    }

    private void RotateShip()
    {
        /*
        /-\    /  <- (this axis)
         |    /   <- (this axis)
         |   /    <- (this axis)
         |  /     <- (this axis)
         | /      <- (this axis)
         |/       <- (this axis)
         / - - - - - - ->
     
        Clockwise:

        /-\
         |     /
         |   _/_   <--rotation path   Spin  :D
         |  //  \  <--rotation path     2   :D
         | |/    | <--rotation path    Win! :D
         | /\   /  <--rotation path         :D
         |/  <--
         / - - - - - - ->

        Counter-Clockwise:

        /-\
         |     /
         |   _/_   <--rotation path   Win!  :D
         |  //  \  <--rotation path     2   :D
         | |/    | <--rotation path    Spin :D
         | /\   /  <--rotation path         :D
         |/  -->
         / - - - - - - ->


            -              -              - 
          /   \          /   \           /   \
        -       -      -       -       -       -       -
                  \   /           \   /           \   /
                    -              -               -
         */

        if (rotation.ReadValue<float>() < 0)
        {
            ApplyRotation(Vector3.forward, torqueAmount);
        }
        else if (rotation.ReadValue<float>() > 0)
        {
            ApplyRotation(-Vector3.forward, torqueAmount);
        }
        else
        {
        }
    }

    //Triple comment = description. Double comment = comment
    //Comment quality = Top-notch

    /// <summary>
    /// Rotation about the given vector. Pass a positive torque amount
    /// </summary>
    void ApplyRotation(Vector3 rotationDir, float torqueAmount)
    {
        torqueAmount = Mathf.Abs(torqueAmount);
        rb.AddRelativeTorque(rotationDir * torqueAmount * Time.fixedDeltaTime);
    }
}
