using UnityEngine;

namespace CodeBase.Audio
{
    [CreateAssetMenu(fileName = "NewAudioDefinition", menuName = "ToolBox/AudioDefinition")]
    public class AudioDefinitionSo : ScriptableObject
    {
        [Header("Clip")]
        [SerializeField] private AudioClip clip;

        [Header("Playback Settings")]
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField, Range(-3f, 3f)] private float pitch = 1f;
        [SerializeField] private bool loop = false;
        [SerializeField] private bool playOnAwake = false;

        [Header("Spatial Settings")]
        [SerializeField, Range(0f, 1.1f)] private float spatialBlend = 0f; // 0 = 2D, 1 = 3D
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 500f;
        [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

        [Header("Other Settings")]
        [SerializeField] private bool mute = false;
        [SerializeField] private bool bypassEffects = false;
        [SerializeField] private bool bypassListenerEffects = false;
        [SerializeField] private bool bypassReverbZones = false;

        // Public read-only accessors
        public AudioClip Clip => clip;
        public float Volume => volume;
        public float Pitch => pitch;
        public bool Loop => loop;
        public bool PlayOnAwake => playOnAwake;
        public float SpatialBlend => spatialBlend;
        public float MinDistance => minDistance;
        public float MaxDistance => maxDistance;
        public AudioRolloffMode RolloffMode => rolloffMode;
        public bool Mute => mute;
        public bool BypassEffects => bypassEffects;
        public bool BypassListenerEffects => bypassListenerEffects;
        public bool BypassReverbZones => bypassReverbZones;
    }
}