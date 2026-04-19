using System.Collections.Generic;
using ToolBox.Helpers;
using ToolBox.Messaging;
using ToolBox.Services;
using UnityEngine;

namespace CodeBase.Services.Audio
{
    public class AudioService : BaseService
    {
        [SerializeField, Header("Audio Clips"), Space(20)] private List<Wormwood.Utils.KeyValuePair<string, AudioDefinitionSo>> audioList;
        
        private readonly Dictionary<string, IAudioDefinition> _audioDictionary = new();
        
        private AudioRouter _audioRouter;
        
        private void Awake()
        {
            InitAudioDictionary();
       
            _audioRouter = new AudioRouter(new CoroutineRunner(this), transform);
        }
        
        protected override void SubscribeToService() => MessageBus.AddListener<AudioRequest>( AudioServiceMessages.RequestAudio, RequestAudio );
        
        protected override void UnsubscribeFromService() => MessageBus.RemoveListener<AudioRequest>( AudioServiceMessages.RequestAudio, RequestAudio );
        
        private void InitAudioDictionary()
        {
            foreach (var audioDefinition in audioList)
                _audioDictionary[AudioDefinitionResolver.FormatID(audioDefinition.Key)] = audioDefinition.Value;
        }
        
        private void RequestAudio(AudioRequest audioRequest)
        {
            var id  = audioRequest.AudioKey;
            
            if(!AudioDefinitionResolver.TryResolveAudio(id, _audioDictionary, out var key,out var audioDefinition)) return;
            
            _audioRouter.ExecuteAudioRequest(audioRequest, audioDefinition);
        }
    }
}
