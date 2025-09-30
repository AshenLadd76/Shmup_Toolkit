using UnityEngine;

public class PingPongMover : MonoBehaviour
{
    [SerializeField] private Vector3 pointA = Vector3.zero;
    [SerializeField] private Vector3 pointB = new Vector3(5f, 0f, 0f);
    [SerializeField] private float duration = 2f;

    private void Start()
    {
        // Move to pointB with ping-pong looping
        LeanTween.move(gameObject, pointB, duration)
            .setEase(LeanTweenType.linear)
            .setLoopPingPong(); // Automatically goes back to pointA and repeats
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pointA, 0.2f);
        Gizmos.DrawSphere(pointB, 0.2f);
        Gizmos.DrawLine(pointA, pointB);
    }
#endif
}