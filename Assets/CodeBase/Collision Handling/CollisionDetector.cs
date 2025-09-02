using System.Collections.Generic;
using CodeBase.Projectile;
using Sirenix.OdinInspector;
using ToolBox.Extensions;
using UnityEngine;

namespace CodeBase.Collision_Handling
{
    public class CollisionDetector : MonoBehaviour
    {
        [SerializeField] private bool disableCollisionDetection = true;
        
        [SerializeField] private bool showGrid;
        
        [SerializeField] private float cellSize = 1f;
        
        private Vector2Int _gridSize;
        
        private Vector2 _gridOrigin;

        [SerializeField] private List<GameObject> collisionObjects;
        
        [ShowInInspector] private Dictionary<Vector2Int, HashSet<IProjectile>> _spatialPartitioningDictionary;
        
        private ISpatialPartitioningSystem _spatialPartitioningSystem;
        
        private Camera _mainCamera;
        
        private void Start()
        {
            _mainCamera = Camera.main;

            _gridOrigin = new Vector2(-2.5f, -5f);

            _gridSize = GridUtility.GetCellCountWorldUnits(_mainCamera,  cellSize);

            _spatialPartitioningSystem = new SpatialPartitioningSystem(_gridSize);
        }


        public void UpdateCheck(Projectile.Projectile[] projectiles)
        {
            if (disableCollisionDetection) return;
            
            if (projectiles.IsNullOrEmpty() ) return;
            
            for (var i = 0; i < projectiles.Length; i++)
            {
                var projectile = projectiles[i];
                
                if(!projectile)  continue;

                if(!RemoveInActiveProjectiles(projectile)) continue;
                
                Vector2Int newCellPosition = GridUtility.GetCellFromWorldPosition(projectile.GetPosition(), _gridOrigin, cellSize);
                
                if( newCellPosition == projectile.LastCellPosition ) continue;
                
                UpdateActiveObjectsPosition( projectile, newCellPosition );
            }
            
            CollisionCheck();
        }

        private void UpdateActiveObjectsPosition(ISpatialObject projectile, Vector2Int newCellPosition)
        {
            _spatialPartitioningSystem?.RemoveFromSpatialPartitionGrid(projectile, projectile.LastCellPosition);
                
            projectile.LastCellPosition = newCellPosition;
                
            _spatialPartitioningSystem?.AddToSpatialPartitionGrid(projectile);
        }

        private bool RemoveInActiveProjectiles(ISpatialObject projectile)
        {
            if (!projectile.IsActive)
            {
                Vector2Int cellPos = GridUtility.GetCellFromWorldPosition(projectile.GetPosition(), _gridOrigin, cellSize);
       
                _spatialPartitioningSystem?.RemoveFromSpatialPartitionGrid(projectile, cellPos);

                return false;
            }

            return true;
        }
        
        public void AddToCollisionCheckGridCell(ISpatialObject projectile)
        {
            if(disableCollisionDetection ) return;
            
            var cellPosition = GridUtility.GetCellFromWorldPosition(projectile.GetPosition(), _gridOrigin, cellSize);
            
            projectile.LastCellPosition = cellPosition;

            _spatialPartitioningSystem?.AddToSpatialPartitionGrid(projectile);
        }
        
        public void RemoveFromCollisionCheck(ISpatialObject projectile, Vector2Int cellPosition)
        {
            if(disableCollisionDetection ) return;
            
            _spatialPartitioningSystem?.RemoveFromSpatialPartitionGrid(projectile, cellPosition);
        }


        private ( Vector2 min, Vector2 max ) _bounds;
        private void CollisionCheck()
        {
            if (collisionObjects.IsNullOrEmpty()) return;
            
            for (int x = 0; x < collisionObjects.Count; x++)
            {
                //Fix this....
                var collisionObjectPosition = collisionObjects[x].transform.position;
                
                //Get the bounds of the current object
                var bounds = GetWorldBounds( collisionObjectPosition,  Vector2.one * 0.25f );
                
                _bounds = bounds;
                
                CheckForMultiCellCollisions(bounds, collisionObjectPosition);
            }
        }

        private Vector2Int _key;
        private void CheckForMultiCellCollisions((Vector2 min, Vector2 max) bounds, Vector3 collisionObjectPosition)
        {
            Vector2Int minCell = GridUtility.GetCellFromWorldPosition(bounds.min, _gridOrigin, cellSize);
            Vector2Int maxCell = GridUtility.GetCellFromWorldPosition(bounds.max, _gridOrigin, cellSize);
            
            int width = maxCell.x - minCell.x + 1;
            int height = maxCell.y - minCell.y + 1;
            int total  = width * height;
            
            //Loop through all overlapped cells
            for (var i = 0; i < total; i++)
            {
                int x = minCell.x + (i % width);
                int y  = minCell.y + (i / width);

                Vector2Int key = new Vector2Int( x,y );

                var cellSet = _spatialPartitioningSystem.GetCellSet(key);
                
                if(cellSet == null) continue;
                
                CheckForProjectileCollisions(cellSet , collisionObjectPosition);
            }
        }


        private HashSet<ISpatialObject> _deadProjectiles = new HashSet<ISpatialObject>();
        private void CheckForProjectileCollisions(HashSet<ISpatialObject> cellSet, Vector3 objectPosition)
        {
            
            foreach (var projectile in cellSet)
            {
                if (!projectile.IsActive) continue;

               //if(CollisionAlgorithms.LineIntersectsCircle(projectile.LastPosition, projectile.GetPosition(), objectPosition, 0.2f))
                if (CollisionAlgorithms.CircleVsCircle(objectPosition, 0.25f, projectile.GetPosition(), .1f))
                {
                    projectile.LifeSpan = 0;
                    _deadProjectiles.Add(projectile);
                }
                
                projectile.LastPosition = projectile.GetPosition();
            }
            
            DeleteDeadProjectiles(_deadProjectiles);
        }
        
        private void DeleteDeadProjectiles(HashSet<ISpatialObject> deadProjectiles)
        {
            foreach (var projectile in deadProjectiles)
            {
                projectile.LifeSpan = 0;
                projectile.LastCellPosition = new Vector2Int(int.MinValue, int.MinValue);
            }
            
            deadProjectiles.Clear();
        }
        
        private (Vector2 min, Vector2 max) GetWorldBounds(Vector2 position, Vector2 halfSize)
        {
            Vector2 min = position - halfSize;
            Vector2 max = position + halfSize;
            return (min, max);
        }
        
        private void OnDrawGizmos()
        {
            if (!showGrid) return;
            
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    Vector2 pos = _gridOrigin + new Vector2(x * cellSize, y * cellSize);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(pos + Vector2.one * cellSize * 0.5f, Vector3.one * cellSize);
                }
            }
            
            Vector2 center = (_bounds.min + _bounds.max) * 0.5f;
            Vector2 size = _bounds.max - _bounds.min;

            Gizmos.color = Color.red; // or any color you want
            Gizmos.DrawWireCube(center, size);
        }
    }
}
