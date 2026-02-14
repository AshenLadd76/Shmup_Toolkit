using System;
using CodeBase.Patterns.CirclePattern;
using UnityEngine;

namespace CodeBase.Modifiers
{
    [CreateAssetMenu(menuName = "Shmup/Modifiers/Position Oscillation Modifier")]
    public class PositionOscillatingModifierSo : BaseModifierSo
    {
        [SerializeField] private float amplitude;
        [SerializeField] private float frequency;
        [SerializeField] private Vector2 direction;
        
        private PositionOscillationModifier _positionOscillationModifier;

        private void OnEnable() => InitAlgorithm();
        
        private void OnValidate() => InitAlgorithm();
        
        private void InitAlgorithm() => _positionOscillationModifier = new PositionOscillationModifier(amplitude, frequency, direction);

        public override void Apply(ref PatternSample patternSample, float deltaTime = 0)
        {
           // patternSample. =  _positionOscillationModifier.Evaluate( ref patternSample.WavePhase, deltaTime );
        }
    }
}
