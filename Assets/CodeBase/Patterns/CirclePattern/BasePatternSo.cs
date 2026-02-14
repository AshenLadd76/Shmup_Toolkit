using System;
using CodeBase.Modifiers;
using ToolBox.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = ToolBox.Utils.Logger;

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
        
        [SerializeField] protected BaseModifierSo[] modifiers;

        public int ProjectileCount => projectileCount;
        public float Radius => radius;
        public float Speed => speed;
        public float LifeSpan => lifeSpan;
        public float FireRate => fireRate;
        public bool ApplyRotation => applyRotation;
        

        // Core execute method — every pattern implements this
        public abstract void Execute(int index, ref PatternSample patternSample, Action<PatternSample> callBack = null, float deltaTime = 0f);
        
        protected virtual void AddModifiers(ref PatternSample patternSample)
        {
            if (modifiers.IsNullOrEmpty())
            { 
                Logger.LogError( $"No modifiers found for {name}" );
                return;
            }

            for( int i = 0; i < modifiers.Length; i++ )
                modifiers[i]?.Apply(ref patternSample, Time.deltaTime);
            
        }
    }
}