using System;
using System.Collections.Generic;
using CodeBase.Audio;
using ToolBox.Helpers;
using ToolBox.Messaging;
using ToolBox.Services;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;
using Object = UnityEngine.Object;

namespace CodeBase.Services.Audio
{
    public class AudioService : BaseService
    {
        [SerializeField, Header("Audio Sfx Clips"), Space(20)] private List<Wormwood.Utils.KeyValuePair<string, AudioDefinitionSo>> audioList;
        [SerializeField, Header("Music Clips")] private List<Wormwood.Utils.KeyValuePair<string, AudioDefinitionSo>> musicClipList;

        [SerializeField] private int audioPreloadCount = 10;
        [SerializeField] private int maxPoolSize = 50;
        
        private readonly Dictionary<string, IAudioDefinition> _oneShotAudioDictionary = new();
        private readonly Dictionary<string, IAudioDefinition> _loopingAudioDictionary = new();
        
        private SfxAudioService _sfxAudioService;
        private LoopingAudioService _loopingAudioService;
        private MusicService _musicAudioService;
        
        private AudioPoolCreator _audioPoolCreator;
        
        private const string SfxAudioPool = "SfxAudioPool";
        private const string LoopingAudioPool = "LoopingAudioPool";
        private const string MusicAudioPool = "MusicAudioPool";
        
        private void Awake()
        {
            InitAudioDictionary();
            InitMusicDictionary();
            
            _audioPoolCreator = new AudioPoolCreator();
            
            _sfxAudioService = new SfxAudioService( new CoroutineRunner(this), _audioPoolCreator.CreateAudioPool(SfxAudioPool, audioPreloadCount, maxPoolSize, transform) );
            _loopingAudioService = new LoopingAudioService(_audioPoolCreator.CreateAudioPool(LoopingAudioPool, audioPreloadCount, maxPoolSize, transform));
            _musicAudioService = new MusicService(_audioPoolCreator.CreateAudioPool(MusicAudioPool, audioPreloadCount, maxPoolSize, transform), new LinearCrossFader(new CoroutineRunner(this)) );
        }
        
        protected override void SubscribeToService()
        {
            MessageBus.AddListener<string>( AudioServiceMessages.RequestAudio, RequestAudio );
            
            
            //One Shot audio
            MessageBus.AddListener<string>(AudioServiceMessages.RequestPlayOneShot, PlayOneShot );
            MessageBus.AddListener<string, Vector3>(AudioServiceMessages.RequestPlayOneShotAtPosition, PlayOneShotAtPosition);
            
            //Looped Audio
            MessageBus.AddListener<Object,string>(AudioServiceMessages.RequestPlayAudioLoop, PlayAudioLoop);
            MessageBus.AddListener<Object, string, Vector3>(AudioServiceMessages.RequestPlayAudioLoopAtPosition, PlayLoopAtPosition);
            MessageBus.AddListener<Object, string>(AudioServiceMessages.RequestStopAudioLoop, StopAudioLoop);
            
            //Music
            MessageBus.AddListener<Object, string>( AudioServiceMessages.RequestPlayMusic, PlayMusic);
            MessageBus.AddListener<Object, string, Vector3>( AudioServiceMessages.RequestPlayMusicAtPosition, PlayMusicAtPosition );
            MessageBus.AddListener<Object, string>( AudioServiceMessages.RequestStopMusic, StopMusic);
            
            //Cross-fade
            MessageBus.AddListener<Object, string>(AudioServiceMessages.RequestAudioCrossFade, CrossFade);
        }

