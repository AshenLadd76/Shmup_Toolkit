using System.Collections.Generic;
using ToolBox.Messaging;
using ToolBox.Services;
using ToolBox.Utils.Pooling;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Audio
{
    public class AudioService : BaseService
    {
        [SerializeField] private AudioSource audioSourcePrefab;
        
        [SerializeField] private List<Wormwood.Utils.KeyValuePair<string, AudioDefinitionSo>> audioList;
        
        private Dictionary<string, AudioDefinitionSo> _audioDictionary = new Dictionary<string, AudioDefinitionSo>();
        
        private GenericPool<AudioSource> _audioSourcePool;
        
        private void Start()
        {
            InitAudioDictionary();
            InitPool();
        }
        
        protected override void SubscribeToService()
        {
            Logger.Log("AudioService subscribed");
            
            MessageBus.AddListener<string>(AudioServiceMessages.RequestPlayOneShot, PlayOneShot );
            MessageBus.AddListener<string>(AudioServiceMessages.RequestPlayLoop, PlayLoop);
        }

        protected override void UnsubscribeFromService()
        {
            Logger.Log("AudioService unsubscribed");
            
            MessageBus.RemoveListener<string>(AudioServiceMessages.RequestPlayOneShot, PlayOneShot );
            MessageBus.RemoveListener<string>(AudioServiceMessages.RequestPlayLoop, PlayLoop);
        }

        private void PlayOneShot( string id)
        {
            Logger.Log($"Playing one shot sound {id}");

            if (_audioSourcePool == null)
            {
                Logger.Log("AudioService audio source pool is null");
                return;
            }

            if (!_audioDictionary.ContainsKey(id))
            {
                Logger.Log($"AudioService clip not found {id}");
                return;
            }

            //TODO Pull audioSource from the audio source pool
            //Assign the audio clip to it with config and the play it
            var audioSource = _audioSourcePool.Get();
            
            audioSource.clip = _audioDictionary[id.Trim()].Clip;
            audioSource.Play();
            
            Logger.Log($"Played one shot sound {id}");


        }

        private void PlayLoop(string id)
        {
            Logger.Log($"Playing loop {id}");
        }


        private void InitAudioDictionary()
        {
            foreach (var audioDefinition in audioList)
            {
                Logger.Log($"Initializing audio definition {audioDefinition.Key}");
                _audioDictionary.Add(audioDefinition.Key, audioDefinition.Value);
            }
            
            Logger.Log($"Initialized audio definitions {_audioDictionary.Count} {_audioDictionary.Keys }" );
        }
        
        private void InitPool()
        {
            if (audioSourcePrefab == null)
            {
                Logger.Log("No audio source prefab assigned");
                return;
            }
            
            _audioSourcePool = new GenericPool<AudioSource>(createFunc: () => {
                    var go = Instantiate(audioSourcePrefab, transform);
                    return go;
                },
                onGet: source => source.gameObject.SetActive(true),
                onRelease: source => {
                    source.Stop();
                    source.clip = null;
                    source.gameObject.SetActive(false);
                },
                preLoadCount: 10,
                maxSize: 50);
        }
    }

    public static class AudioServiceMessages
    {
        public const string RequestPlayOneShot = "RequestPlayOneShot";
        public const string RequestPlayLoop = "RequestPlayLoop";
    }
}
