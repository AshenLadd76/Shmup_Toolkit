using CodeBase.Modifiers.Algorithms;
using CodeBase.Patterns.CirclePattern;
using UnityEngine;

namespace CodeBase.Modifiers
{
    [CreateAssetMenu(menuName = "Shmup/Modifiers/Colour Cycle Modifier")]
    public class ColourModifierSo : BaseModifierSo
    {
        [SerializeField] private bool randomColours;
        
        [SerializeField] private float colourCycleSpeed;
        [SerializeField] private Color[] colours;
        
        private ColourCycle _colourCycle;

        private void OnEnable()
        {
            _colourCycle = new ColourCycle(colours, colourCycleSpeed);
        }
        
        public override void Apply(ref PatternSample patternSample, ref float speed, float deltaTime = 0)
        {
            if (!isEnabled) return;
            
           _colourCycle.GetColor(ref patternSample, deltaTime);
        }
    }
}
