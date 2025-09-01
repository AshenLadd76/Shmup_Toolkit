using System;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace ToolBox.Animation.Lerp
{
    [Serializable]
    public class LerpDescriptor
    {
        public LerpDescriptor(Vector3 start, Vector3 end, float duration, EaseType easeType = EaseType.Linear,
            Action onCompleteCallback = null)
        {
            StartPosition = start;
            EndPosition = end;
            Duration = duration;
            EaseType = easeType;
            InvertedDuration = duration > 0 ? 1 / duration : 0f;
            OnComplete = onCompleteCallback;
            IsActive = true;
        }
        
        public Transform Target { get; set; }

        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public float Duration { get; set; }
        
        public float InvertedDuration { get; set; }

        public float ElapsedTime { get; set; }
        
        public float Delay { get; set; }

        public float T;

        public EaseType EaseType { get; set; } = EaseType.Linear;

        public Action OnComplete { get; set; }

        public bool IsActive { get; set; }
        public LerpDescriptor NextLerpDescriptor { get; set; }
        
        public bool IsPingPong { get; set; }
        private int _loopCount = 0;
        
        
        //Chainable Methods 
        public LerpDescriptor SetEase(EaseType easeType)
        {
            EaseType = easeType;
            return this;
        }

        public LerpDescriptor SetDelay(float delay)
        {
            Delay = delay;
            return this;
        }
        
        public LerpDescriptor SetOnComplete(Action callback)
        {
            OnComplete = callback;
            return this;
        }

        public LerpDescriptor Then(LerpDescriptor next)
        {
            next.IsActive = false;
            NextLerpDescriptor = next;
            
            return this;
        }

        public LerpDescriptor SetPingPong(int loopCount)
        {
            IsPingPong = true;
            _loopCount = loopCount;
            
            return this;
        }


        private float _elapsedTime = 0;
        private bool _isForward = true;
        private int _completedLoops = 0;
        private float _pingPongTime;

        public float HandlePingPong(float deltaTime)
        {
            if (!IsPingPong) return Mathf.Clamp01(ElapsedTime * InvertedDuration);

            // Increment local ping-pong timer
            if (_isForward)
                _pingPongTime += deltaTime;
            else
                _pingPongTime -= deltaTime; 

            float t = _isForward ? Mathf.Clamp01(_pingPongTime / Duration)
                : 1f - Mathf.Clamp01(_pingPongTime / Duration);

            if (_isForward && _pingPongTime >= Duration)
            {
                _isForward = false;
                _pingPongTime = Duration; // clamp for backward
            }
            else if (!_isForward && _pingPongTime <= 0)
            {
                _isForward = true;
                _pingPongTime = 0; // reset for forward
                _completedLoops++;
                if (_loopCount != -1 && _completedLoops >= _loopCount)
                    IsPingPong = false;
            }

            return t;
        }

        
        public void StartTween(Transform target)
        {
            Target = target;
            ElapsedTime = 0f;
            IsActive = true;
        }
        
        public bool UpdateInternal(float deltaTime)
        {
            if (!IsActive) return true;

            ElapsedTime += deltaTime;
            float t = Mathf.Clamp01(ElapsedTime / Duration);

            Target.position = LerpCore.Lerp(StartPosition, EndPosition, t, EaseType);

            if (ElapsedTime >= Duration)
            {
                IsActive = false;
                OnComplete?.Invoke();

                return true;
            }

            return false;
        }
        
       
    }
}
