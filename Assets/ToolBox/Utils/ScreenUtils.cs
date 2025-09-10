namespace ToolBox.Utils
{
    using UnityEngine;

    public static class ScreenUtils
    {
        /// <summary>
        /// Clamps a position to the given screen bounds.
        /// </summary>
        /// <param name="position">The position to clamp.</param>
        /// <param name="bounds">The bounds to clamp against.</param>
        /// <returns>The clamped position.</returns>
        public static Vector3 ClampPositionToScreenBounds(Vector3 position, Bounds bounds)
        {
            position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
            position.y = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
            return position;
        }
    }

}
