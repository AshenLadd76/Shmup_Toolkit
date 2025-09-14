using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Helpers
{
    public class AnimatorClipHelper
    {
        private readonly Dictionary<string, AnimationClip> _clipLookup;
    
        /// <summary>
        /// Creates a helper for the given Animator.
        /// </summary>
        /// <param name="animator">Animator to cache clips from.</param>
        /// <param name="ignoreCase">Whether lookups ignore case.</param>
        public AnimatorClipHelper(Animator animator, bool ignoreCase = true)
        {
            _clipLookup = new Dictionary<string, AnimationClip>(
                ignoreCase ? System.StringComparer.OrdinalIgnoreCase : System.StringComparer.Ordinal
            );

            var clips = animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                if (!_clipLookup.ContainsKey(clip.name))
                    _clipLookup.Add(clip.name, clip);
            }
        }

        /// <summary>
        /// Try to get an animation clip by name.
        /// </summary>
        public bool TryGetClip(string clipName, out AnimationClip clip)
        {
            return _clipLookup.TryGetValue(clipName, out clip);
        }

        /// <summary>
        /// Get an animation clip by name, or null if it doesn't exist.
        /// </summary>
        public AnimationClip GetClip(string clipName)
        {
            _clipLookup.TryGetValue(clipName, out var clip);
            return clip;
        }
    
        /// <summary>
        /// Get the length of a clip by name, or 0 if not found.
        /// </summary>
        public float GetClipLength(string clipName)
        {
            if (_clipLookup.TryGetValue(clipName, out var clip))
                return clip.length;
            return 0f;
        }
    }
}