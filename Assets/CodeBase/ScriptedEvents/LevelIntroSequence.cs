using System.Collections;
using UnityEngine;

namespace CodeBase.ScriptedEvents
{
    public class LevelIntroSequence : MonoBehaviour
    {
        private Coroutine _levelIntroSequence;
        
        private void Start() => StartIntroSequence();
        
        private void StartIntroSequence()
        {
            if (_levelIntroSequence != null) return;
            
            _levelIntroSequence = StartCoroutine(IntroSequence());
        }

        private void StopIntroSequence()
        {
            if( _levelIntroSequence == null ) return;
            
            StopCoroutine(_levelIntroSequence);
            
            _levelIntroSequence = null;
        }
        
        private IEnumerator IntroSequence()
        {
            yield return null;
            
            //Play overlay dissolve effect
            
            
            
            //trigger scrolling back ground
            
            //Load mother ship
            
            //move forward and back slightly
            
            //Play mother ship sequence
        }
    }
}
