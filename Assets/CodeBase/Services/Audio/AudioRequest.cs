using UnityEngine;

namespace CodeBase.Services.Audio
{
    public struct AudioRequest
    {
        public AudioRequest(string key, AudioCommand audioType, Object owner = null, Vector3 position = default)
        {
            AudioKey = key;
            AudioType = audioType;
            Owner = owner;
            Position = position;
        }
        
        public string AudioKey { get; set; }
        
        public AudioCommand AudioType { get; set; }
        public Object Owner { get; set; }

        public Vector3 Position { get; set; }
    }
}