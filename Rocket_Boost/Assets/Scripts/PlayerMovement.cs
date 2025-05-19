using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    //Note: These create input actions in the script itself. The action map is not being referenced here

    [Header("Input Actions")]
    [SerializeField]
    [Tooltip("Input that enables the rocket thrust")]
    InputAction thrust;

    [SerializeField]
    [Tooltip("Input that enables the rocket rotation")]
    InputAction rotation;

    [Header("Local References")]
    [SerializeField]
    [Tooltip("The audio source responsible for playing movement sounds")]
    AudioSource audioSource;

    [Header("Settings")]
    [SerializeField]
    [Tooltip("The amount of force that will move the player up")]
    float forceAmount = 10f;

    [SerializeField]
    [Tooltip("The amount of torque that the player will rotate")]
    float torqueAmount = 100f;

    [SerializeField]
    ParticleSystem mainThrustVFX;

    [SerializeField]
    ParticleSystem leftSideThrustVFX;

    [SerializeField]
    ParticleSystem rightSideThrustVFX;

    Rigidbody rb;


    bool isThrusting = false;
    bool isRotating = false;

    private void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
    }

    private void OnDisable()
    {
        thrust.Disable();
        rotation.Disable();

        audioSource.Stop();
    }

    private void Awake()
    {
        //Initialize references
        rb = GetComponent<Rigidbody>();
        if (audioSource == null)
        {
            if(!TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.LogWarning($"No audio source given for {gameObject.name}");
            }
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleAudio();
    }

    /// <summary>
    /// Method to handle audio playing
    /// </summary>
    private void HandleAudio()
    {
        //Check condition logic
        if (isThrusting || isRotating)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        //Do not play audio
        else
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Method to handle the player movement logic
    /// </summary>
    private void HandleMovement()
    {
        ThrustShip();
        RotateShip();
    }

    /// <summary>
    /// Method to thrust the ship upwards
    /// </summary>
    private void ThrustShip()
    {
        //Check for thrusing
        if (thrust.IsPressed())
        {
            mainThrustVFX.Play();
            isThrusting = true;
            rb.AddRelativeForce(Vector3.up * forceAmount * Time.fixedDeltaTime);
        }
        //No thrusting
        else
        {
            mainThrustVFX.Stop();
            isThrusting = false;
        }
    }

    /// <summary>
    /// Method to rotate the ship
    /// </summary>
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

        //Chech for Rotation
        if (rotation.ReadValue<float>() < 0)
        {
            rightSideThrustVFX.Play();
            isRotating = true;
            ApplyRotation(Vector3.forward, torqueAmount);
        }
        else if (rotation.ReadValue<float>() > 0)
        {
            leftSideThrustVFX.Play();
            isRotating = true;
            ApplyRotation(-Vector3.forward, torqueAmount);
        }
        //No rotation
        else
        {
            leftSideThrustVFX.Stop();
            rightSideThrustVFX.Stop();
            isRotating = false;
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

    public void StopAllVFX()
    {
        mainThrustVFX?.Stop();
        leftSideThrustVFX.Stop();
        rightSideThrustVFX?.Stop();
    }
}
