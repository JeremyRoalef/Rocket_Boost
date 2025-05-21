using Unity.Cinemachine;
using UnityEngine;

public class CameraInit : MonoBehaviour
{
    [SerializeField]
    CinemachineCamera cinemachineCamera;

    [SerializeField]
    GameObject player;

    const string NULL_PLAYER_REFERENCE_STRING = "No player found!";
    const string PLAYER_TAG_STRING = "Player";

    private void Start()
    {
        if (ObjectReference.IsNull(player, NULL_PLAYER_REFERENCE_STRING))
        {
            player = GameObject.FindGameObjectWithTag(PLAYER_TAG_STRING);
        }

        if (!ObjectReference.IsNull(player, NULL_PLAYER_REFERENCE_STRING))
        {
            Debug.Log("Hi");
            cinemachineCamera.Target.TrackingTarget = player.transform;
            cinemachineCamera.Target.LookAtTarget = player.transform;
        }
    }
}
