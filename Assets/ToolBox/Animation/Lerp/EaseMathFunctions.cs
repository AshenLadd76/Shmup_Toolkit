using UnityEngine;

namespace ToolBox.Animation.Lerp
{
    public static class EaseMathFunctions
    {
        public static float SmoothStep(float t)
        {
            return Mathf.SmoothStep(0,1,t);
        }

        #region Sine

        // Ease In Sine
        public static float EaseInSine(float t)
        {
            return 1f - Mathf.Cos((t * Mathf.PI) * 0.5f);
        }

        // Ease Out Sine
        public static float EaseOutSine(float t)
        {
            return Mathf.Sin((t * Mathf.PI) * 0.5f);
        }

        // Ease InOut Sine
        public static float EaseInOutSine(float t)
        {
            return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
        }

        #endregion
        
        
        #region Cubic

        // Ease In Cubic
        public static float EaseInCubic(float t)
        {
            return t * t * t;
        }

        // Ease Out Cubic
        public static float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        // Ease InOut Cubic
        public static float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
        }

        #endregion

        
        #region Quad

        //Ease in Quad
        public static float EaseInQuad(float t)
        {
            return t * t;
        }

        // Ease Out Quad
        public static float EaseOutQuad(float t)
        {
            return t * (2f - t);
        }

        // Ease InOut Quad
        public static float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
        }

        #endregion
        
        
        #region Quart

        // Ease In Quart
        public static float EaseInQuart(float t)
        {
            return t * t * t * t;
        }

        // Ease Out Quart
        public static float EaseOutQuart(float t)
        {
            return 1f - Mathf.Pow(1f - t, 4f);
        }

        // Ease InOut Quart
        public static float EaseInOutQuart(float t)
        {
            return t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) * 0.5f;
        }

        #endregion
        
        
        #region Quint

        // Ease In Quint
        public static float EaseInQuint(float t)
        {
            return t * t * t * t * t;
        }

        // Ease Out Quint
        public static float EaseOutQuint(float t)
        {
            return 1f - Mathf.Pow(1f - t, 5f);
        }

        // Ease InOut Quint
        public static float EaseInOutQuint(float t)
        {
            return t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) * 0.5f;
        }

        #endregion
        
        
        #region Expo

        // Ease In Expo
        public static float EaseInExpo(float t)
        {
            return t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
        }

        // Ease Out Expo
        public static float EaseOutExpo(float t)
        {
            return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
        }

        // Ease InOut Expo
        public static float EaseInOutExpo(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            return t < 0.5f 
                ? Mathf.Pow(2f, 20f * t - 10f) * 0.5f 
                : (2f - Mathf.Pow(2f, -20f * t + 10f)) * 0.5f;
        }

        #endregion
        
        #region Circ

        // Ease In Circ
        public static float EaseInCirc(float t)
        {
            return 1f - Mathf.Sqrt(1f - t * t);
        }

        // Ease Out Circ
        public static float EaseOutCirc(float t)
        {
            return Mathf.Sqrt(1f - (t - 1f) * (t - 1f));
        }

        // Ease InOut Circ
        public static float EaseInOutCirc(float t)
        {
            return t < 0.5f 
                ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * t, 2f))) * 0.5f 
                : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) * 0.5f;
        }

        #endregion
        
        #region Back
        public static float EaseInBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return c3 * x * x * x - c1 * x * x;
        }

        public static float EaseOutBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
        }

        public static float EaseInOutBack(float x)
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            if (x < 0.5f)
                return (Mathf.Pow(2 * x, 2) * ((c2 + 1f) * 2 * x - c2)) / 2f;
            else
                return (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1f) * (x * 2 - 2) + c2) + 2) / 2f;

        }
        #endregion


        #region Elastic
        
        public static float EaseInElastic(float x)
        {
            if (x == 0) return 0;
            if (x == 1) return 1;

            float c4 = (2f * Mathf.PI) / 0.3f; // period p = 0.3
            return -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10 - 10.75f) * c4);
        }

        public static float EaseOutElastic(float x)
        {
            if (x == 0) return 0;
            if (x == 1) return 1;

            float c4 = (2f * Mathf.PI) / 0.33f;
            return Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;

        }

        public static float EaseInOutElastic(float x)
        {
            if (x == 0) return 0;
            if (x == 1) return 1;

            float c5 = (2f * Mathf.PI) / 0.45f; // period p = 0.45

            if (x < 0.5f)
                return -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2;
            else
                return (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1;

        }
        
        #endregion


        #region Bounce
        
        public static float EaseInBounce(float x) => 1f - EaseOutBounce(1f - x);
        
        public static float EaseOutBounce(float x)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (x < 1 / d1)
                return n1 * x * x;
            else if (x < 2 / d1)
            {
                x -= 1.5f / d1;
                return n1 * x * x + 0.75f;
            }
            else if (x < 2.5 / d1)
            {
                x -= 2.25f / d1;
                return n1 * x * x + 0.9375f;
            }
            else
            {
                x -= 2.625f / d1;
                return n1 * x * x + 0.984375f;
            }
        }
        
        public static float EaseInOutBounce(float x)
        {
            
            if (x < 0.5f)
                return (1f - EaseOutBounce(1f - 2f * x)) / 2f;
            else
                return (1f + EaseOutBounce(2f * x - 1f)) / 2f;
        }
        
        #endregion
        
        
    }
}