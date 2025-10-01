using System.Collections.Generic;
using Sirenix.OdinInspector;
using ToolBox.Messenger;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public class CollisionDetector : MonoBehaviour
    {
        [SerializeField] private Transform parentTransform;
        
        [SerializeField] private bool disableCollisionDetection = true;
        
        [SerializeField] private bool showGrid;
        
        [SerializeField] private float cellSize = 1f;
        
        [SerializeField] private Vector2 gridOffset;
        
        [SerializeField] private Vector2 _initialGridOrigin;
        [SerializeField] private Vector2 _currentGridOrigin;

        [SerializeField] private List<MonoBehaviour> collisionObjects;
        
        [SerializeField] private BaseCollisionAlgorithmSo collisionAlgorithmSo;
        
        [ShowInInspector] private List<ICollisionObject> _iCollisionObjectsList;
        
        private ISpatialPartitioningSystem _spatialPartitioningSystem;
        
        private ICollisionDetectionSystem _collisionDetectionSystem;
        
        private Camera _mainCamera;
        
        private Vector2Int _gridSize;
        
        private MessageBus _messageBus;
        
        private void OnEnable()
        {
            _messageBus.AddListener<MonoBehaviour>( CollisionDetectorMessages.AddToCollisionObject.ToString(), AddToCollisionObjectsList );
            _messageBus.AddListener<MonoBehaviour>( CollisionDetectorMessages.RemoveCollisionObject.ToString(), RemoveFromCollisionObjectsList );
        }


        private void OnDisable()
        {
            _messageBus.RemoveListener<MonoBehaviour>(CollisionDetectorMessages.AddToCollisionObject.ToString(), AddToCollisionObjectsList );
            _messageBus.RemoveListener<MonoBehaviour>(CollisionDetectorMessages.RemoveCollisionObject.ToString(), RemoveFromCollisionObjectsList );
        } 
        
        private void Awake()
        {
            LoadICollisionObjectsList();
            
            _messageBus = MessageBus.Instance;
        }
        
        private void Start()
        {
            _mainCamera = Camera.main;

            _initialGridOrigin = new Vector2(0, 0) + gridOffset;

            _gridSize = GridUtility.GetCellCountWorldUnits(_mainCamera,  cellSize);

            _spatialPartitioningSystem = new SpatialPartitioningSystem(_gridSize);
            
            _collisionDetectionSystem = new CollisionDetectionSystem(_spatialPartitioningSystem, _iCollisionObjectsList, _initialGridOrigin, cellSize, collisionAlgorithmSo );
        }


        public void UpdateCheck(ICollisionObject[] spatialObjects)
        {
            _currentGridOrigin = _initialGridOrigin + (Vector2)parentTransform.position;
            
            _spatialPartitioningSystem.UpdateCheck(spatialObjects, _currentGridOrigin, cellSize);
            
            _collisionDetectionSystem?.CollisionCheck(_currentGridOrigin);
        }

        
        public void AddToCollisionCheckGridCell(ICollisionObject projectile)
        {
            if(disableCollisionDetection ) return;
            
            var cellPosition = GridUtility.GetCellFromWorldPosition(projectile.GetPosition(), _currentGridOrigin, cellSize);
            
            projectile.LastCellPosition = cellPosition;

            _spatialPartitioningSystem?.AddToSpatialPartitionGrid(projectile);
        }
        
        
        public void RemoveFromCollisionCheck(ICollisionObject projectile, Vector2Int cellPosition)
        {
            if(disableCollisionDetection ) return;
            
            _spatialPartitioningSystem?.RemoveFromSpatialPartitionGrid(projectile, cellPosition);
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
        
        private void OnDrawGizmos()
        {
            if (!showGrid) return;
            
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    Vector2 pos = _currentGridOrigin + new Vector2(x * cellSize, y * cellSize);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(pos + Vector2.one * cellSize * 0.5f, Vector3.one * cellSize);
                }
            }
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
    }

    public enum CollisionDetectorMessages
    {
        AddToCollisionObject,
        RemoveCollisionObject,
    }
}
