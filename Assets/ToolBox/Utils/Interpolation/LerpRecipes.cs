using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Utils.Interpolation
{
    [CreateAssetMenu(fileName = "LerpRecipes", menuName = "Interpolation/Lerp Recipe")]
    public class LerpRecipes : ScriptableObject
    {
        [SerializeField] private LerpType lerpType = LerpType.Linear;

        private Dictionary<LerpType, Func<float, float>> _recipes = new Dictionary<LerpType, Func<float, float>>()
        {
            { LerpType.Linear, t => t },
            { LerpType.SmoothStep, t => Mathf.SmoothStep(0f, 1f, t) },
            { LerpType.EaseIn, t => t * t },
            { LerpType.EaseOut, t => 1 - Mathf.Pow(1 - t, 2) },
            { LerpType.EaseInOut, t => t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f }
        };
        
        private Func<float, float> _lerpRecipe;

        private void OnEnable() => SetLerpRecipe();
        
        private void OnValidate() => SetLerpRecipe();
        
        private void SetLerpRecipe()
        {
            if (!_recipes.TryGetValue(lerpType, out var func))
                func = _recipes[LerpType.Linear];

            _lerpRecipe = func;
        }
        
        private float Apply(float t)
        {
            return _lerpRecipe(Mathf.Clamp01(t));
        }
        
        public Vector3 Lerp(Vector3 from, Vector3 to, float t)
        {
            float adjustedT = Apply(t);
            return Vector3.Lerp(from, to, adjustedT);
        }
        
    }

    public enum LerpType { Linear, SmoothStep, EaseIn, EaseOut, EaseInOut }
}