using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public static class CollisionAlgorithms
    {
        public static bool CircleVsCircle(Vector2 posA, float radiusA, Vector2 posB, float radiusB)
        {
            Vector2 difference = posA - posB;
            float sqrDistance = difference.sqrMagnitude;
            float sqrRadii = (radiusA + radiusB) * (radiusA + radiusB);
            return sqrDistance <= sqrRadii;
        }

        public static bool LineIntersectsCircle(Vector2 a, Vector2 b, Vector2 center, float radius)
        {
            Vector2 ab = b - a;
            Vector2 ac = center - a;
            float t = Mathf.Clamp01(Vector2.Dot(ac, ab) / ab.sqrMagnitude);
            Vector2 closest = a + t * ab;
            return (center - closest).sqrMagnitude <= radius * radius;
        }
    }
}