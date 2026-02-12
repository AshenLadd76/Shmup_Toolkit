using CodeBase.Patterns.CirclePattern;
using UnityEngine;

namespace CodeBase.Patterns.Phase
{
    public abstract class BaseModifierSo : ScriptableObject
    {
        // Applies the modifier to the passed-in PatternSample
        public abstract void Apply(ref PatternSample sample, float deltaTime = 0f);
    }
}