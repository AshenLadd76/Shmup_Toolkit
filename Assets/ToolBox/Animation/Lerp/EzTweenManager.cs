using ToolBox.Messenger;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Animation.Lerp
{
    public class EzTweenManager : MonoBehaviour
    {
        [SerializeField] private LerpDescriptor[] _tweensArr = new LerpDescriptor[5];
        private int _activeTweenCount = 0;

        private void OnEnable()
        {
            MessageBus.Instance.AddListener<LerpDescriptor>( ExTweenStrings.AddLerpToActiveQueue, AddLerp );
        }

        private void OnDisable()
        {
            MessageBus.Instance.RemoveListener<LerpDescriptor>( ExTweenStrings.AddLerpToActiveQueue, AddLerp);
        }
        
        private void Update()
        {
            for (int x = _activeTweenCount-1; x >= 0; x--)
            {
                LerpDescriptor lerpDesc = _tweensArr[x];
                
                if (!lerpDesc.IsActive) continue;
                
                ExecuteTween( _tweensArr[x], x );
            }
        }

        private void ExecuteTween(LerpDescriptor lerpDesc, int index)
        { 
            
            var isFinished = lerpDesc.UpdateInternal(Time.deltaTime);

            if (isFinished)
                RemoveLerp(index);
        }

        private void AddLerp(LerpDescriptor descriptor)
        {
            if (_activeTweenCount < _tweensArr.Length)
            {
                Logger.Log( $"Adding Lerp {_activeTweenCount}" );
                _tweensArr[_activeTweenCount] = descriptor;
                _activeTweenCount++;
            }
            else
            {
                Logger.LogWarning("Tween limit reached!");
            }

        }

        private void RemoveLerp(int index)
        {
            
            _tweensArr[index] = _tweensArr[_activeTweenCount - 1];
           _tweensArr[_activeTweenCount - 1] = null;
           _activeTweenCount--;
           
           Logger.Log( $"Removing Lerp {_activeTweenCount}" );
        }
    }

    public static class ExTweenStrings
    {
        public const string AddLerpToActiveQueue = "AddLerpToActiveQueue";
    }
}
