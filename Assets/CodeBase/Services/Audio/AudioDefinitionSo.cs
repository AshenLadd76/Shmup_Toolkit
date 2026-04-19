using UnityEngine;

namespace CodeBase.Services.Audio
{
    [CreateAssetMenu(fileName = "NewAudioDefinition", menuName = "ToolBox/AudioDefinition")]
    public class AudioDefinitionSo : ScriptableObject, IAudioDefinition
    {
        [Header("Clip")]
        [SerializeField] private AudioClip clip;
        [SerializeField] private CodeBase.Audio.AudioType audioType;

        [Header("Playback Settings"), Space(20)]
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField, Range(-3f, 3f)] private float pitch = 1f;
        
        [Header("Spatial Settings")]
        [SerializeField, Range(0f, 1.1f)] private float spatialBlend = 0f; // 0 = 2D, 1 = 3D
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 500f;
        [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        
        
        [SerializeField] private bool loop = false;
        
        [SerializeField] private bool playOnAwake = false;

        [Header("Other Settings")]
        [SerializeField] private bool mute = false;
        [SerializeField] private bool bypassEffects = false;
        [SerializeField] private bool bypassListenerEffects = false;
        [SerializeField] private bool bypassReverbZones = false;
        
    

        // Public read-only accessors
        public AudioClip Clip => clip;
        
        public AudioCommand AudioType => (AudioCommand)audioType;
      
        public bool Loop { get => loop; set => loop = value; }

        public bool PlayOnAwake { get => playOnAwake; set => playOnAwake = value; }
        public float SpatialBlend { get => spatialBlend; set => spatialBlend = value; }
        public float Pitch { get => pitch; set => pitch = value; }
        
        public float MinDistance { get => minDistance; set => minDistance = value; }
        public float MaxDistance { get => maxDistance; set => maxDistance = value; }
        public AudioRolloffMode RolloffMode { get => rolloffMode; set => rolloffMode = value; }
        public bool Mute { get => mute; set => mute = value; }
        public bool BypassEffects { get => bypassEffects; set => bypassEffects = value; }
        public bool BypassListenerEffects { get => bypassListenerEffects; set => bypassListenerEffects = value; }
        public bool BypassReverbZones { get => bypassReverbZones; set => bypassReverbZones = value; }

        public float Volume { get => volume; set => volume = value; }
    }
    
    public interface IAudioDefinition
    {
        AudioClip Clip { get; }
        
        AudioCommand AudioType { get; }

        float Pitch { get; set; }
        bool Loop { get; set; }
        bool PlayOnAwake { get; set; }
        float SpatialBlend { get; set; }
        float MinDistance { get; set; }
        float MaxDistance { get; set; }
        AudioRolloffMode RolloffMode { get; set; }
        bool Mute { get; set; }
        bool BypassEffects { get; set; }
        bool BypassListenerEffects { get; set; }
        bool BypassReverbZones { get; set; }
        float Volume { get; set; }
    }
}