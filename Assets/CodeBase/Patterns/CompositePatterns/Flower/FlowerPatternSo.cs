using System;
using CodeBase.Modifiers;
using CodeBase.Patterns.CirclePattern;
using ToolBox.Extensions;
using UnityEngine;

namespace CodeBase.Patterns.CompositePatterns.Flower
{
    [CreateAssetMenu(menuName = "Shmup/Composite Patterns/Flower Pattern")]
    public class FlowerPatternSo : BaseCompositePatternSo
    { 
        [SerializeField] private BasePatternSo[] patterns;

        [Header("Modifiers")][Space(10)]
        [SerializeField] private RotationModifierSo rotationModifier;
        [SerializeField] private ColourModifierSo[] colourModifierSo;

        [SerializeField, Range(-20f, 20f)]  private float rotationSpeed;
        
        private void OnEnable()
        {
            patternSampleCount = patterns.Length;
        }

        public override void Execute(ref PatternSample[] patternSample, Action<PatternSample> callBack,  float deltaTime = 0)
        {
            if (patterns.IsNullOrEmpty()) return;

            if (patternSample.IsNullOrEmpty()) return;
            
            for (int i = 0; i < patterns.Length; i++)
            {
                ApplyModifiers(i, ref patternSample[i], deltaTime);
                
          
                patternSample[i].RotationMultiplier = i % 2 == 0 ? 1 : -1;
                
                patterns[i].ArcAngle = i % 2 == 0 ? 45 : -90;
                patterns[i].Execute(ref patternSample[i], callBack, deltaTime, i);  
            }
        }

        private void ApplyModifiers(int index, ref PatternSample patternSample, float deltaTime)
        {
            rotationModifier?.Apply(ref patternSample, ref rotationSpeed, deltaTime);
            colourModifierSo[0]?.Apply(ref patternSample, ref rotationSpeed, deltaTime);
        }
    }

    public abstract class BaseCompositePatternSo : ScriptableObject
    {
        [SerializeField] protected float fireRate = 0.5f;
        public float FireRate
        {
            get => fireRate;
            set => fireRate = value;
        }
        
        [SerializeField] protected int patternSampleCount = 1;
        public int PatternSampleCount
        {
            get => patternSampleCount;
            set => patternSampleCount = value;
        }

       [SerializeField] private float projectileLifeTime = 2f;
       public float ProjectileLifeTime
       {
           get => projectileLifeTime;
           set => projectileLifeTime = value;
       }
       
       public abstract void Execute(ref PatternSample[] patternSample, Action<PatternSample> callBack, float deltaTime = 0);
    }
}
