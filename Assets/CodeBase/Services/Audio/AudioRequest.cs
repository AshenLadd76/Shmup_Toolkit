using UnityEngine;

namespace CodeBase.Services.Audio
{
    public struct AudioRequest
    {
        public AudioRequest(string key, AudioCommand audioCommand, Object owner = null, Vector3 position = default)
        {
            AudioKey = key;
            AudioCommand = audioCommand;
            Owner = owner;
            Position = position;
        }
        
        public string AudioKey { get; set; }
        
        public AudioCommand AudioCommand { get; set; }
        public Object Owner { get; set; }

        public Vector3 Position { get; set; }
    }
}