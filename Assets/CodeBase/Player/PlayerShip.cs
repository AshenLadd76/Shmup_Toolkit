using System.Collections.Generic;
using CodeBase.Collision_Handling;
using UnityEngine;

namespace CodeBase.Player
{
    public class PlayerShip : MonoBehaviour, ICollider
    {
        [SerializeField] private bool showDebug;
        
        [SerializeField] private Vector3 pointA;
        [SerializeField] private Vector3 pointB;
        [SerializeField] private float duration = 1f;

        [SerializeField] private Vector2 localOffset;
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public List<Vector2Int> OccupiedCells { get; set; }

        private void Awake()
        {
            Position = transform.position;
            localOffset = Vector2.one * 0.25f;
        }

      //  private void Start() => Move();

        private void Update()
        {
            if(showDebug)
                GetWorldBounds();
        }
        
        private void Move() => LeanTween.move(gameObject, pointB, duration).setEase(LeanTweenType.linear).setLoopPingPong();
        
        public (Vector2 min, Vector2 max) GetWorldBounds()
        {
            Vector2 min = Position - localOffset;
            Vector2 max = Position + localOffset;
            
            return (min, max);
        }
        
        private void OnDrawGizmos()
        {
            if (!showDebug) return;
            
            Vector2 pos = (Vector2)transform.position;
            var min = pos - localOffset;
            var max = pos + localOffset;

            Vector3 center = (Vector3)(min + max) * 0.5f;
            Vector3 size = (Vector3)(max - min);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(center, size);
        }
    }
}
