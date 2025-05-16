using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
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
                Debug.Log("You Died");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
