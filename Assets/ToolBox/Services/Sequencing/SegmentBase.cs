using System.Collections;
using UnityEngine;

namespace ToolBox.Services.Sequencing
{
    public abstract class SegmentBase : MonoBehaviour, ISegment
    {
        private Coroutine _sequenceCr;
        
        protected abstract IEnumerator SequenceCr();
        
        
        public IEnumerator StartSegment()
        {
            _sequenceCr  = StartCoroutine( SequenceCr() );
            
            yield return _sequenceCr;
        }

        public void StopSegment()
        {
            if( _sequenceCr == null ) return;
            
            StopCoroutine( _sequenceCr );
            
            _sequenceCr = null;
        }
    }
}