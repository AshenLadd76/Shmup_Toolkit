using UnityEngine;

namespace ToolBox.Utils.Animation
{
    public static class AnimatorUtils
    {
        /// <summary>
        /// Gets the length (in seconds) of the currently playing animation on the given Animator layer.
        /// Accounts for playback speed.
        /// </summary>
        public static float GetCurrentAnimationLength(Animator animator, int layer = 0)
        {
            if (animator == null) return 0f;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

            // Duration adjusted for speed
            return stateInfo.length / Mathf.Abs(stateInfo.speed);
        }
    }
}