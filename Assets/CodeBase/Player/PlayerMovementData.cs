using System;
using System.Collections.Generic;
using ToolBox.Extensions;
using UnityEngine;

namespace CodeBase.Player
{
    [CreateAssetMenu(fileName = "PlayerMovementSet", menuName = "Shmup/Animation/PlayerMovementSet")]
    public class PlayerMovementData : ScriptableObject
    {
        [Serializable]
        public struct DirectionAnimation
        {
           [SerializeField] public Vector2Int direction;
           [SerializeField] public string animationName;
        }
        
        [SerializeField] private List<DirectionAnimation> directionAnimations = new List<DirectionAnimation>();

        private Dictionary<Vector2Int, int> _animationDictionary;

        private void OnEnable() => InitDictionary();
        
        private void InitDictionary()
        {
            if (directionAnimations.IsNullOrEmpty()) return;
            
            _animationDictionary = new Dictionary<Vector2Int, int>();

            foreach (var animation in directionAnimations)
            {
                if (_animationDictionary.ContainsKey(animation.direction)) continue;
                
                _animationDictionary.Add(animation.direction, Animator.StringToHash(animation.animationName));
            }
        }

        public int GetAnimationHash(Vector2Int key)
        {
            if(_animationDictionary.IsNullOrEmpty()) InitDictionary();
            
            return _animationDictionary.TryGetValue( key, out var animationHash ) ? animationHash : 0;
        }
    }
}
