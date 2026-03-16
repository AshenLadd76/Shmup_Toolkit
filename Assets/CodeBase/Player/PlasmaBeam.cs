using System.Collections.Generic;
using CodeBase.Collision_Handling;
using ToolBox.Messaging;
using UnityEngine;

namespace CodeBase.Player
{
    public class PlasmaBeam : MonoBehaviour
    {
        [SerializeField] private Vector2 anchor;

        [SerializeField] private Vector2 targetPosition;
        
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [SerializeField] private float beamLength;

        [SerializeField] private List<MonoBehaviour> collisionObjects = new();

        private List<ICollisionObject> _iCollisionObjectsList = new();
        
        [SerializeField] private BaseCollisionAlgorithmSo collisionAlgorithmSo;
        
        private readonly ICollisionAlgorithm _collisionAlgorithm;

        [SerializeField] private float growSpeed = 8;
        
        [SerializeField] private float shrinkSpeed = 40;
        
        [SerializeField] private float defaultBeamLength = 30;
        
        private bool _isBeamActive;
        
        private float _beamWidth;


        
        private ICollisionObject _lastHitObject;
        private float _lastCollisionDistance;
        private int _framesSinceLastHit = 0;
        private const int MaxFramesNoHit = 3; // allow a few frames before retracting

        
        private void OnEnable()
        {
            MessageBus.AddListener<MonoBehaviour>( CollisionDetectorMessages.AddToCollisionObject.ToString(), AddToCollisionObjectsList );
            MessageBus.AddListener<MonoBehaviour>( CollisionDetectorMessages.RemoveCollisionObject.ToString(), RemoveFromCollisionObjectsList );
        }
        
        private void OnDisable()
        {
            MessageBus.RemoveListener<MonoBehaviour>(CollisionDetectorMessages.AddToCollisionObject.ToString(), AddToCollisionObjectsList );
            MessageBus.RemoveListener<MonoBehaviour>(CollisionDetectorMessages.RemoveCollisionObject.ToString(), RemoveFromCollisionObjectsList );
        } 
        
        private void Awake()
        {
            spriteRenderer.enabled = false;
            
            _beamWidth = spriteRenderer.bounds.size.x;
            
            LoadICollisionObjectsList();
        }

        private void Start() => SetBeamHeight( beamLength );
        

        private void Update()
        {
            if( Input.GetKeyDown( KeyCode.Space ) ) 
                FireBeam();
            
            if( Input.GetKeyUp(KeyCode.Space ) ) 
                HideBeam();

            if (!_isBeamActive) return;
            
            CheckForCollision();
            
            SetBeamHeight(beamLength);
        }

        
        private void AddToCollisionObjectsList(MonoBehaviour collisionObject)
        {
            if (collisionObject == null) return;
            
            collisionObjects.Add(collisionObject);

            if (collisionObject is ICollisionObject iobject)
                _iCollisionObjectsList.Add(iobject);
        }
        
        private void RemoveFromCollisionObjectsList(MonoBehaviour collisionObject)
        {
            if (collisionObject == null) return;
            
            collisionObjects.Remove( collisionObject );

            if (collisionObject is ICollisionObject iCollisionObject)
                _iCollisionObjectsList.Remove(iCollisionObject);
        }
        
        private void SetBeamHeight(float height)
        {
            var yHeight = height - anchor.y;

            var tempSize = spriteRenderer.size;
            
            tempSize.y = yHeight; 
            
            spriteRenderer.size = tempSize;
        }

        private void FireBeam()
        {
            spriteRenderer.enabled = true;
            _isBeamActive = true;
        }

        private void HideBeam()
        {
            spriteRenderer.enabled = false;
            _isBeamActive = false;
        }

        private void LoadICollisionObjectsList()
        {
            _iCollisionObjectsList = new List<ICollisionObject>();

            foreach (var collisionObject in collisionObjects   )
            {
                ICollisionObject iCollisionObject  = collisionObject as ICollisionObject;

                if (iCollisionObject == null) continue;
                
                _iCollisionObjectsList.Add(iCollisionObject);
            }
        }
        
        private void CheckForCollision()
        {
            const float Half = 0.5f;
            float closestDistance = defaultBeamLength;
            
            ICollisionObject closestCollisionObject = null;
            
            Vector2 halfSize = new Vector2(_beamWidth * Half, beamLength * Half);
            
            Vector2 beamCenter = (Vector2)transform.position + new Vector2(0, beamLength * Half);

            for (var i = 0; i < _iCollisionObjectsList.Count; i++)
            {
                var obj = _iCollisionObjectsList[i];
                
                Vector2 objHalfSize = new Vector2(obj.RadiusX, obj.RadiusY);

                if (!CheckCollision(beamCenter, halfSize, obj.Position, objHalfSize))
                    continue;

                // Trigger collision event
                obj.OnCollision();

                // Compute distance from beam origin to top of the object
                float collisionDistance = obj.Position.y - transform.position.y - obj.RadiusY;

                // Keep track of the closest collision
                if (collisionDistance < closestDistance)
                {
                    closestDistance = collisionDistance;
                    closestCollisionObject = obj;
                }
            }


            if (closestCollisionObject != null)
            {
                //Hit something this frame
                _lastHitObject = closestCollisionObject;
                _lastCollisionDistance = closestDistance;
                _framesSinceLastHit = 0;
            }
            else if (_lastHitObject != null)
            {
                //No collision detected, but we have a lst hit object
                _framesSinceLastHit++;

                if (_framesSinceLastHit <= MaxFramesNoHit)
                {
                    //Track last object's current position
                    float predictedDistance = _lastHitObject.Position.y - transform.position.y + _lastHitObject.RadiusY;
                    _lastCollisionDistance = predictedDistance; 
                }
                else
                {
                    _lastHitObject = null;
                    _lastCollisionDistance = defaultBeamLength;
                }
            }
            
            // Smoothly grow/shrink the beam
            float speed = beamLength < _lastCollisionDistance ? growSpeed : shrinkSpeed;
            
            beamLength = Mathf.Lerp(beamLength, _lastCollisionDistance, Time.deltaTime * speed);
        }
        
        
        private bool CheckCollision(Vector2 centreA, Vector2 halfSizeA, Vector2 centreB, Vector2 halfSizeB)
        {
            return Mathf.Abs(centreA.x - centreB.x) <= halfSizeA.x + halfSizeB.x &&
                   Mathf.Abs(centreA.y - centreB.y) <= halfSizeA.y + halfSizeB.y;
        }
    }
}
