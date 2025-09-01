using UnityEngine;

namespace ToolBox.Extensions
{
    public static class VectorExtensions
    {
        // Vector3 → Vector3Int
        public static Vector3Int ToVector3Int(this Vector3 v)
        {
            return new Vector3Int(
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.y),
                Mathf.RoundToInt(v.z)
            );
        }

        public static Vector3Int ToVector3IntFloor(this Vector3 v)
        {
            return new Vector3Int(
                Mathf.FloorToInt(v.x),
                Mathf.FloorToInt(v.y),
                Mathf.FloorToInt(v.z)
            );
        }

        public static Vector3Int ToVector3IntCeil(this Vector3 v)
        {
            return new Vector3Int(
                Mathf.CeilToInt(v.x),
                Mathf.CeilToInt(v.y),
                Mathf.CeilToInt(v.z)
            );
        }

        // Vector3 → Vector2Int (drops z)
        public static Vector2Int ToVector2Int(this Vector3 v)
        {
            return new Vector2Int(
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.y)
            );
        }

        public static Vector2Int ToVector2IntFloor(this Vector3 v)
        {
            return new Vector2Int(
                Mathf.FloorToInt(v.x),
                Mathf.FloorToInt(v.y)
            );
        }

        public static Vector2Int ToVector2IntCeil(this Vector3 v)
        {
            return new Vector2Int(
                Mathf.CeilToInt(v.x),
                Mathf.CeilToInt(v.y)
            );
        }
    }
}