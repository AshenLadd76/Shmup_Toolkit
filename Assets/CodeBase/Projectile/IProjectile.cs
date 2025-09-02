using UnityEngine;

namespace CodeBase.Projectile
{
    public interface IProjectile
    {
        public bool IsActive { get; set; }

        public float LifeSpan { get; set; }

        public float Speed { get; set; }
        public Vector3 Velocity { get; set; }
        
        public float Radius { get; set; }
        
        public Vector3 LastPosition { get; set; }
        
        public Vector2Int LastCellPosition { get; set; }

        public Vector3 GetPosition();
        
        public void SetPosition(Vector3 position);

        public void SetDirection(Vector3 direction);

        public void SetRotation(Quaternion rotation);

        public void SetColour(Color color);
    }
}
