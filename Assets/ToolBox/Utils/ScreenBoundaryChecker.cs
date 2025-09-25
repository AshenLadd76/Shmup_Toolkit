using UnityEngine;

namespace ToolBox.Utils
{
    public static class ScreenBoundaryChecker
    {
        private static Camera _cam;
        private static Vector3 _minBounds;
        private static Vector3 _maxBounds;

        private static Transform _cachedCameraTransform;
        
        private static Vector3 _lastCameraPosition;
        
        /// <summary>
        /// Gets the world-space minimum bounds (bottom-left).
        /// </summary>
        public static Vector3 Min => _minBounds;

        /// <summary>
        /// Gets the world-space maximum bounds (top-right).
        /// </summary>
        public static Vector3 Max => _maxBounds;

        private static uint _rangeX;
        private static uint _rangeY;

        private static float _screenWidth;
        private static float _screenHeight;

        public static void Initialize()
        {
            _cam = Camera.main;
            
            if (_cam == null)
            {
                Logger.LogError("No Main Camera found! Assign a camera with the 'MainCamera' tag.");
                return;
            }

            // For orthographic cameras (shmups usually are)
            if (!_cam.orthographic)
            {
                // For perspective cameras, you’d use frustum corners
                Logger.LogWarning("Perspective camera not fully supported in this method.");
                return;
            }

            _cachedCameraTransform = _cam.transform;

            CalculateScreenDimensions();
            
            CalculateBounds( );
        }

        private static void CalculateScreenDimensions()
        {
            _screenHeight = _cam.orthographicSize * 2f;
            _screenWidth = _screenHeight * _cam.aspect;
        }

        public static void CalculateBounds(  )
        {
            Vector3 camPos = _cachedCameraTransform.position;
            
            if(camPos == _lastCameraPosition) return;
            
            _minBounds = new Vector3(camPos.x - _screenWidth / 2f, camPos.y - _screenHeight / 2f, camPos.z);
            _maxBounds = new Vector3(camPos.x + _screenWidth / 2f, camPos.y + _screenHeight / 2f, camPos.z);
            
            _rangeX = (uint)(_maxBounds.x - _minBounds.x); 
            _rangeY = (uint)(_maxBounds.y - _minBounds.y);
            
            
            //Logger.Log( $"{_minBounds.x}, {_minBounds.y}, {_maxBounds.x}, {_maxBounds.y}" );

            _lastCameraPosition = camPos;
        }


        /// <summary>
        // Checks if a position is outside the screen bounds.
        // Unsigned integers (uint) in C# are 32-bit numbers that cannot be negative. When you subtract and cast a float to uint, the comparison can be done without branching because the CPU can treat it as a single check:
        //return (uint)(pos.x - minX) > (uint)(maxX - minX);
        //Here’s what’s happening:
        //pos.x - minX → distance from minimum bound.
        // Cast to uint → anything negative becomes a very large positive number (overflow wraps around).
       // Compare with (uint)(maxX - minX) → if the position was below minX, the subtraction will be negative, wrapping to a huge number, and the comparison will automatically be true.
       // If the position is inside bounds, the subtraction is positive and smaller than (maxX - minX), comparison returns false.
       // ✅ No if branching is needed — just arithmetic and a comparison. The CPU executes it in one shot.
       /// </summary>
        // public static bool IsOutsideBounds(Vector3 position)
        // {
        //     // Distance from min bounds
        //     uint dx = (uint)(position.x - _minBounds.x);
        //     uint dy = (uint)(position.y - _minBounds.y);
        //     
        //     // Bitwise OR of the comparisons: 0 if both inside, non-zero if outside
        //     return (dx > _rangeX | dy > _rangeY);
        // }
       
   
        public static bool IsOutsideBounds(Vector3 position) => position.x < _minBounds.x || position.x > _maxBounds.x || position.y < _minBounds.y || position.y > _maxBounds.y;
        
    }
}


