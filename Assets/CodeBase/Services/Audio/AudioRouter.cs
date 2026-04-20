using System;
using System.Collections.Generic;
using ToolBox.Helpers;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Services.Audio
{
    public class AudioRouter
    {
        private readonly Dictionary<AudioCommand, Action<AudioRequest, IAudioDefinition>> _audioRouterDictionary = new();

        private readonly SfxAudioService _sfxAudioService;
        private readonly LoopingAudioService _loopingAudioService;
        private readonly MusicService _musicService;
        
        private const string SfxAudioPool = "SfxAudioPool";
        private const string LoopingAudioPool = "LoopingAudioPool";
        private const string MusicAudioPool = "MusicAudioPool";
        
        private readonly float _fadeDuration;
        
        public AudioRouter(AudioPoolConfigSo audioPoolConfigSo, ICoroutineRunner coroutineRunner, Transform parent)
        {
            if (audioPoolConfigSo == null) throw new ArgumentNullException(nameof(audioPoolConfigSo));
            if (coroutineRunner == null) throw new ArgumentNullException(nameof(coroutineRunner));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            _fadeDuration = audioPoolConfigSo.FadeTime;

            var audioPoolCreator = new AudioPoolCreator();
            
            _sfxAudioService = new SfxAudioService( coroutineRunner, audioPoolCreator.CreateAudioPool(SfxAudioPool, audioPoolConfigSo.SfxPreloadCount, audioPoolConfigSo.SfxMaxPoolSize, parent));
            _loopingAudioService = new LoopingAudioService(audioPoolCreator.CreateAudioPool(LoopingAudioPool, audioPoolConfigSo.LoopingPreloadCount, audioPoolConfigSo.LoopingMaxPoolSize, parent));
            _musicService = new MusicService(audioPoolCreator.CreateAudioPool(MusicAudioPool, audioPoolConfigSo.MusicPreloadCount, audioPoolConfigSo.MusicMaxPoolSize, parent), new LinearCrossFader(coroutineRunner) );
            
            InitAudioRouterDictionary();
        }

        private void InitAudioRouterDictionary()
        {
            _audioRouterDictionary.Add(AudioCommand.OneShot, (audioRequest, audioDefinition) => { _sfxAudioService.PlayOneShot(audioDefinition); });
            _audioRouterDictionary.Add(AudioCommand.OneShotAtPosition, (audioRequest, audioDefinition) => { _sfxAudioService.PlayOneShotAtPosition(audioDefinition, audioRequest.Position); });
            
            _audioRouterDictionary.Add(AudioCommand.LoopingAudio, (audioRequest, audioDefinition) => { _loopingAudioService.PlayAudioLoop(audioRequest.Owner, audioRequest.AudioKey, audioDefinition); });
            _audioRouterDictionary.Add(AudioCommand.LoopingAudioAtPosition, (audioRequest, audioDefinition) => { _loopingAudioService.PlayAudioLoopAtPosition(audioRequest.Owner, audioRequest.AudioKey, audioDefinition, audioRequest.Position); });
            _audioRouterDictionary.Add(AudioCommand.StopAudioLoop, (audioRequest, audioDefinition) => { _loopingAudioService.StopAudioLoop(audioRequest.Owner, audioRequest.AudioKey);} );
            
            _audioRouterDictionary.Add(AudioCommand.Music, (audioRequest, audioDefinition) => { _musicService.PlayMusic(audioRequest.Owner, audioRequest.AudioKey, audioDefinition); });
            _audioRouterDictionary.Add(AudioCommand.MusicAtPosition, (audioRequest, audioDefinition) => { _musicService.PlayMusicAtPosition(audioRequest.Owner, audioRequest.AudioKey, audioDefinition, audioRequest.Position); });
            _audioRouterDictionary.Add(AudioCommand.StopMusic, (audioRequest, audioDefinition) => { _musicService.StopMusic(audioRequest.Owner, audioRequest.AudioKey); });
            
            _audioRouterDictionary.Add(AudioCommand.CrossFade, (audioRequest, audioDefinition) => { _musicService.CrossFadeAudioTrack(audioRequest.Owner, audioRequest.AudioKey, audioDefinition, _fadeDuration); });
        }

        public void ExecuteAudioRequest(AudioRequest audioRequest, IAudioDefinition audioDefinition)
        {
            var audioCommand =  audioRequest.AudioCommand;
            
            if (audioDefinition == null)
            {
                Logger.LogError( $"Required AudioDefinition is null" );
                return;
            }
            
            if (!_audioRouterDictionary.TryGetValue(audioCommand, out var audioAction))
            {
                Logger.LogError($"Audio request for {audioCommand} not found");
                return;
            }

            audioAction.Invoke(audioRequest, audioDefinition);
        }
    }
}