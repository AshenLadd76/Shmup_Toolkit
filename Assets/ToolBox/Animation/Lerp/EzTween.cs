using ToolBox.Messaging;
using UnityEngine;

namespace ToolBox.Animation.Lerp
{
    public static class EzTween 
    {
        public static LerpDescriptor Move(Transform target, Vector3 start, Vector3 end, float duration ) => CreateTweenDescriptor(target, target.position, end, duration);
        
        //Generic helper class used by all facades
        private static LerpDescriptor CreateTweenDescriptor(Transform target, Vector3 start, Vector3 end, float duration)
        {
            var lerpDescriptor = new LerpDescriptor(start, end, duration)
            {
                Target = target
            };
           
            AddLerpToActiveQueue( lerpDescriptor );
           
           return lerpDescriptor;
        }

        private static void AddLerpToActiveQueue(LerpDescriptor lerpDescriptor)
        {
            MessageBus.Broadcast( ExTweenStrings.AddLerpToActiveQueue, lerpDescriptor );
        }
    }
}
