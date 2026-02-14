using System;
using CodeBase.Patterns.CirclePattern;
using ToolBox.Extensions;
using UnityEngine;

namespace CodeBase.Patterns.CompositePatterns.Flower
{
    [CreateAssetMenu(menuName = "Shmup/Composite Patterns/Flower Pattern")]
    public class FlowerPatternSo : CompositePatternSo
    { 
        [SerializeField] private BasePatternSo[] patterns;

        public int SampleCount { get; set; } = 1;

        public void Execute(int index, ref PatternSample[] patternSample, Action<PatternSample> callBack,  float deltaTime = 0)
        {
            if (patterns.IsNullOrEmpty()) return;

            if (patternSample.IsNullOrEmpty()) return;

            for (int i = 0; i < patterns.Length; i++)
            {
                patternSample[i].RotationMultiplier = i % 2 == 0 ? 1 : -1;
                
                patterns[i].Execute(index, ref patternSample[i], null, deltaTime);  
                
                callBack?.Invoke(patternSample[i]);
            }
        }
    }

    public abstract class CompositePatternSo : ScriptableObject
    {
        
    }
}
