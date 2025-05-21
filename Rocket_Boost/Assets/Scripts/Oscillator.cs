using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [SerializeField]
    Vector3 movementVector;

    [SerializeField]
    float objectSpeed;

    [SerializeField]
    [Range(0,1)]
    float initialMoveOffset;

    Vector3 startPos;
    Vector3 endPos;
    float movementFactor;

    private void Start()
    {
        startPos = transform.position;
        endPos = startPos + movementVector;
    }

    private void Update()
    {
        movementFactor = Mathf.PingPong((Time.time + initialMoveOffset) * objectSpeed, 1);
        transform.position = Vector3.Lerp(startPos, endPos, movementFactor);
    }
}
