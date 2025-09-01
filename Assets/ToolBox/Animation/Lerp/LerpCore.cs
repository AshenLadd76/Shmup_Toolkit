using System;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Animation.Lerp
{
    public static class LerpCore
    {
        //Delegate type for easing functions
        private delegate float EaseFunc(float t);
        
        // Array of easing functions indexed by EaseType
        private static EaseFunc[] _easeFuncs;
        
        static LerpCore()
        {
             int typeCount = Enum.GetValues(typeof(EaseType)).Length;
            
             _easeFuncs = new EaseFunc[typeCount];

            _easeFuncs[(int)EaseType.Linear] = t => t;
            _easeFuncs[(int)EaseType.SmoothStep] = EaseMathFunctions.SmoothStep;

            // Sine
            _easeFuncs[(int)EaseType.EaseInSine] = EaseMathFunctions.EaseInSine;
            _easeFuncs[(int)EaseType.EaseOutSine] = EaseMathFunctions.EaseOutSine;
            _easeFuncs[(int)EaseType.EaseInOutSine] = EaseMathFunctions.EaseInOutSine;

            // Quad
            _easeFuncs[(int)EaseType.EaseInQuad] = EaseMathFunctions.EaseInQuad;
            _easeFuncs[(int)EaseType.EaseOutQuad] = EaseMathFunctions.EaseOutQuad;
            _easeFuncs[(int)EaseType.EaseInOutQuad] = EaseMathFunctions.EaseInOutQuad;

            // Cubic
            _easeFuncs[(int)EaseType.EaseInCubic] = EaseMathFunctions.EaseInCubic;
            _easeFuncs[(int)EaseType.EaseOutCubic] = EaseMathFunctions.EaseOutCubic;
            _easeFuncs[(int)EaseType.EaseInOutCubic] = EaseMathFunctions.EaseInOutCubic;

            // Quart
            _easeFuncs[(int)EaseType.EaseInQuart] = EaseMathFunctions.EaseInQuart;
            _easeFuncs[(int)EaseType.EaseOutQuart] = EaseMathFunctions.EaseOutQuart;
            _easeFuncs[(int)EaseType.EaseInOutQuart] = EaseMathFunctions.EaseInOutQuart;

            // Quint
            _easeFuncs[(int)EaseType.EaseInQuint] = EaseMathFunctions.EaseInQuint;
            _easeFuncs[(int)EaseType.EaseOutQuint] = EaseMathFunctions.EaseOutQuint;
            _easeFuncs[(int)EaseType.EaseInOutQuint] = EaseMathFunctions.EaseInOutQuint;

            // Expo
            _easeFuncs[(int)EaseType.EaseInExpo] = EaseMathFunctions.EaseInExpo;
            _easeFuncs[(int)EaseType.EaseOutExpo] = EaseMathFunctions.EaseOutExpo;
            _easeFuncs[(int)EaseType.EaseInOutExpo] = EaseMathFunctions.EaseInOutExpo;

            // Circ
            _easeFuncs[(int)EaseType.EaseInCirc] = EaseMathFunctions.EaseInCirc;
            _easeFuncs[(int)EaseType.EaseOutCirc] = EaseMathFunctions.EaseOutCirc;
            _easeFuncs[(int)EaseType.EaseInOutCirc] = EaseMathFunctions.EaseInOutCirc;

            // Back
            _easeFuncs[(int)EaseType.EaseInBack] = EaseMathFunctions.EaseInBack;
            _easeFuncs[(int)EaseType.EaseOutBack] = EaseMathFunctions.EaseOutBack;
            _easeFuncs[(int)EaseType.EaseInOutBack] = EaseMathFunctions.EaseInOutBack;

            // Elastic
            _easeFuncs[(int)EaseType.EaseInElastic] = EaseMathFunctions.EaseInElastic;
            _easeFuncs[(int)EaseType.EaseOutElastic] = EaseMathFunctions.EaseOutElastic;
            _easeFuncs[(int)EaseType.EaseInOutElastic] = EaseMathFunctions.EaseInOutElastic;

            // Bounce
            _easeFuncs[(int)EaseType.EaseInBounce] = EaseMathFunctions.EaseInBounce;
            _easeFuncs[(int)EaseType.EaseOutBounce] = EaseMathFunctions.EaseOutBounce;
            _easeFuncs[(int)EaseType.EaseInOutBounce] = EaseMathFunctions.EaseInOutBounce;
        }

        private static float SetEaseType(EaseType type, float t)
        {
            t = Mathf.Clamp01(t);
            int index = (int)type;

            if (index < 0 || index >= _easeFuncs.Length)
                return t; // fallback
            
            return _easeFuncs[index](t);
        }
    
        
        public static Vector3 Lerp(Vector3 from, Vector3 to, float t, EaseType type)
        {
            t = Mathf.Clamp01(t);
            
            Logger.Log( $"from: {from}, to: {to}, t: {t}, type: {type}" );
            
            float adjustedT = SetEaseType(type, t);
            
            return from + (to - from) * adjustedT;
        }

        public static float Lerp(float from, float to, float t, EaseType type)
        {
            t = Mathf.Clamp01(t);
            
            float adjustedT = SetEaseType(type, t);
            return from + (to - from) * adjustedT;
        }

        public static Quaternion Lerp(Quaternion from, Quaternion to, float t, EaseType type)
        {
            t = Mathf.Clamp01(t);
            
            float adjustedT = SetEaseType(type, t);
            return Quaternion.Slerp(from, to, adjustedT);
        }
    }
}

