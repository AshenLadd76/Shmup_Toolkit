using System.Collections.Generic;
using Sirenix.OdinInspector;
using ToolBox.Extensions;
using ToolBox.Messenger;
using UnityEngine;
using Logger = ToolBox.Utils.Logger;

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

        private HashSet<Projectile.Projectile> _tempProjectiles = new();
        
        [ShowInInspector] private Dictionary<Vector2Int, HashSet<Projectile.Projectile>> _collisionGridDictionary;
        
        private Camera _mainCamera;
        
        private void Start()
        {
            _mainCamera = Camera.main;

            _gridOrigin = new Vector2(-2.5f, -5f);

            _gridSize = GridUtility.GetCellCountWorldUnits(_mainCamera,  cellSize);
            
            InitDictionary(_gridSize);
        }


        private void InitDictionary(Vector2Int gridSize)
        {
            _collisionGridDictionary = new Dictionary<Vector2Int, HashSet<Projectile.Projectile>>(gridSize.x * gridSize.y);
            
            for(int x = 0; x < gridSize.x; x++)
                for(int y = 0; y < gridSize.y; y++)
                    _collisionGridDictionary.Add(new Vector2Int(x, y), new HashSet<Projectile.Projectile>());
        }

        public void UpdateCheck(Projectile.Projectile[] projectiles)
        {
            if (disableCollisionDetection) return;
            
            if (projectiles.IsNullOrEmpty() ) return;
            
            for (var i = 0; i < projectiles.Length; i++)
            {
                var projectile = projectiles[i];
                
                if(!projectile)  continue;

                if (!projectile.IsActive)
                {
                    Vector2Int cellPos = GridUtility.GetCellFromWorldPosition(projectile.GetPosition(), _gridOrigin, cellSize);
                    //projectile.LastCellPosition = cellPos;
                    RemoveFromCollisionCheck(projectile, cellPos);
                    continue;
                }
                
                Vector2Int newCellPosition = GridUtility.GetCellFromWorldPosition(projectile.GetPosition(), _gridOrigin, cellSize);
                
                if( newCellPosition == projectile.LastCellPosition ) continue;
                
                RemoveFromCollisionCheck(projectile, projectile.LastCellPosition);
                
                AddToCollisionCheckGridCell(projectile, newCellPosition);
                
                projectile.LastCellPosition = newCellPosition;
            }
            
            CollisionCheck();
        }
        
        public void AddToCollisionCheckGridCell(Projectile.Projectile projectile)
        {
            if(disableCollisionDetection ) return;
            
            var cellPosition = GridUtility.GetCellFromWorldPosition(projectile.GetPosition(), _gridOrigin, cellSize);

            AddToCollisionCheckGridCell(projectile, cellPosition);
        }

        private void AddToCollisionCheckGridCell(Projectile.Projectile projectile, Vector2Int cellPosition)
        {
            
            if(disableCollisionDetection ) return;
            
            if (!projectile) return;
            
            projectile.LastCellPosition = cellPosition;

            if (_collisionGridDictionary.TryGetValue(cellPosition, out var cellSet))
            {
                cellSet.Add(projectile); // HashSet automatically ignores duplicates
            }
        }
        
        public void RemoveFromCollisionCheck(Projectile.Projectile projectile, Vector2Int cellPosition)
        {
            if(disableCollisionDetection ) return;
            
            if (_collisionGridDictionary.TryGetValue(cellPosition, out var cellSet))
            {
                cellSet.Remove(projectile); // O(1)
            }
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
                
                _key.x = x;
                _key.y = y;
                
                if (!_collisionGridDictionary.TryGetValue(_key, out var cellSet)) continue;
                
                CheckForProjectileCollisions(cellSet , collisionObjectPosition);
            }
        }
        
        private void CheckForProjectileCollisions(HashSet<Projectile.Projectile> cellSet, Vector3 objectPosition)
        {
            var deadProjectiles = new HashSet<Projectile.Projectile>();
            
            foreach (var projectile in cellSet)
            {
                if (!projectile.IsActive) continue;

               //if(CollisionAlgorithms.LineIntersectsCircle(projectile.LastPosition, projectile.GetPosition(), objectPosition, 0.2f))
                if (CollisionAlgorithms.CircleVsCircle(objectPosition, 0.25f, projectile.GetPosition(), .1f))
                {
                    projectile.LifeSpan = 0;
                    deadProjectiles.Add(projectile);
                }
                
                projectile.LastPosition = projectile.GetPosition();
            }
            
            DeleteDeadProjectiles(deadProjectiles);
        }
        
        private void DeleteDeadProjectiles(HashSet<Projectile.Projectile> deadProjectiles)
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
