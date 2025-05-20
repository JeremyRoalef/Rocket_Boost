using UnityEngine;

public class TriggerParticlesOnCollision : MonoBehaviour
{
    [SerializeField]
    ParticleSystem vfx;

    const string NULL_VFX_STRING = "Warning: No particle system component!";

    private void Awake()
    {
        InitializeReferences();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Conditions to return
        if (ObjectReference.IsNull(vfx, NULL_VFX_STRING)) return;
        if (vfx.isPlaying) return;

        //Collision logic
        vfx.Play();
    }

    /// <summary>
    /// Method to initialize the references for this script
    /// </summary>
    private void InitializeReferences()
    {
        if (ObjectReference.IsNull(vfx, NULL_VFX_STRING))
        {
            vfx = GetComponent<ParticleSystem>();
            ObjectReference.IsNull(vfx, NULL_VFX_STRING);
        }
    }
}
