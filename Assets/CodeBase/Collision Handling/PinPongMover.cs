using Unity.VisualScripting;
using UnityEngine;

public class PingPongMover : MonoBehaviour
{
    [SerializeField] private Vector3 pointA = Vector3.zero;
    [SerializeField] private Vector3 pointB = new Vector3(5f, 0f, 0f);
    [SerializeField] private float duration = 2f;
    
    [SerializeField] private Transform _parentTransform;

    private void Awake()
    {
        _parentTransform = transform.parent;
    }
    
    private void Start()
    {
        // Move to pointB with ping-pong looping
        LeanTween.moveLocal(gameObject, pointB, duration)
            .setEase(LeanTweenType.linear)
            .setLoopPingPong(); // Automatically goes back to pointA and repeats
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!_parentTransform ) return;
        
        pointA = _parentTransform.position + pointA;
        pointB = _parentTransform.position + pointB;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pointA, 0.2f);
        Gizmos.DrawSphere(pointB, 0.2f);
        Gizmos.DrawLine(pointA, pointB);
    }
#endif
}