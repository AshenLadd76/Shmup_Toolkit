using System.Collections.Generic;
using ToolBox.Helpers;
using ToolBox.Messaging;
using ToolBox.Services;
using ToolBox.Utils.Pooling;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Audio
{
    public class AudioService : BaseService
    {
        [SerializeField, Header("Audio Sfx Clips"), Space(20)] private List<Wormwood.Utils.KeyValuePair<string, AudioDefinitionSo>> audioList;
        [SerializeField, Header("Music Clips")] private List<Wormwood.Utils.KeyValuePair<string, AudioDefinitionSo>> musicClipList;
        
        private readonly Dictionary<string, IAudioDefinition> _audioDictionary = new();
        private readonly Dictionary<string, IAudioDefinition> _musicDictionary = new();
        
        private SfxAudioService _sfxAudioService;
        private MusicAudioService _musicAudioService;
        
        private void Awake()
        {
            InitAudioDictionary();
            InitMusicDictionary();
            
            _sfxAudioService = new SfxAudioService( new CoroutineRunner(this), CreateAudioPool("SfxAudioPool", 10, 50) );
            _musicAudioService = new MusicAudioService(CreateAudioPool("MusicAudioPool", 10, 50), new LinearCrossFader(new CoroutineRunner(this)) );
        }
        
        protected override void SubscribeToService()
        {
            Logger.Log("AudioService subscribed");
            
            MessageBus.AddListener<string>(AudioServiceMessages.RequestPlayOneShot, PlayOneShot );
            MessageBus.AddListener<string, Vector3>(AudioServiceMessages.RequestPlayOneShotAtPosition, PlayOneShotAtPosition);
            
            MessageBus.AddListener<Object,string>(AudioServiceMessages.RequestPlayAudioLoop, PlayLoop);
            MessageBus.AddListener<Object, string, Vector3>(AudioServiceMessages.RequestPlayAudioLoopAtPosition, PlayLoopAtPosition);
            
            MessageBus.AddListener<Object, string>(AudioServiceMessages.RequestPlayMusicTrack, PlayMusicTrack);
            MessageBus.AddListener<Object, string>(AudioServiceMessages.RequestStopMusicTrack, StopMusicTrack);
             
            MessageBus.AddListener<Object, string>(AudioServiceMessages.RequestAudioCrossFade, CrossFade);
        }

        protected override void UnsubscribeFromService()
        {
            Logger.Log("AudioService unsubscribed");
            
            MessageBus.RemoveListener<string>(AudioServiceMessages.RequestPlayOneShot, PlayOneShot );
            MessageBus.RemoveListener<string, Vector3>(AudioServiceMessages.RequestPlayOneShotAtPosition, PlayOneShotAtPosition);
            
            MessageBus.RemoveListener<Object,string>(AudioServiceMessages.RequestPlayAudioLoop, PlayLoop);
            MessageBus.RemoveListener<Object,string, Vector3>(AudioServiceMessages.RequestPlayAudioLoopAtPosition, PlayLoopAtPosition);
            
            MessageBus.RemoveListener<Object, string>(AudioServiceMessages.RequestPlayMusicTrack, PlayMusicTrack);
            MessageBus.RemoveListener<Object, string>(AudioServiceMessages.RequestStopMusicTrack, StopMusicTrack);
            
            MessageBus.RemoveListener<Object,string>(AudioServiceMessages.RequestAudioCrossFade, CrossFade);
        }

        
        //Audio
        public void PlayOneShot(string id)
        {
            var key = FormatID(id);

            var audioDefinition = GetAudioDefinition(key, _audioDictionary);
            
            _sfxAudioService.PlayOneShot(audioDefinition);
        }

        public void PlayOneShotAtPosition(string id, Vector3 position)
        {
            var key = FormatID(id);

            var audioDefinition = GetAudioDefinition(key, _audioDictionary);
            
            
            _sfxAudioService.PlayOneShotAtPosition(audioDefinition, position);
        }
        //Audio ends
        
        
        //Music
        public void PlayMusicTrack(Object owner, string id)
        {
            var key = FormatID(id);
            
            var audioDefinition = GetAudioDefinition(key, _musicDictionary);
            
            _musicAudioService.PlayMusic(owner,key,audioDefinition);
        }

        public void StopMusicTrack(Object owner, string id)
        {
            var key = FormatID(id);
            
            _musicAudioService.StopMusic(owner, key);
        }
        
        
        public void PlayLoop(Object owner,string id)
        {
            var key = FormatID(id);
            
            var audioDefinition = GetAudioDefinition(key, _musicDictionary);
            
            _musicAudioService.PlayAudioLoop(owner,key, audioDefinition);
        }

        public void PlayLoopAtPosition(Object owner, string id, Vector3 position)
        {
            var key = FormatID(id);
            
            var audioDefinition = GetAudioDefinition(key, _musicDictionary);

            if (audioDefinition == null)
            {
                Logger.Log("Audio definition not found");
                return;
            }
            
            _musicAudioService.PlayAudioLoopAtPosition(owner, key, audioDefinition, position);
        }

        public void StopLoop(Object owner, string id)
        {
            var key = FormatID(id);
            
            _musicAudioService.StopAudioLoop(owner, key);
        }

        public void CrossFade(Object owner,string id)
        {
            var key = FormatID(id);
            
            var audioDefinition = GetAudioDefinition(key, _musicDictionary);
            
            _musicAudioService.CrossFadeAudioTrack(owner, key, audioDefinition);
        }


        private string FormatID(string id) => id.Trim().ToLower();

        private IAudioDefinition GetAudioDefinition(string key, Dictionary<string,IAudioDefinition> audioDictionary)
        {
            if (_musicDictionary.TryGetValue(key, out var audioDefinition)) return audioDefinition;
            
            Logger.Log($"AudioService clip not found {key}");
            
            return null;
        }
        
        
        private void InitAudioDictionary()
        {
            foreach (var audioDefinition in audioList)
                _audioDictionary[audioDefinition.Key.Trim().ToLower()] = audioDefinition.Value;
        }

        private void InitMusicDictionary()
        {
            foreach (var audioDefinition in musicClipList)
                _musicDictionary[audioDefinition.Key.Trim().ToLower()] = audioDefinition.Value;
        }
        
        private GenericPool<AudioSource> CreateAudioPool(string poolRootName, int preloadCount, int maxPoolSize)
        {
            var poolRoot = new GameObject($"{poolRootName}");
            poolRoot.transform.SetParent(transform);
            
            return new GenericPool<AudioSource>(createFunc: () => {
                    
                    var go = new GameObject("AudioSource");
                    
                    go.transform.SetParent(poolRoot.transform);
                    
                    var audioSource = go.AddComponent<AudioSource>();
                    
                    audioSource.playOnAwake = false;
                    audioSource.loop = false;
                    
                    return audioSource;
                },
                onGet: source => source.gameObject.SetActive(true),
                onRelease: source => {
                    source.Stop();
                    source.clip = null;
                    source.gameObject.SetActive(false);
                },
                preLoadCount: preloadCount,
                maxSize: maxPoolSize);
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
       
        public const string RequestPlayMusicTrack = "RequestPlayMusicTrack";
        public const string RequestStopMusicTrack = "RequestStopMusicTrack";
        
        
        public const string RequestAudioCrossFade = "RequestAudioCrossFade";
    }
}
