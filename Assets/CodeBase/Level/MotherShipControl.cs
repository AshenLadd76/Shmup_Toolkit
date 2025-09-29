using System.Collections;
using ToolBox.Services.Sequencing;
using ToolBox.Utils.Animation;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Level
{
   
    [RequireComponent(typeof(Animator))]
    public class MotherShipControl : SegmentBase
    {
        private Animator _animator;
        
        private Coroutine _sequenceCr;
        
        
        private int _accelerateTriggerHash = Animator.StringToHash("Accelerate");
        private int _decelerateTriggerHash = Animator.StringToHash("Decelerate");
        private int _unloadShipTriggerHash = Animator.StringToHash("UnloadShip");


        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        protected override IEnumerator SequenceCr()
        {
            if (!_animator)
            {
                Logger.LogWarning("Animator not assigned on MotherShipControl");
                yield break;
            }
            
            _animator.SetTrigger(_accelerateTriggerHash);
            
            yield return new WaitForSeconds(AnimatorUtils.GetCurrentAnimationLength( _animator, 0 ));

            yield return null;
            
            yield return new WaitForSeconds( 2f );
            
            _animator.SetTrigger(_decelerateTriggerHash);
            
            yield return new WaitForSeconds( AnimatorUtils.GetCurrentAnimationLength( _animator, 0 ) );
            
            yield return null;
        }
    }
}
