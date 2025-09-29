using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Services.Sequencing
{
    public class Sequencer : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> segmentObjects;

        private List<ISegment> _segments;
        
        private Coroutine _sequencerCoroutine;
        
        [SerializeField] private UnityEvent onSequencerFinished;

        private void Awake()
        {
            _segments = new List<ISegment>();
            foreach (var obj in segmentObjects)
            {
                var seg = obj.GetComponent<ISegment>();
                if (seg != null)
                {
                    Logger.Log($"Sequencer: {gameObject.name} found segment {seg}");
                    _segments.Add(seg);
                }
            }
        }

        private void Start()
        {
           StartSequence();
        }

        private void StartSequence()
        {
            if (_sequencerCoroutine != null) return;
            
            _sequencerCoroutine = StartCoroutine(RunSequenceCr());
        }

        private void StopSequence()
        {
            if (_sequencerCoroutine == null) return;
            
            StopCoroutine(_sequencerCoroutine);
        }

        private IEnumerator RunSequenceCr()
        {
            foreach (var seg in _segments)
            {
                if (seg == null) continue;

                yield return seg.StartSegment();
            }
            
            onSequencerFinished?.Invoke();
        }

    }
}
