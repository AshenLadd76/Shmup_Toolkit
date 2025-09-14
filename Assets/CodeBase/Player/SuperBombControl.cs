using System.Collections;
using ToolBox.Helpers;
using ToolBox.Messenger;
using ToolBox.Utils.Validation;
using UnityEngine;

namespace CodeBase.Player
{
    public class SuperBombControl : MonoBehaviour
    {
        [SerializeField] private int bombCount;
        [SerializeField] private float shieldBuffer = 1f;
        [Validate, SerializeField] private GameObject bombShield;
        
        [Validate] private Animator _animator;
        [Validate] private Transform _transform;
        [Validate] private AnimatorClipHelper _animatorClipHelper;
        
        private WaitForSeconds _bombLifeSpan;
        private Coroutine _detonateBombCoroutine;
        private const string SuperBombAnimationName = "SuperBomb";
        private int _superBombAnimationHash;

        private void Awake()
        {
            _transform = transform;

            _animator = GetComponent<Animator>();
            
            _animatorClipHelper = new AnimatorClipHelper(_animator);
            
            ObjectValidator.Validate(this, this, true);
            
            _bombLifeSpan = new WaitForSeconds( _animatorClipHelper.GetClipLength( SuperBombAnimationName) + shieldBuffer);

            _superBombAnimationHash = Animator.StringToHash(SuperBombAnimationName);
        }

        public void DetonateBomb(float positionX)
        {
            if (bombCount <= 0) return;
            
            if (_detonateBombCoroutine != null) return;

            bombCount--;
            
            SetXPosition(positionX);

            _detonateBombCoroutine = StartCoroutine(DetonateBombCoroutine());
        }

        private IEnumerator DetonateBombCoroutine()
        {
            if (!_animator || !bombShield) yield break;
            
            _animator.SetTrigger( _superBombAnimationHash );
            
            MessageBus.Instance.Broadcast( SuperBombAnimationName );
         
            bombShield.SetActive(true);
            
            yield return _bombLifeSpan;
            
            bombShield.SetActive(false);
            
            _detonateBombCoroutine = null;
        }

        private void SetXPosition(float positionX) => _transform.position = new Vector3(positionX, _transform.position.y, _transform.position.z);
    }
}
