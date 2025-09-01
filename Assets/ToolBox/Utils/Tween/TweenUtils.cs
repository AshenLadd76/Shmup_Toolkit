using System;
using System.Collections;
using UnityEngine;

namespace ToolBox.Utils.Tween
{
    public static class TweenUtils
    {
        public static IEnumerator LerpFloat(float from, float to, float duration, Action<float> onUpdate)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float value = Mathf.Lerp(from, to, t);
                onUpdate?.Invoke(value);
                yield return null;
            }
            
            onUpdate?.Invoke(to);
        }
    }
}