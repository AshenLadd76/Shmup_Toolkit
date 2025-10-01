using UnityEngine;

namespace CodeBase.Player
{
    public interface IScreenClamper
    {
        Vector3 ClampPositionToScreenBounds(Vector3 position, (Vector3 min, Vector3 max) bounds);
        Vector3 CalculateWorldPosition( Vector3 delta );
        Vector3 CalculateLocalPosition(Vector3 worldPosition);
    }
}