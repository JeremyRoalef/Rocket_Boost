using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField]
    AudioSource deathSource;

    [SerializeField]
    AudioClip deathClip;

    [SerializeField]
    float delayBeforeSceneReload = 3f;

    bool isDead = false;

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
                Debug.Log("You finished the level");
                break;
            case FUEL_TAG_STRING:
                Debug.Log("You collected fuel");
                break;
            default:
                if (isDead) break;
                StartCoroutine(HandleDeath());
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    IEnumerator HandleDeath()
    {
        isDead = true;
        deathSource.Stop();
        deathSource.clip = deathClip;
        deathSource.Play();

        yield return new WaitForSeconds(delayBeforeSceneReload);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
