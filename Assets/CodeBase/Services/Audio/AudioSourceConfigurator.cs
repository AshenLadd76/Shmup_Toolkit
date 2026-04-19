using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Services.Audio
{
    public static class AudioSourceConfigurator
    {
        public static AudioSource ConfigAudioSource(IAudioDefinition audioDefinition, AudioSource audioSource)
        {
            if (audioSource == null)
            {
                Logger.LogError("Audio source is null");
                return null;
            }

            if (audioDefinition == null)
            {
                Logger.LogError("Audio definition is null");
                return null;
            }
            
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

        public static AudioSource ResetAudioSource(AudioSource audioSource)
        {
            if (audioSource == null)
            {
                Logger.LogError("Audio source is null");
                return null;
            }
            
            audioSource.clip = null;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = 0;
            audioSource.mute = false;
            audioSource.pitch = 1;
            audioSource.spatialBlend = 1;
            audioSource.minDistance = 0;
            audioSource.maxDistance = 0;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.bypassEffects = false;
            audioSource.bypassReverbZones = false;
            audioSource.bypassListenerEffects = false;
            
            return audioSource;
        }
    }
}