        protected override void UnsubscribeFromService()
        {
            
            MessageBus.RemoveListener<string>( AudioServiceMessages.RequestAudio, RequestAudio );
            
            //One Shot Audio
            MessageBus.RemoveListener<string>(AudioServiceMessages.RequestPlayOneShot, PlayOneShot );
            MessageBus.RemoveListener<string, Vector3>(AudioServiceMessages.RequestPlayOneShotAtPosition, PlayOneShotAtPosition);
            
            //Looped Audio
            MessageBus.RemoveListener<Object,string>(AudioServiceMessages.RequestPlayAudioLoop, PlayAudioLoop);
            MessageBus.RemoveListener<Object,string, Vector3>(AudioServiceMessages.RequestPlayAudioLoopAtPosition, PlayLoopAtPosition);
            MessageBus.RemoveListener<Object, string>(AudioServiceMessages.RequestStopAudioLoop, StopAudioLoop);
            
            //Music
            MessageBus.RemoveListener<Object, string>( AudioServiceMessages.RequestPlayMusic, PlayMusic);
            MessageBus.RemoveListener<Object, string, Vector3>( AudioServiceMessages.RequestPlayMusicAtPosition, PlayMusicAtPosition );
            MessageBus.RemoveListener<Object, string>( AudioServiceMessages.RequestStopMusic, StopMusic);
            
            //Cross-fade
            MessageBus.RemoveListener<Object,string>(AudioServiceMessages.RequestAudioCrossFade, CrossFade);
        }

        private void RequestAudio(string id)
        {
            Logger.Log( $"Requesting audio clip for {id} received..." );
            
            if(!AudioDefinitionResolver.TryResolveAudio(id, _oneShotAudioDictionary, out var key,out var audioDefinition)) return;

            var audioRequest = new AudioRequest();
        }
        
        private void InitAudioDictionary()
        {
            foreach (var audioDefinition in audioList)
                _oneShotAudioDictionary[AudioDefinitionResolver.FormatID(audioDefinition.Key)] = audioDefinition.Value;
        }

        private void InitMusicDictionary()
        {
            foreach (var audioDefinition in musicClipList)
                _loopingAudioDictionary[AudioDefinitionResolver.FormatID(audioDefinition.Key)] = audioDefinition.Value;
        }
        
        //One Shot Audio
        private void PlayOneShot(string id)
        {
            if(!AudioDefinitionResolver.TryResolveAudio(id, _oneShotAudioDictionary, out var key,out var audioDefinition)) return;
            
            _sfxAudioService.PlayOneShot(audioDefinition);
        }

        private void PlayOneShotAtPosition(string id, Vector3 position)
        {
            if(!AudioDefinitionResolver.TryResolveAudio(id, _oneShotAudioDictionary, out var key,out var audioDefinition)) return;
            
            _sfxAudioService.PlayOneShotAtPosition(audioDefinition, position);
        }
        //One Shot Audio ends
        
        
        
        //Looping Audio
        private void PlayAudioLoop(Object owner,string id)
        {
            if(!AudioDefinitionResolver.TryResolveAudio(id, _loopingAudioDictionary, out var key,out var audioDefinition)) return;

            _loopingAudioService.PlayAudioLoop(owner, key, audioDefinition);
        }

        private void PlayLoopAtPosition(Object owner, string id, Vector3 position)
        {
            if(!AudioDefinitionResolver.TryResolveAudio(id, _loopingAudioDictionary, out var key,out var audioDefinition)) return;
            
            _loopingAudioService.PlayAudioLoopAtPosition(owner, key, audioDefinition, position);
        }

        private void StopAudioLoop(Object owner, string id)
        {
            if(!AudioDefinitionResolver.TryResolveAudio(id, _loopingAudioDictionary, out var key,out var audioDefinition)) return;
            
            _loopingAudioService.StopAudioLoop(owner, key);
        }
        //Looping audio ends
        
        
        //Music
        private void PlayMusic(Object owner, string id)
        {
            if(!AudioDefinitionResolver.TryResolveAudio(id, _loopingAudioDictionary, out var key,out var audioDefinition)) return;
            
            _musicAudioService.PlayMusic(owner, key, audioDefinition);
        }

        private void PlayMusicAtPosition(Object owner, string id, Vector3 position)
        {
            if(!AudioDefinitionResolver.TryResolveAudio(id, _loopingAudioDictionary, out var key,out var audioDefinition)) return;
            
            _musicAudioService.PlayMusicAtPosition(owner, key, audioDefinition, position);
        }

        private void StopMusic(Object owner, string id)
        {
            if(!AudioDefinitionResolver.TryResolveAudio(id, _loopingAudioDictionary, out var key,out var audioDefinition)) return;
            
            _musicAudioService.StopMusic(owner, key);
        }
        
