using System.Collections.Generic;
using ToolBox.Helpers;
using ToolBox.Messaging;
using ToolBox.Services;
using ToolBox.Utils.Pooling;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Audio
{
    public class AudioService : BaseService, IAudioService
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
            _musicAudioService = new MusicAudioService(new CoroutineRunner(this), CreateAudioPool("MusicAudioPool", 10, 50) );
        }
        
        protected override void SubscribeToService()
        {
            Logger.Log("AudioService subscribed");
            MessageBus.AddListener<string>(AudioServiceMessages.RequestPlayOneShot, PlayOneShot );
            MessageBus.AddListener<string, Vector3>(AudioServiceMessages.RequestPlayOneShotAtPosition, PlayOneShotAtPosition);
            
            MessageBus.AddListener<string>(AudioServiceMessages.RequestPlayAudioLoop, PlayLoop);
            MessageBus.AddListener<string>(AudioServiceMessages.RequestPlayMusicTrack, PlayMusicTrack);
            
            MessageBus.AddListener<string>(AudioServiceMessages.RequestAudioCrossFade, CrossFade);
     
        }

        protected override void UnsubscribeFromService()
        {
            Logger.Log("AudioService unsubscribed");
            MessageBus.RemoveListener<string>(AudioServiceMessages.RequestPlayOneShot, PlayOneShot );
            MessageBus.RemoveListener<string, Vector3>(AudioServiceMessages.RequestPlayOneShotAtPosition, PlayOneShotAtPosition);
            
            MessageBus.RemoveListener<string>(AudioServiceMessages.RequestPlayAudioLoop, PlayLoop);
            
            MessageBus.RemoveListener<string>(AudioServiceMessages.RequestAudioCrossFade, CrossFade);
        }

        
        //Audio
        public void PlayOneShot(string id)
        {
            var key = FormatID(id);
            
            if (!_audioDictionary.TryGetValue(key, out var audioDefinition))
            {
                Logger.Log($"AudioService clip not found {key}");
                return;
            }
            
            _sfxAudioService.PlayOneShot(audioDefinition);
        }

        public void PlayOneShotAtPosition(string id, Vector3 position)
        {
            var key = FormatID(id);

            if (!_audioDictionary.TryGetValue(key, out var audioDefinition))
            {
                Logger.Log($"AudioService clip not found {key}");
                return;
            }
            
            _sfxAudioService.PlayOneShotAtPosition(audioDefinition, position);
        }
        //Audio ends
        
        
        //Music
        public void PlayMusicTrack(string id)
        {
            var key = FormatID(id);
            
            if (!_musicDictionary.TryGetValue(key, out var audioDefinition))
            {
                Logger.Log($"AudioService clip not found {key}");
                return;
            }
            
            _musicAudioService.PlayMusic(key, audioDefinition);
        }
        
        
        public void PlayLoop(string id)
        {
            var key = FormatID(id);
            
            if (!_musicDictionary.TryGetValue(key, out var audioDefinition))
            {
                Logger.Log($"AudioService clip not found {key}");
                return;
            }
            
            _musicAudioService.PlayAudioLoop(key, audioDefinition);
        }

        public void PlayLoopAtPosition(string id, Vector3 position)
        {
            var key = FormatID(id);
            
            if (!_musicDictionary.TryGetValue(key, out var audioDefinition))
            {
                Logger.Log($"AudioService clip not found {key}");
                return;
            }
            
            _musicAudioService.PlayAudioLoop(key, audioDefinition);
        }

        public void StopLoop(string id)
        {
            var key = FormatID(id);
            
            _musicAudioService.StopAudioLoop(key);
        }

        public void CrossFade(string id)
        {
            var key = FormatID(id);
            
            if (!_musicDictionary.TryGetValue(key, out var audioDefinition))
            {
                Logger.Log($"AudioService clip not found {key}");
                return;
            }
            
            _musicAudioService.CrossFadeAudioTrack(key, audioDefinition);
        }


        private string FormatID(string id) => id.Trim().ToLower();
        
        
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
        public const string RequestAudioCrossFade = "RequestAudioCrossFade";
    }

    public interface IAudioService
    {
        public void PlayOneShot(string id);
        public void PlayOneShotAtPosition(string id, Vector3 position);
        public void PlayLoop(string id);
        public void StopLoop(string id);
        public void PlayLoopAtPosition(string id, Vector3 position);
    }
}
