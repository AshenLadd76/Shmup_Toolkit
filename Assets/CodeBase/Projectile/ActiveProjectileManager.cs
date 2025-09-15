using CodeBase.Collision_Handling;
using Sirenix.OdinInspector;
using TMPro;
using ToolBox.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;



namespace CodeBase.Projectile
{
    [RequireComponent(typeof(CollisionDetector))]
    public class ActiveProjectileManager : MonoBehaviour
    {

        [SerializeField] private CollisionDetector collisionDetector;
        
        [SerializeField] private BulletCollisionInfo _target;
        [SerializeField] private Projectile[] activeProjectileArr;
        // Scratch buffer, reused every frame
        [ShowInInspector] private ProjectileDataStruct[] _positionBuffer;

        private Vector3 _velocity;
        
        private int _frameCount;
        
        
        [SerializeField] private int _activeProjectileCount;
        
        private TransformAccessArray _transformAccessArray;
        
        [SerializeField] private TextMeshProUGUI activeProjectileCountText;

        private void Awake()
        {
            collisionDetector = GetComponent<CollisionDetector>();
            
            ScreenBoundaryChecker.Initialize();
        }

        private void Start() => InitializeProjectileArray();

        private void Update()
        {
            BatchMoveActiveProjectiles(Time.deltaTime);
            
            collisionDetector?.UpdateCheck(activeProjectileArr);
            
            if (_frameCount <= 60)
            {
                _frameCount++;
                return;
            }
            
            activeProjectileCountText.SetText(_activeProjectileCount.ToString());
            _frameCount = 0;
        }

        private void LateUpdate() => ScreenBoundaryChecker.CalculateBounds();

        private void InitializeProjectileArray()
        {
            if (transform.childCount <= 0) return;

            int activeArrayLength = GetComponentsInChildren<IProjectile>(true).Length;
            
            activeProjectileArr = new Projectile[activeArrayLength];
        }

        private void BatchMoveActiveProjectiles(float deltaTime = 0)
        {
            // Step 1: Snapshot all positions in one tight loop
           CreatePositionBuffer();
           

            // Step 2: Do batch movement math directly on buffer
            BatchMovementCalculation(deltaTime);
            
            // Step 3: Push updated positions back in one tight loop
            BatchApplyMovement();
            
        }

        private void CreatePositionBuffer()
        {
            if ( _positionBuffer == null || _positionBuffer.Length < _activeProjectileCount)
                _positionBuffer = new ProjectileDataStruct[_activeProjectileCount];
            
            for (int i = 0; i < _activeProjectileCount; i++)
            {
                var projectile = activeProjectileArr[i];
                
                _positionBuffer[i].Position = (float3)projectile.GetPosition();
                _positionBuffer[i].Velocity = (float3)projectile.Velocity;
                _positionBuffer[i].LifeSpan = projectile.LifeSpan;
                _positionBuffer[i].Radius = projectile.Radius;
                
            }
        }
        
        private void BatchMovementCalculation(float deltaTime = 0)
        {
            for (int i = 0; i < _activeProjectileCount; i++)
            {
                _positionBuffer[i].Position +=  _positionBuffer[i].Velocity * deltaTime;
                _positionBuffer[i].LifeSpan -= deltaTime;
            }
        }

        private void BatchApplyMovement()
        {
            for (int i = _activeProjectileCount - 1; i >= 0; i--)
            {
                var projectile = activeProjectileArr[i];
                
                if(!projectile.IsActive) continue;
                
                var expired = _positionBuffer[i].LifeSpan <= 0 || ScreenBoundaryChecker.IsOutsideBounds(_positionBuffer[i].Position);// || CheckCollision(new BulletCollisionInfo(_positionBuffer[i].Position, _positionBuffer[i].Radius), _target);

                if (expired)
                    RemoveExpiredProjectile(projectile, i);
                else
                {
                    activeProjectileArr[i].SetPosition(_positionBuffer[i].Position);
                }
            }
        }

        public void AddActiveProjectile(Projectile projectile)
        {
            activeProjectileArr[_activeProjectileCount] = projectile;
            
            collisionDetector?.AddToCollisionCheckGridCell(projectile);
            
            _activeProjectileCount++;
        }

        private void RemoveExpiredProjectile(Projectile projectile, int index)
        {
            projectile.Release();
            
            collisionDetector?.RemoveFromCollisionCheck(projectile, projectile.LastCellPosition);

            activeProjectileArr[index] = activeProjectileArr[_activeProjectileCount - 1];
            activeProjectileArr[_activeProjectileCount - 1] = null;
            
            // Swap buffer position to keep things aligned
            _positionBuffer[index] = _positionBuffer[_activeProjectileCount - 1];
            
            
            // Shrink the active count
            _activeProjectileCount--;
        }
        