        private void CrossFade(Object owner,string id)
        {
            if(!AudioDefinitionResolver.TryResolveAudio(id, _loopingAudioDictionary, out var key,out var audioDefinition)) return;
            
            _musicAudioService.CrossFadeAudioTrack(owner, key, audioDefinition);
        }
        //Music ends
    }

    public class AudioRequest
    {
        public string Key { get; set; }
        public Object Owner { get; set; }
        public IAudioDefinition AudioDefinition { get; set; }
        public Vector3 Position { get; set; }
    }
    
    public class AudioRouter
    {
        private readonly Dictionary<string, Action<AudioRequest>> _audioRouterDictionary = new();

        private readonly SfxAudioService _sfxAudioService;
        private readonly LoopingAudioService _loopingAudioService;
        private readonly MusicService _musicService;
        
        private readonly IAudioPoolCreator _audioPoolCreator;
        
        private const string SfxAudioPool = "SfxAudioPool";
        private const string LoopingAudioPool = "LoopingAudioPool";
        private const string MusicAudioPool = "MusicAudioPool";
        
        private readonly int audioPreloadCount = 10;
        private readonly int maxPoolSize = 50;
        
        public AudioRouter(ICoroutineRunner coroutineRunner, Transform parent)
        {
            _audioPoolCreator = new AudioPoolCreator();
            
            _sfxAudioService = new SfxAudioService( coroutineRunner, _audioPoolCreator.CreateAudioPool(SfxAudioPool, audioPreloadCount, maxPoolSize, parent));
            _loopingAudioService = new LoopingAudioService(_audioPoolCreator.CreateAudioPool(LoopingAudioPool, audioPreloadCount, maxPoolSize, parent));
            
            _musicService = new MusicService(_audioPoolCreator.CreateAudioPool(MusicAudioPool, audioPreloadCount, maxPoolSize, parent), new LinearCrossFader(coroutineRunner) );
            
            InitAudioDictionary();
        }

        private void InitAudioDictionary()
        {
            _audioRouterDictionary.Clear();

            _audioRouterDictionary.Add(AudioServiceMessages.RequestPlayOneShot, (audioRequest) => { _sfxAudioService.PlayOneShot(audioRequest.AudioDefinition); });
            _audioRouterDictionary.Add(AudioServiceMessages.RequestPlayOneShotAtPosition, (audioRequest) => { _sfxAudioService.PlayOneShotAtPosition(audioRequest.AudioDefinition, audioRequest.Position); });
            
            _audioRouterDictionary.Add(AudioServiceMessages.RequestPlayAudioLoop, (audioRequest) => { _loopingAudioService.PlayAudioLoop(audioRequest.Owner, audioRequest.Key, audioRequest.AudioDefinition); });
            _audioRouterDictionary.Add(AudioServiceMessages.RequestPlayAudioLoopAtPosition, (audioRequest) => { _loopingAudioService.PlayAudioLoopAtPosition(audioRequest.Owner, audioRequest.Key, audioRequest.AudioDefinition, audioRequest.Position); });
            _audioRouterDictionary.Add(AudioServiceMessages.RequestStopAudioLoop, (audioRequest => { _loopingAudioService.StopAudioLoop(audioRequest.Owner, audioRequest.Key);} ));
            
            _audioRouterDictionary.Add(AudioServiceMessages.RequestPlayMusic, (audioRequest) => { _musicService.PlayMusic(audioRequest.Owner, audioRequest.Key, audioRequest.AudioDefinition); });
            _audioRouterDictionary.Add(AudioServiceMessages.RequestPlayMusicAtPosition, (audioRequest) => { _musicService.PlayMusicAtPosition(audioRequest.Owner, audioRequest.Key, audioRequest.AudioDefinition, audioRequest.Position); });
            _audioRouterDictionary.Add(AudioServiceMessages.RequestStopMusic, (audioRequest) => { _musicService.StopMusic(audioRequest.Owner, audioRequest.Key); });
        }

        public void ExecuteAudioRequest(string key, AudioRequest request)
        {
            if (string.IsNullOrEmpty(key)) return;

            if (!_audioRouterDictionary.TryGetValue(key, out var audioAction))
            {
                Logger.LogError($"Audio request for {key} not found");
                return;
            }

            audioAction?.Invoke(request);
        }
    }
}
