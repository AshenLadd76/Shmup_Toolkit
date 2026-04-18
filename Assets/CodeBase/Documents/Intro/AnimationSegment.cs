using System;
using System.Collections;
using ToolBox.Helpers;
using UnityEngine;
using UnityEngine.Events;
using Logger = ToolBox.Utils.Logger;
using Object = UnityEngine.Object;

namespace CodeBase.Documents.Intro
{
    public interface IAnimationSegment
    {
        Coroutine PlaySegment(ICoroutineRunner coroutineRunner, Transform parent, IAnimationSegment animationSegment);
        
        public float SegmentLength { get; set; }

        public void CleanUp();

    }

    [Serializable]
    public class AnimationSegment : IAnimationSegment
    {
        [SerializeField] private bool waitForInput;
        
        [SerializeField] private GameObject segment;
        public GameObject Segment
        {
            get => segment;
            set => segment = value;
        }

        [SerializeField] private float segmentLength;
        public float SegmentLength
        {
            get => segmentLength;
            set => segmentLength = value;
        }
        
        

      //  [SerializeField] private UnityEvent onSegmentStart;
       // [SerializeField] private UnityEvent onSegmentEnd;

        private GameObject _instance;
        
        public Coroutine PlaySegment(ICoroutineRunner coroutineRunner, Transform parent, IAnimationSegment previousSegment)
        {
            return coroutineRunner.StartCoroutine(PlaySegmentCr(parent, previousSegment));
        }
        
        private IEnumerator PlaySegmentCr(Transform parent, IAnimationSegment previousSegment)
        {
            Logger.Log($"Playing segment");
            
            _instance = Object.Instantiate(Segment, parent);

            if (_instance == null)
            {
                Logger.Log($"Could not instantiate the segment");
                yield break;
            }
            
            if(!_instance.activeSelf)
                _instance.SetActive(true);
            
            
            if( waitForInput ) 
                yield return new WaitUntil(() => Input.anyKeyDown);
            else
            {
                yield return new WaitForSeconds(SegmentLength);
                
            }

            previousSegment?.CleanUp();
            
       
        }

        public void CleanUp()
        {
            if (_instance == null) return;
            
            _instance?.SetActive(false);
        }

    }
}