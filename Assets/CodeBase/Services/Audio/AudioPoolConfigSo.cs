using UnityEngine;

namespace CodeBase.Services.Audio
{
    [CreateAssetMenu(fileName = "AudioPoolConfig", menuName = "ToolBox/AudioPoolConfig")]
    public class AudioPoolConfigSo : ScriptableObject
    {
        [SerializeField] private int sfxPreloadCount = 10;
        [SerializeField] private int sfxMaxPoolSize = 50;
    
        [SerializeField] private int loopingPreloadCount = 5;
        [SerializeField] private int loopingMaxPoolSize = 20;
    
        [SerializeField] private int musicPreloadCount = 2;
        [SerializeField] private int musicMaxPoolSize = 5;
        
        [SerializeField] private float fadeTime = 3f;
    
        public int SfxPreloadCount => sfxPreloadCount;
        public int SfxMaxPoolSize => sfxMaxPoolSize;
        public int LoopingPreloadCount => loopingPreloadCount;
        public int LoopingMaxPoolSize => loopingMaxPoolSize;
        public int MusicPreloadCount => musicPreloadCount;
        public int MusicMaxPoolSize => musicMaxPoolSize;
        public float FadeTime => fadeTime;
        
        
    }
}