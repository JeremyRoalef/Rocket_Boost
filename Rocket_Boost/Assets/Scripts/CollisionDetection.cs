using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField]
    AudioSource collisionSource;

    [SerializeField]
    AudioClip deathClip;

    [SerializeField]
    AudioClip levelCompleteClip;

    [SerializeField]
    float delayBeforeSceneReload = 3f;

    bool isDead = false;
    bool hasFinishedLevel = false;

    //Tag strings
    const string FRIENDLY_TAG_STRING = "Friendly";
    const string FINISH_TAG_STRING = "Finish";
    const string FUEL_TAG_STRING = "Fuel";

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
            case FUEL_TAG_STRING:
                Debug.Log("You collected fuel");
                break;
            default:
                HandleDeath();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void HandleLevelComplete()
    {
        //Invalid Checks
        if (isDead) return;
        if (hasFinishedLevel) return;

        //Audio
        collisionSource.Stop();
        collisionSource.clip = levelCompleteClip;
        collisionSource.Play();

        //Logic
        hasFinishedLevel = true;
        StartCoroutine(LoadNextLevel());
    }

    void HandleDeath()
    {
        //Invalid Checks
        if (hasFinishedLevel) return;
        if (isDead) return;

        //Audio
        collisionSource.Stop();
        collisionSource.clip = deathClip;
        collisionSource.Play();

        //Logic
        isDead = true;
        StartCoroutine(LoadCurrentLevel());
    }

    IEnumerator LoadNextLevel()
    {
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevelIndex > SceneManager.sceneCount)
        {
            nextLevelIndex = 0;
        }

        yield return new WaitForSeconds(delayBeforeSceneReload);
        SceneManager.LoadScene(nextLevelIndex);
    }

    IEnumerator LoadCurrentLevel()
    {
        yield return new WaitForSeconds(delayBeforeSceneReload);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
