using UnityEngine;

namespace ToolBox.Extensions
{
    public static class CameraExtensions
    {
        private static Camera cachedCam;
        private static Vector3 cachedMin;
        private static Vector3 cachedMax;
        private static float cachedPadding;

        // Explicitly cache bounds
        public static void CacheBounds(this Camera cam, float padding = 0f)
        {
            cachedCam = cam;
            cachedPadding = padding;

            Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

            cachedMin = new Vector3(min.x + padding, min.y + padding, min.z);
            cachedMax = new Vector3(max.x - padding, max.y - padding, max.z);
        }

        // Get bounds, recalculating if needed
        public static (Vector3 min, Vector3 max) GetBounds(this Camera cam, float padding = -1f)
        {
            // If padding wasn't passed, use cached padding
            if (padding < 0f)
                padding = cachedPadding;

            // Recalculate if no cache exists or camera changed
            if (cam != cachedCam || cachedCam == null)
                cam.CacheBounds(padding);

            return (cachedMin, cachedMax);
        }
    }
}