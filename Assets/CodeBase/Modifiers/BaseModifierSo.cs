using CodeBase.Patterns.CirclePattern;
using UnityEngine;

namespace CodeBase.Modifiers
{
    public abstract class BaseModifierSo : ScriptableObject
    {

        [SerializeField] public bool isEnabled;
        // Applies the modifier to the passed-in PatternSample
        public abstract void Apply(ref PatternSample sample, float deltaTime = 0f);
    }
}