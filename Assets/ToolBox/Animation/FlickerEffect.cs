using System;
using System.Collections;
using ToolBox.Extensions;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace ToolBox.Animation
{
    public class FlickerEffect : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] spriteRenderers;
        [SerializeField] private float interval = 0.1f;
        
        private Coroutine _flickerCoroutine;

        private WaitForSeconds _flickerDelay;


        private void OnDisable()
        {
            StopFlicker();
        }
        
        private void Awake()
        {
            if (spriteRenderers.IsNullOrEmpty())
            {
                Logger.LogError( $"No sprite renderers found..set some in the inspector" );
                return;
            }
        }

        private void Start()
        {
            _flickerDelay = new WaitForSeconds( interval );
            
            StartFlicker();
        }


        /// <summary>
        /// Starts flickering all sprites for a duration.
        /// </summary>
        /// <param name="duration">Total time to flicker.</param>
        /// <param name="interval">Time between on/off toggles.</param>
        public void StartFlicker()
        {
            if (_flickerCoroutine != null) return;
            
           _flickerCoroutine = StartCoroutine(FlickerCoroutine());
        }

        public void StopFlicker()
        {
            if (_flickerCoroutine == null) return;
            
            StopCoroutine( _flickerCoroutine );
            
            ToggleSpriteVisibility(true);
            
            _flickerCoroutine = null;
        }

        private IEnumerator FlickerCoroutine()
        {
            if (spriteRenderers == null || spriteRenderers.Length == 0) yield break;
            
            while (true)
            {
                ToggleSpriteVisibility(false);

                yield return _flickerDelay;
                
                ToggleSpriteVisibility( true );
                
                yield return _flickerDelay;
            }
        }

        private void ToggleSpriteVisibility(bool b)
        {
            foreach (var spriteRenderer in spriteRenderers)
                if (spriteRenderer) spriteRenderer.enabled = b;
        }
    }
}