        private bool CheckCollision(BulletCollisionInfo a, BulletCollisionInfo b)
        {
            float sqrDist = (a.Position - b.Position).sqrMagnitude;
            float radiusSum = a.Radius + b.Radius;
            return sqrDist <= radiusSum * radiusSum;
        }
    }
}

// using TMPro;
// using ToolBox.Utils;
// using Unity.Mathematics;
// using UnityEngine;
//
// namespace CodeBase.Projectile
// {
//     public class ActiveProjectileManager : MonoBehaviour
//     {
//         [SerializeField] private Projectile[] activeProjectileArr;
//         [SerializeField] private TextMeshProUGUI activeProjectileCountText;
//
//         private ProjectileDataStruct[] _positionBuffer;
//         private int _activeProjectileCount;
//
//         private float _moveTimer = 0f;
//         private float _uiTimer = 0f;
//
//         private const float MoveInterval = 0.032f; // 60Hz movement, adjust as needed
//         private const float UIUpdateInterval = 1.25f; // update UI every 0.25s
//
//         private void Awake() => ScreenBoundaryChecker.Initialize();
//
//         private void Start() => InitializeProjectileArray();
//
//         private void Update()
//         {
//             float deltaTime = Time.deltaTime;
//
//             // --- Movement ---
//             _moveTimer += deltaTime;
//             if (_moveTimer >= MoveInterval)
//             {
//                 BatchMoveActiveProjectiles(_moveTimer);
//                 _moveTimer = 0f;
//             }
//
//             // --- UI ---
//             _uiTimer += deltaTime;
//             if (_uiTimer >= UIUpdateInterval)
//             {
//                 activeProjectileCountText.SetText(_activeProjectileCount.ToString());
//                 _uiTimer = 0f;
//             }
//         }
//
//         private void InitializeProjectileArray()
//         {
//             if (transform.childCount <= 0) return;
//
//             int length = GetComponentsInChildren<Projectile>(true).Length;
//             activeProjectileArr = new Projectile[length];
//             _positionBuffer = new ProjectileDataStruct[length];
//         }
//
//         private void BatchMoveActiveProjectiles(float deltaTime)
//         {
//             // 1️⃣ Snapshot positions into buffer
//             CreatePositionBuffer();
//
//             // 2️⃣ Move bullets in buffer
//             for (int i = 0; i < _activeProjectileCount; i++)
//             {
//                 ref var proj = ref _positionBuffer[i];
//                 proj.Position += proj.Velocity * deltaTime;
//                 proj.LifeSpan -= deltaTime;
//             }
//
//             // 3️⃣ Apply movement and remove expired bullets
//             for (int i = _activeProjectileCount - 1; i >= 0; i--)
//             {
//                 var projectile = activeProjectileArr[i];
//                 var expired = _positionBuffer[i].LifeSpan <= 0 ||
//                               ScreenBoundaryChecker.IsOutsideBounds(_positionBuffer[i].Position);
//
//                 if (expired)
//                     RemoveExpiredProjectile(i);
//                 else
//                     projectile.SetPosition(_positionBuffer[i].Position);
//             }
//         }
//
//         private void CreatePositionBuffer()
//         {
//             for (int i = 0; i < _activeProjectileCount; i++)
//             {
//                 var projectile = activeProjectileArr[i];
//                 _positionBuffer[i].Position = projectile.GetPosition();
//                 _positionBuffer[i].Velocity = projectile.Velocity;
//                 _positionBuffer[i].LifeSpan = projectile.LifeSpan;
//                 _positionBuffer[i].Radius = projectile.Radius;
//             }
//         }
//
//         public void AddActiveProjectile(Projectile projectile)
//         {
//             activeProjectileArr[_activeProjectileCount] = projectile;
//             _activeProjectileCount++;
//         }
//
//         private void RemoveExpiredProjectile(int index)
//         {
//             var projectile = activeProjectileArr[index];
//             projectile.Release();
//
//             // Swap last bullet into removed slot
//             activeProjectileArr[index] = activeProjectileArr[_activeProjectileCount - 1];
//             _positionBuffer[index] = _positionBuffer[_activeProjectileCount - 1];
//
//             activeProjectileArr[_activeProjectileCount - 1] = null;
//             _activeProjectileCount--;
//         }
//     }
//
//   
// }


