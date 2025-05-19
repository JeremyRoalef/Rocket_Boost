using UnityEngine;

public class TriggerParticlesOnCollision : MonoBehaviour
{
    [SerializeField]
    ParticleSystem vfx;

    private void OnCollisionEnter(Collision collision)
    {
        if (!vfx.isPlaying)
        {
            vfx.Play();
        }
    }
}
