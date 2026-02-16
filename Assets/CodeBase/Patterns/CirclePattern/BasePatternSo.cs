using System;
using CodeBase.Modifiers;
using UnityEngine;


namespace CodeBase.Patterns.CirclePattern
{
    public abstract class BasePatternSo : ScriptableObject
    {
        [SerializeField] protected int projectileCount = 8;
        [SerializeField] protected float radius = 1f;
        [SerializeField] protected float speed = 1f;
        [SerializeField] protected float lifeSpan = 5f;
        [SerializeField] protected float fireRate = 0.1f;
        [SerializeField] protected bool applyRotation = true;
        [SerializeField] protected float arcAngle = 0f;

        public float ArcAngle
        {
            get => arcAngle;
            set => arcAngle = value;
        }

        //[SerializeField] protected BaseModifierSo[] modifiers;

        [SerializeField] protected BaseModifierSo rotationModifier;
        [SerializeField] protected BaseModifierSo speedModifier;
        [SerializeField] protected BaseModifierSo scaleModifier;
        [SerializeField] protected BaseModifierSo colourModifier;
        
        [SerializeField] private int patternCount;
        public int PatternCount
        {
            get => patternCount;
            set => patternCount = value;
        }

        
        public int ProjectileCount => projectileCount;
        public float Radius => radius;
        public float Speed => speed;
        public float LifeSpan => lifeSpan;
        public float FireRate => fireRate;
        public bool ApplyRotation => applyRotation;
        

        // Core execute method — every pattern implements this
        public abstract void Execute(ref PatternSample patternSample, Action<PatternSample> callBack = null, float deltaTime = 0f, int index = 0);
        
        protected virtual void AddModifiers(ref PatternSample patternSample, float deltaTime)
        {
            rotationModifier?.Apply(ref patternSample, deltaTime);
            speedModifier?.Apply(ref patternSample, deltaTime);
            scaleModifier?.Apply(ref patternSample, deltaTime);
            colourModifier?.Apply(ref patternSample, deltaTime);
        }
    }
}