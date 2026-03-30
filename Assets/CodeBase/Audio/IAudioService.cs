using UnityEngine;

namespace CodeBase.Audio
{
    public interface IAudioService
    {
        public void PlayOneShot(string id);
        public void PlayOneShotAtPosition(string id, Vector3 position);
        public void PlayLoop(string id);
        public void StopLoop(string id);
        public void PlayLoopAtPosition(string id, Vector3 position);
    }
}