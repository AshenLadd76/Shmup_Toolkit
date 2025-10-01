using System;
using UnityEngine;

namespace CodeBase.Level
{
    public class BackgroundScroller : MonoBehaviour
    {
        private Transform _transform;
        
        [SerializeField] private float scrollSpeed = 0.5f;
        [SerializeField] private Vector3 scrollDirection = Vector2.zero;

        [SerializeField] private Transform[] layerArray;
        
        int pixelsPerUnit = 16;

        private void Awake()
        {
            _transform = transform;
        }

        // Update is called once per frame
        private void Update()
        {
            Move();
        }

        private void Move()
        {
           _transform.position += scrollDirection * (Time.deltaTime * scrollSpeed); 
            
           //layerArray[0].position += scrollDirection * (scrollSpeed * Time.deltaTime);
           //layerArray[1].position += scrollDirection * (scrollSpeed * 0.8f * Time.deltaTime);
           
        }
    }
}
