using System;
using UnityEngine;

namespace CodeBase.Patterns
{
    [Serializable]
    public struct PatternConfig
    {
        [SerializeField] private int projectileCount;
        public int ProjectileCount
        {
            get => projectileCount;
            set => projectileCount = value;
        }
        
        [SerializeField] private int waveCount;

        public int WaveCount
        {
            get => waveCount;
            set => waveCount = value;
        }

        [SerializeField] private float projectileSpeed;
        public float ProjectileSpeed
        {
            get => projectileSpeed;
            set => projectileSpeed = value;
        }

        [SerializeField] private float projectileLifeSpan;
        public float ProjectileLifeSpan
        {
            get => projectileLifeSpan;
            set => projectileLifeSpan = value;
        }

        [SerializeField] private float angleOffset;

        public float AngleOffset
        {
            get => angleOffset;
            set => angleOffset = value;
        }

        [SerializeField] private float numberStepsPerWave;

        public float NumberStepsPerWave
        {
            get => numberStepsPerWave;
            set => numberStepsPerWave = value;
        }


        [SerializeField] private float spreadAngle;
        public float SpreadAngle
        {
            get => spreadAngle;
            set => spreadAngle = value;
        }

        [SerializeField] private float spawnRadius;
        public float SpawnRadius
        {
            get => spawnRadius;
            set => spawnRadius = value;
        }

        [SerializeField] private float fireRate;
        public float FireRate
        {
            get => fireRate;
            set => fireRate = value;
        }
        
       
        [SerializeField] private Vector3 origin;

        public Vector3 Origin
        {
            get => origin;
            set => origin = value;
        }
    }
}