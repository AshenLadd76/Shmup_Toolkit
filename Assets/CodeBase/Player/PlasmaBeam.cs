using System.Collections.Generic;
using CodeBase.Collision_Handling;
using ToolBox.Messaging;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

namespace CodeBase.Player
{
    public class PlasmaBeam : MonoBehaviour
    {
        [SerializeField] private PlasmaBeamTypeSo plasmaBeamTypeSo;
        
        [SerializeField] private Vector2 anchor;

        [SerializeField] private Vector2 targetPosition;
        
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [SerializeField] private float beamLength;

        [SerializeField] private List<MonoBehaviour> collisionObjects = new();
        
        [SerializeField] private BaseCollisionAlgorithmSo collisionAlgorithmSo;
        
        private List<ICollisionObject> _iCollisionObjectsList = new();
        
        private bool _isBeamActive;
        
        private ICollisionObject _lastHitObject;
        
        private float _beamWidth;
        private float _lastCollisionDistance;
        private int _framesSinceLastHit = 0;
        
        private const int MaxFramesNoHit = 3; // allow a few frames before retracting


        private Vector2 _beamDirection;
        
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
            if (plasmaBeamTypeSo == null)
            {
                Logger.LogError( "PlasmaBeamTypeSo is null" );
                return;
            }
            
            spriteRenderer.enabled = false;
            
           // _beamWidth = spriteRenderer.bounds.size.x - plasmaBeamTypeSo.BeamWidthOffset;
           
           _beamWidth = spriteRenderer.size.x - plasmaBeamTypeSo.BeamWidthOffset;
            
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
            
            if( collisionObjects.Contains( collisionObject ) ) return;

            if (collisionObject is not ICollisionObject iobject) return;
            
            collisionObjects.Add(collisionObject);
            _iCollisionObjectsList.Add(iobject);
        }
        
        private void RemoveFromCollisionObjectsList(MonoBehaviour collisionObject)
        {
            if (collisionObject == null) return;
            
            if( !collisionObjects.Contains( collisionObject ) ) return;
            
            if (collisionObject is not ICollisionObject iCollisionObject) return;
            
            _iCollisionObjectsList.Remove(iCollisionObject);
            collisionObjects.Remove( collisionObject );
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
        
        
        //Direction agnostic collision check
        private void CheckForCollision()
        {
            _beamDirection = transform.up.normalized;

            float closestDistance = plasmaBeamTypeSo.DefaultBeamLength;
            ICollisionObject closestCollisionObject = null;

            Vector3 origin = transform.position;

            for (var i = 0; i < _iCollisionObjectsList.Count; i++)
            {
                var obj = _iCollisionObjectsList[i];
                
                // Use the reusable collision method
                bool hit = OBB2DCollision.CheckCollision(
                    centerA: (Vector2)transform.position,
                    halfSizeA: new Vector2(_beamWidth * 0.5f, beamLength * 0.5f),
                    directionA: _beamDirection,
                    centerB: obj.Position,
                    halfSizeB: new Vector2(obj.RadiusX, obj.RadiusY)
                );

                if (!hit) continue;
                

                // ✅ Apply damage (time-based)
                obj.Damage(plasmaBeamTypeSo.DamagePerSecond * Time.deltaTime);

                float forward = Vector2.Dot(obj.Position - transform.position, _beamDirection);
                
                // Track closest object along beam
                if (forward < closestDistance)
                {
                    closestDistance = forward;
                    closestCollisionObject = obj;
                }
            }

            FlickerPrevention(closestCollisionObject, closestDistance);

            SmoothlyAdjustBeamLength();
        }
        
        
        private void FlickerPrevention(ICollisionObject closestCollisionObject, float closestDistance )
        {
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
                    float predictedDistance = Vector2.Dot(_lastHitObject.Position - transform.position, _beamDirection);
                    _lastCollisionDistance = predictedDistance;
                }
                else
                {
                    _lastHitObject = null;
                    _lastCollisionDistance = plasmaBeamTypeSo.DefaultBeamLength;
                }
            }
        }

        private void SmoothlyAdjustBeamLength()
        {
            // Smoothly grow/shrink the beam
            float speed = beamLength < _lastCollisionDistance ? plasmaBeamTypeSo.GrowSpeed : plasmaBeamTypeSo.ShrinkSpeed;
            
            beamLength = Mathf.Lerp(beamLength, _lastCollisionDistance, Time.deltaTime * speed);
        }
        
        private bool CheckCollision(Vector2 centreA, Vector2 halfSizeA, Vector2 centreB, Vector2 halfSizeB)
        {
            return Mathf.Abs(centreA.x - centreB.x) <= halfSizeA.x + halfSizeB.x && Mathf.Abs(centreA.y - centreB.y) <= halfSizeA.y + halfSizeB.y;
        }
        
        private void OnDrawGizmos()
        {
            if (!_isBeamActive) return;
            
            if (spriteRenderer == null) return;

            Bounds bounds = spriteRenderer.bounds;
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;

            Gizmos.color =Color.green;

            // Draw rectangle in XY plane
            Vector3 topLeft = new Vector3(center.x - size.x / 2, center.y + size.y / 2, 0);
            Vector3 topRight = new Vector3(center.x + size.x / 2, center.y + size.y / 2, 0);
            Vector3 bottomRight = new Vector3(center.x + size.x / 2, center.y - size.y / 2, 0);
            Vector3 bottomLeft = new Vector3(center.x - size.x / 2, center.y - size.y / 2, 0);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
    
    public static class OBB2DCollision
    {
        // Checks if a rectangle (center, halfSize, rotation) collides with another
        public static bool CheckCollision(Vector2 centerA, Vector2 halfSizeA, Vector2 directionA,
            Vector2 centerB, Vector2 halfSizeB)
        {
            // project along A’s forward and right (perpendicular) axes
            Vector2 forward = directionA.normalized;
            Vector2 right = new Vector2(forward.y, -forward.x);

            Vector2 toB = centerB - centerA;

            float forwardDist = Vector2.Dot(toB, forward);
            float sidewaysDist = Vector2.Dot(toB, right);

            float projectedRadius = Mathf.Abs(right.x) * halfSizeB.x + Mathf.Abs(right.y) * halfSizeB.y;

            return Mathf.Abs(sidewaysDist) <= (halfSizeA.x + projectedRadius)
                   && forwardDist >= 0
                   && forwardDist <= halfSizeA.y * 2; // beam length
        }
    }
 }
