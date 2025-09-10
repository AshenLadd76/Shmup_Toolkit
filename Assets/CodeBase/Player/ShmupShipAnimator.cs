using System;
using UnityEngine;

namespace CodeBase.Player
{
    public class ShmupShipAnimator
    {
        private readonly Animator _animator;
        private readonly PlayerMovementData _movementData;
        
        private Vector2Int _lastDirection;
        private int _currentAnimationHash;
        private float _tiltTime;
        
        private readonly int _tiltField = Animator.StringToHash("TiltTime");
        
        public ShmupShipAnimator(Animator animator, PlayerMovementData movementData)
        {
            _animator = animator ?? throw new ArgumentNullException(nameof(animator), "Animator cannot be null.");
            _movementData = movementData ?? throw new ArgumentNullException(nameof(movementData), "PlayerMovementData cannot be null.");
        }
        
        public void UpdateAnimator(Vector2 direction)
        {
            Vector2Int newDirection = Vector2Int.RoundToInt(direction);

            if (newDirection != _lastDirection)
            {
                _lastDirection = newDirection;
                _currentAnimationHash = _movementData.GetAnimationHash(newDirection);
                _animator.CrossFade(_currentAnimationHash, 0.05f);
            }

            _tiltTime = (newDirection.x == 0) ? 0f : _tiltTime + Time.deltaTime;
            _animator.SetFloat(_tiltField, _tiltTime);
        }
    }
}