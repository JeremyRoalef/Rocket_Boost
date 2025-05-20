using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMovement))]
public class CollisionDetection : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    [Tooltip("The script responsible for player movement")]
    PlayerMovement playerMovement;

    [SerializeField]
    [Tooltip("The audio source responsible for playing audio")]
    AudioSource collisionSource;

    [SerializeField]
    [Tooltip("The audio clip that sounds like the player dying")]
    AudioClip deathClip;

    [SerializeField]
    [Tooltip("The audio clip that sounds like a success")]
    AudioClip levelCompleteClip;

    [SerializeField]
    [Tooltip("The parent game object containing the player mesh renderer components")]
    GameObject parentRenderer;

    [SerializeField]
    ParticleSystem smokeVFX;

    [SerializeField]
    ParticleSystem extraPartsVFX;

    [SerializeField]
    InputAction levelLoader;

    [SerializeField]
    InputAction collisionDisabler;

    [Header("Settings")]
    [SerializeField]
    [Tooltip("The delay between finishing the current level and loading the next level")]
    float delayBeforeSceneReload = 3f;

    [SerializeField]
    [Tooltip("The minimum force that can be applied to the renderer game objects")]
    float minRendererForce = -10;

    [SerializeField]
    [Tooltip("The maximum force that can be applied to the renderer game objects")]
    float maxRendererForce = 10;


    bool isDead = false;
    bool hasFinishedLevel = false;

    //debugging attributes
    bool isCollidable = true;
    float levelLoaderTimeout = 1f;

    //Tag strings
    const string FRIENDLY_TAG_STRING = "Friendly";
    const string FINISH_TAG_STRING = "Finish";
    const string FUEL_TAG_STRING = "Fuel";

    const string NULL_PLAYER_MOVEMENT_STRING = "Warning: No player movement script given!";
    const string NULL_COLLISION_SOURCE_STRING = "Warning: No collision source given!";
    const string NULL_EXTRA_PARTS_VFX_STRING = "Warning: No extra parts VFX given!";
    const string NULL_SMOKE_VFX_STRING = "Warning: No smoke VFX given!";
    const string NULL_LEVEL_LOADER_INPUT_ACTION_STRING = "Warning: no level loader input action!";
    const string NULL_COLLIDABLE_INPUT_ACTION_STRING = "Warning: no collision disabler action!";

    /*
     * 
     * ---------------UNITY EVENTS---------------
     * 
     */

    private void OnEnable()
    {
        if (ObjectReference.IsNull(levelLoader, NULL_LEVEL_LOADER_INPUT_ACTION_STRING)) return;
        if (ObjectReference.IsNull(collisionDisabler, NULL_COLLIDABLE_INPUT_ACTION_STRING)) return;


        levelLoader.Enable();
        collisionDisabler.Enable();
    }

    private void OnDisable()
    {
        if (ObjectReference.IsNull(levelLoader, NULL_LEVEL_LOADER_INPUT_ACTION_STRING)) return;
        if (ObjectReference.IsNull(collisionDisabler, NULL_COLLIDABLE_INPUT_ACTION_STRING)) return;

        levelLoader.Disable();
        collisionDisabler.Disable();
    }

    private void Awake()
    {
        InitializeReferences();
    }

    private void Update()
    {
        RespondToDebugKeys();
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case FRIENDLY_TAG_STRING:
                Debug.Log("You hit a friendly object");
                break;
            case FINISH_TAG_STRING:
                HandleLevelComplete();
                break;
            default:
                HandleDeath();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case FUEL_TAG_STRING:
                HandleFuelGain(other);
                break;
            default:
                Debug.Log("TRIGGERED!");
                break;
        }
    }

    /*
     * 
     * ---------------PRIVATE METHODS---------------
     * 
     */

    /// <summary>
    /// Method to handle level completion logic (i.e., audio playing, scene loading, etc.)
    /// </summary>
    private void HandleLevelComplete()
    {
        //Invalid Checks
        if (ObjectReference.IsNull(collisionSource, NULL_COLLISION_SOURCE_STRING)) return;
        if (isDead) return;
        if (hasFinishedLevel) return;
        if (!isCollidable) return;

        //Audio
        collisionSource.Stop();
        collisionSource.clip = levelCompleteClip;
        collisionSource.Play();

        //Logic
        hasFinishedLevel = true;
        StartCoroutine(LoadNextLevel(delayBeforeSceneReload));
    }

    /// <summary>
    /// Method to handle death logic (i.e., particle systems, scene loading, audio, etc.)
    /// </summary>
    void HandleDeath()
    {
        //Invalid Checks
        if (ObjectReference.IsNull(collisionSource, NULL_COLLISION_SOURCE_STRING)) return;
        if (ObjectReference.IsNull(extraPartsVFX, NULL_EXTRA_PARTS_VFX_STRING)) return;
        if (ObjectReference.IsNull(playerMovement, NULL_PLAYER_MOVEMENT_STRING)) return;
        if (ObjectReference.IsNull(smokeVFX, NULL_SMOKE_VFX_STRING)) return;
        if (hasFinishedLevel) return;
        if (isDead) return;
        if (!isCollidable) return;

        //Audio
        collisionSource.Stop();
        collisionSource.clip = deathClip;
        collisionSource.Play();

        //Logic
        extraPartsVFX.Play();
        playerMovement.StopAllVFX();
        smokeVFX.Play();
        isDead = true;
        playerMovement.enabled = false;
        DeparentRenderer();
        StartCoroutine(LoadCurrentLevel(delayBeforeSceneReload));
    }

    /// <summary>
    /// Method to deparent and apply a random force to the renderer game objects on the player.
    /// </summary>
    private void DeparentRenderer()
    {
        Rigidbody childRB;
        foreach (Transform child in parentRenderer.transform)
        {
            //No more parent
            child.SetParent(null);

            //Enable collision
            child.GetComponent<BoxCollider>().enabled = true;

            //Apply random force
            childRB = child.AddComponent<Rigidbody>();
            childRB.AddForce(new Vector3(
                Random.Range(minRendererForce, maxRendererForce), 
                Random.Range(minRendererForce, maxRendererForce),
                Random.Range(minRendererForce, maxRendererForce)
                ));
        }
    }

    /// <summary>
    /// Method to handle fuel collection
    /// </summary>
    /// <param name="other">The collider of the object tagged 'fuel'</param>
    private void HandleFuelGain(Collider other)
    {
        Debug.Log("Fuel Picked Up");
        Destroy(other.gameObject);
    }

    /// <summary>
    /// Method to initialize the references for collision detection
    /// </summary>
    private void InitializeReferences()
    {
        //Check for player movement script
        if (playerMovement == null)
        {
            if (!TryGetComponent<PlayerMovement>(out playerMovement))
            {
                Debug.LogWarning("No player movement script found");
            }
        }

        //Check for audio source component
        if (collisionSource == null)
        {
            if (!TryGetComponent<AudioSource>(out collisionSource))
            {
                Debug.LogWarning("No audio source component found");
            }
        }

        //Check for audio clips
        if (deathClip == null)
        {
            Debug.LogWarning("No audio clip found");
        }

        //Check for audio clips
        if (levelCompleteClip == null)
        {
            Debug.LogWarning("No audio clip found");
        }

        //Check for player movement script
        if (parentRenderer == null)
        {
            Debug.LogWarning("No parent renderer game object");
        }

        if (smokeVFX == null)
        {
            if (!TryGetComponent<ParticleSystem>(out smokeVFX))
            {
                Debug.LogWarning("No particle system component found");
            }
        }

        if (extraPartsVFX == null)
        {
            if (!TryGetComponent<ParticleSystem>(out extraPartsVFX))
            {
                Debug.LogWarning("No particle system component found");
            }
        }
    }

    /// <summary>
    /// Debugging logic for dev use only
    /// </summary>
    private void RespondToDebugKeys()
    {
        if (ObjectReference.IsNull(levelLoader, NULL_LEVEL_LOADER_INPUT_ACTION_STRING)) return;
        if (ObjectReference.IsNull(collisionDisabler, NULL_COLLIDABLE_INPUT_ACTION_STRING)) return;

        //Control collision
        if (collisionDisabler.WasPressedThisFrame())
        {
            isCollidable = !isCollidable;
        }

        //Handling button held issues
        if (levelLoaderTimeout > 0)
        {
            levelLoaderTimeout -= Time.deltaTime;
        }
        //Load next level
        else if (levelLoader.WasPressedThisFrame())
        {
            StartCoroutine(LoadNextLevel());
        }
    }

    /// <summary>
    /// Coroutine to load the next level after delay
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadNextLevel(float delay = 0)
    {
        //Get next level indes
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevelIndex > SceneManager.sceneCount)
        {
            nextLevelIndex = 0;
        }

        //Load next level
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextLevelIndex);
    }

    /// <summary>
    /// Coroutine to load next level after delay
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadCurrentLevel(float delay = 0)
    {
        //Load current level
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
