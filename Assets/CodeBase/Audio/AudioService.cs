using System.Collections.Generic;
using ToolBox.Extensions;
using ToolBox.Helpers;
using ToolBox.Messaging;
using ToolBox.Services;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Audio
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
        private MusicAudioService _musicAudioService;
        
        private AudioPoolCreator _audioPoolCreator;
        
        private const string SfxAudioPool = "SfxAudioPool";
        private const string MusicAudioPool = "MusicAudioPool";
        
        private void Awake()
        {
            InitAudioDictionary();
            InitMusicDictionary();
            
            _audioPoolCreator = new AudioPoolCreator();
            
            _sfxAudioService = new SfxAudioService( new CoroutineRunner(this), _audioPoolCreator.CreateAudioPool(SfxAudioPool, audioPreloadCount, maxPoolSize, transform) );
            _musicAudioService = new MusicAudioService(_audioPoolCreator.CreateAudioPool(MusicAudioPool, audioPreloadCount, maxPoolSize, transform), new LinearCrossFader(new CoroutineRunner(this)) );
        }
        
        protected override void SubscribeToService()
        {
            //One Shot audio
            MessageBus.AddListener<string>(AudioServiceMessages.RequestPlayOneShot, PlayOneShot );
            MessageBus.AddListener<string, Vector3>(AudioServiceMessages.RequestPlayOneShotAtPosition, PlayOneShotAtPosition);
            
            //Looped Audio
            MessageBus.AddListener<Object,string>(AudioServiceMessages.RequestPlayAudioLoop, PlayLoop);
            MessageBus.AddListener<Object, string, Vector3>(AudioServiceMessages.RequestPlayAudioLoopAtPosition, PlayLoopAtPosition);
            MessageBus.AddListener<Object, string>(AudioServiceMessages.RequestStopAudioLoop, StopAudioLoop);
            
            //Cross-fade
            MessageBus.AddListener<Object, string>(AudioServiceMessages.RequestAudioCrossFade, CrossFade);
        }

        protected override void UnsubscribeFromService()
        {
            //One Shot Audio
            MessageBus.RemoveListener<string>(AudioServiceMessages.RequestPlayOneShot, PlayOneShot );
            MessageBus.RemoveListener<string, Vector3>(AudioServiceMessages.RequestPlayOneShotAtPosition, PlayOneShotAtPosition);
            
            //Looped Audio
            MessageBus.RemoveListener<Object,string>(AudioServiceMessages.RequestPlayAudioLoop, PlayLoop);
            MessageBus.RemoveListener<Object,string, Vector3>(AudioServiceMessages.RequestPlayAudioLoopAtPosition, PlayLoopAtPosition);
            MessageBus.RemoveListener<Object, string>(AudioServiceMessages.RequestStopAudioLoop, StopAudioLoop);
            
            //Cross-fade
            MessageBus.RemoveListener<Object,string>(AudioServiceMessages.RequestAudioCrossFade, CrossFade);
        }
        
        private void InitAudioDictionary()
        {
            foreach (var audioDefinition in audioList)
                _oneShotAudioDictionary[FormatID(audioDefinition.Key)] = audioDefinition.Value;
        }

        private void InitMusicDictionary()
        {
            foreach (var audioDefinition in musicClipList)
                _loopingAudioDictionary[FormatID(audioDefinition.Key)] = audioDefinition.Value;
        }
        
        //One Shot Audio
        private void PlayOneShot(string id)
        {
            if ( !TryGetDefinition(id, _oneShotAudioDictionary, out var audioDefinition, out var key) ) return;
            
            _sfxAudioService.PlayOneShot(audioDefinition);
        }

        private void PlayOneShotAtPosition(string id, Vector3 position)
        {
            if ( !TryGetDefinition(id, _oneShotAudioDictionary, out var audioDefinition, out var key) ) return;
            
            _sfxAudioService.PlayOneShotAtPosition(audioDefinition, position);
        }
        //One Shot Audio ends
        
        
        
        //Looping Audio
        public void PlayLoop(Object owner,string id)
        {
            if ( !TryGetDefinition(id, _loopingAudioDictionary, out var audioDefinition, out var key) ) return;
            
            switch (audioDefinition.AudioType)
            {
                case AudioType.Loop:
                    _musicAudioService.PlayAudioLoop(owner, key, audioDefinition);
                    break;
                
                case AudioType.Music:
                    _musicAudioService.PlayMusic(owner,key, audioDefinition);
                    break;
            }
        }

        private void PlayLoopAtPosition(Object owner, string id, Vector3 position)
        {
            if ( !TryGetDefinition(id, _loopingAudioDictionary, out var audioDefinition, out var key) ) return;
            
            _musicAudioService.PlayAudioLoopAtPosition(owner, key, audioDefinition, position);
        }

        private void StopAudioLoop(Object owner, string id)
        {
            if ( !TryGetDefinition(id, _loopingAudioDictionary, out var audioDefinition, out var key) ) return;
            
            switch (audioDefinition.AudioType)
            {
                case AudioType.Loop:
                    _musicAudioService.StopAudioLoop(owner, key);
                    break;
                
                case AudioType.Music:
                    _musicAudioService.StopMusic(owner,key);
                    break;
            }
        }

        private void CrossFade(Object owner,string id)
        {
            if ( !TryGetDefinition(id, _loopingAudioDictionary, out var audioDefinition, out var key) ) return;
            
            _musicAudioService.CrossFadeAudioTrack(owner, key, audioDefinition);
        }
        //Looping Audio Finishes
        
        private string FormatID(string id) => id?.Trim().ToLower();
        
        private bool TryGetDefinition(string id, Dictionary<string, IAudioDefinition> dictionary, out IAudioDefinition def, out string key)
        {
            if (string.IsNullOrEmpty(id) || dictionary.IsNullOrEmpty())
            {
                Logger.LogError("Invalid audio request or dictionary not initialised");
                def = null;
                key = null;
                return false;
            }
            
            key = FormatID(id);

            if (dictionary.TryGetValue(key, out def)) return true;
            
            Logger.LogError($"Audio not found: {key}");
            
            return false;
        }
    }

    public static class AudioServiceMessages
    {
        //Sfx Audio Service messages
        public const string RequestPlayOneShot = "RequestPlayOneShot";
        public const string RequestPlayOneShotAtPosition = "RequestPlayOneShotAtPosition";
       
        //Music Service messages
        public const string RequestPlayAudioLoop = "RequestPlayAudioLoop";
        public const string RequestPlayAudioLoopAtPosition = "RequestPlayAudioLoopAtPosition";
        public const string RequestStopAudioLoop = "RequestStopAudioLoop";
      
        
        public const string RequestAudioCrossFade = "RequestAudioCrossFade";
    }

    public enum AudioType
    {
        OneShot,
        Music,
        Loop,
    }
}
