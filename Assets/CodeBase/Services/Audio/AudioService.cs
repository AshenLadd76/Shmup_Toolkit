using System.Collections.Generic;
using ToolBox.Helpers;
using ToolBox.Messaging;
using ToolBox.Services;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Services.Audio
{
    public class AudioService : BaseService
    {
        [SerializeField, Header("Audio Clips"), Space(20)] private List<Wormwood.Utils.KeyValuePair<string, AudioDefinitionSo>> audioList;
        
        private  Dictionary<string, IAudioDefinition> _audioDictionary = new();
        
        private AudioRouter _audioRouter;
        
        protected override void Awake()
        {
            base.Awake();
            
            InitAudioDictionary();
       
            _audioRouter = new AudioRouter(new CoroutineRunner(this), transform);
        }
        
        protected override void SubscribeToService() => MessageBus.AddListener<AudioRequest>( AudioServiceMessages.RequestAudio, RequestAudio );
        
        protected override void UnsubscribeFromService() => MessageBus.RemoveListener<AudioRequest>( AudioServiceMessages.RequestAudio, RequestAudio );

        private void InitAudioDictionary()
        {
            _audioDictionary.Clear();
            
            foreach (var audioDefinition in audioList)
            {
                var formattedKey = AudioDefinitionResolver.FormatID(audioDefinition.Key);

                if (_audioDictionary.ContainsKey(formattedKey))
                {
                    Logger.LogWarning( $"Duplicate key: {formattedKey} skipped." );
                    continue;
                }

                _audioDictionary[formattedKey] = audioDefinition.Value;
            }
            
            onFinishedInitialisation?.Invoke();
        }
    
        
        private void RequestAudio(AudioRequest audioRequest)
        {
            var id  = audioRequest.AudioKey;
            
            if(!AudioDefinitionResolver.TryResolveAudio(id, _audioDictionary, out var audioDefinition)) return;

            if (audioDefinition == null)
            {
                Logger.LogError( $"No audio definition found for {audioRequest.AudioKey}" );
                return;
            }
            
            _audioRouter.ExecuteAudioRequest(audioRequest, audioDefinition);
        }
    }
}
