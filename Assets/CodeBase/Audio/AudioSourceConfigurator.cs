using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Audio
{
    public static class AudioSourceConfigurator
    {
        public static AudioSource ConfigAudioSource(IAudioDefinition audioDefinition, AudioSource audioSource)
        {
            audioSource.transform.localPosition = Vector3.zero;
            
            Logger.Log($"AudioService playing audio clip { audioDefinition.SpatialBlend } {audioDefinition.MinDistance} to {audioDefinition.MaxDistance}");
                 
            audioSource.clip = audioDefinition.Clip;
            audioSource.playOnAwake = audioDefinition.PlayOnAwake;
            audioSource.loop = audioDefinition.Loop;
            audioSource.volume = audioDefinition.Volume;
            audioSource.mute = audioDefinition.Mute;
            audioSource.pitch =  audioDefinition.Pitch;
            audioSource.spatialBlend = audioDefinition.SpatialBlend;
            audioSource.minDistance = audioDefinition.MinDistance;
            audioSource.maxDistance = audioDefinition.MaxDistance;
            audioSource.rolloffMode = audioDefinition.RolloffMode;
            audioSource.bypassEffects = audioDefinition.BypassEffects;
            audioSource.bypassReverbZones = audioDefinition.BypassReverbZones;
            audioSource.bypassListenerEffects = audioDefinition.BypassListenerEffects;
            
            return audioSource;
        }
    }
}