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

    float rotationDir;
    bool isThrusting = false;
    bool isRotating = false;

    const string NULL_RB_STRING = "Warning: No rigidbody component!";
    const string NULL_THRUST_STRING = "Warning: No thrust imput action!";
    const string NULL_ROTATION_STRING = "Warning: No rotation input action!";
    const string NULL_MAIN_THRUST_VFX_STRING = "Warning: No main thrust VFX component!";
    const string NULL_LEFT_THRUST_VFX_STRING = "Warning: No left thrust VFX component!";
    const string NULL_RIGHT_THRUST_VFX_STRING = "Warning: No right thrust VFX component!";
    const string NULL_AUDIO_SOURCE_STRING = "Warning: No audio source component!";


    /*
     * 
     * ---------------UNITY EVENTS---------------
     * 
     */

    private void OnEnable()
    {
        if (ObjectReference.IsNull(thrust, NULL_THRUST_STRING)) return;
        if (ObjectReference.IsNull(rotation, NULL_ROTATION_STRING)) return;

        thrust.Enable();
        rotation.Enable();
    }

    private void OnDisable()
    {
        if (ObjectReference.IsNull(thrust, NULL_THRUST_STRING)) return;
        if (ObjectReference.IsNull(rotation, NULL_ROTATION_STRING)) return;

        thrust.Disable();
        rotation.Disable();

        audioSource.Stop();
    }

    private void Awake()
    {
        InitializeReferences();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleAudio();
    }

    /*
     * 
     * ---------------PUBLIC METHODS---------------
     * 
     */

    public void StopAllVFX()
    {
        if (ObjectReference.IsNull(mainThrustVFX, NULL_MAIN_THRUST_VFX_STRING)) return;
        if (ObjectReference.IsNull(leftSideThrustVFX, NULL_LEFT_THRUST_VFX_STRING)) return;
        if (ObjectReference.IsNull(rightSideThrustVFX, NULL_RIGHT_THRUST_VFX_STRING)) return;

        mainThrustVFX?.Stop();
        leftSideThrustVFX.Stop();
        rightSideThrustVFX?.Stop();
    }

    /*
     * 
     * ---------------PRIVATE METHODS---------------
     * 
     */

    /// <summary>
    /// Method to handle audio playing
    /// </summary>
    private void HandleAudio()
    {
        if (ObjectReference.IsNull(audioSource, NULL_AUDIO_SOURCE_STRING)) return;

        //Conditions to return
        if (!isThrusting && !isRotating)
        {
            audioSource.Stop();
            return;
        }
        if (audioSource.isPlaying) return;

        //Logic for audio
        audioSource.Play();
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
        if (ObjectReference.IsNull(thrust, NULL_THRUST_STRING)) return;
        if (ObjectReference.IsNull(mainThrustVFX, NULL_MAIN_THRUST_VFX_STRING)) return;

        //Conditions to return
        if (!thrust.IsPressed())
        {
            mainThrustVFX.Stop();
            isThrusting = false;
            return;
        }

        //Logic for thrusting
        mainThrustVFX.Play();
        isThrusting = true;
        rb.AddRelativeForce(Vector3.up * forceAmount * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Method to rotate the ship
    /// </summary>
    private void RotateShip()
    {
        if (ObjectReference.IsNull(rotation, NULL_ROTATION_STRING)) return;
        if (ObjectReference.IsNull(leftSideThrustVFX, NULL_LEFT_THRUST_VFX_STRING)) return;
        if (ObjectReference.IsNull(rightSideThrustVFX, NULL_RIGHT_THRUST_VFX_STRING)) return;

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

        //Get input value
        rotationDir = rotation.ReadValue<float>();

        //Chech for Rotation
        if (rotationDir < 0)
        {
            rightSideThrustVFX.Play();
            isRotating = true;
            ApplyRotation(Vector3.forward, torqueAmount);
        }
        else if (rotationDir > 0)
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
        if (ObjectReference.IsNull(rb, NULL_RB_STRING)) return;

        torqueAmount = Mathf.Abs(torqueAmount);
        rb.AddRelativeTorque(rotationDir * torqueAmount * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Method to initialize the player movement references
    /// </summary>
    void InitializeReferences()
    {
        //Initialize references
        if (!TryGetComponent<Rigidbody>(out rb))
        {
            Debug.LogWarning($"Warning: No rigidbody on {gameObject.name}");
        }

        //Check if audio source is null
        if (!audioSource)
        {
            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.LogWarning($"No audio source given for {gameObject.name}");
            }
        }
    }
}
