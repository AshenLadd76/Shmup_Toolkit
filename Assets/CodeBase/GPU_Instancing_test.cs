using System.Linq;
using NUnit.Compatibility;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;


namespace CodeBase
{
    public class GPUInstancingTest : MonoBehaviour
    {
        
        [SerializeField] private TextMeshProUGUI projectileCountText;
        
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Material material;
        
        private Mesh projectileMesh;
        
        [SerializeField] private int projectileCount = 50000;
        [SerializeField] private float projectileSize = 0.1f;
        [SerializeField] private float spawnRadius = 50f;

        [SerializeField] private float speed = 3f;
        
        private const int BatchSize = 1023;
        
        [SerializeField] private LayerMask layerMask;

        private ProjectileTestData[] _projectilePositionsArray;

        private Matrix4x4[] _matrices;
        
      // [SerializeField] private float3[] positionsArr;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            
            var meshFilter = projectilePrefab.GetComponent<MeshFilter>();

            if (meshFilter == null) return;

            projectileMesh = meshFilter.sharedMesh;
            
         //   positionsArr = new float3[projectileCount];
            
            GeneratePositionsArray();
            
            _matrices = new Matrix4x4[projectileCount];
            
        }

        private void GeneratePositionsArray()
        {
            _projectilePositionsArray = new ProjectileTestData[projectileCount];

            for (int x = 0; x < projectileCount; x++)
            {
                var projectile = _projectilePositionsArray[x];
                
                _projectilePositionsArray[x].Position = new float3(   UnityEngine.Random.Range(-spawnRadius, spawnRadius), UnityEngine.Random.Range(-spawnRadius, spawnRadius), UnityEngine.Random.Range(-spawnRadius, spawnRadius));
                projectile.Position = new Vector3(0, 0, 0);
                projectile.Phase = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                projectile.Amplitude = UnityEngine.Random.Range(1f, 3f);
                //
                // var step = x * (Mathf.PI * 2f) / projectileCount;
                // var angle = x * step;
                //
                // projectile.Direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;
                
              //  _projectilePositionsArray[x] = projectile;
                
            }
            
            projectileCountText.text = projectileCount.ToString();
        }

       

        private void UpdateProjectilePositions()
        {
            for (int x = 0; x < _projectilePositionsArray.Length; x++)
            {
                float offsetX = Mathf.Sin( Time.time + _projectilePositionsArray[x].Phase);
                float offsetY = Mathf.Sin(Time.time * 0.7f + _projectilePositionsArray[x].Phase);
               float offsetZ = Mathf.Sin(Time.time * 1.3f + _projectilePositionsArray[x].Phase);


               _projectilePositionsArray[x].Position = new float3(offsetX, offsetY, offsetZ) * _projectilePositionsArray[x].Amplitude;

               // var projectile = _projectilePositionsArray[x];

               //  projectile.Position += projectile.Direction * speed * Time.deltaTime; 

               // _projectilePositionsArray[x] = projectile;
            }
        }

        private void Update()
        {

            if (Input.GetKey(KeyCode.Space))
            {
                     
                UpdateProjectilePositions();
                RenderProjectiles();
            }
       
        }

        private void RenderProjectiles()
        {
            for (int i = 0; i < _projectilePositionsArray.Length; i++)
            {
                _matrices[i] = Matrix4x4.TRS(_projectilePositionsArray[i].Position, Quaternion.identity, Vector3.one);
            }

            for (int i = 0; i < _projectilePositionsArray.Length; i += BatchSize)
            {
                int count = Mathf.Min(BatchSize, _projectilePositionsArray.Length - i);
                
                Graphics.DrawMeshInstanced(projectileMesh, 0, material, _matrices.Skip(i).Take(count).ToArray(), count,  null);

            }
        }
        
    }

    public struct ProjectileTestData
    {
        public float3 Position;
        public float3 Direction; // optional if GPU calculates movement
        public float Rotation;  // rotation in radians
        public float Scale;   // bullet size
        public float Frame;  // animation frame index
        public float Phase;  // optional for oscillation/waves
        public float Amplitude;  // optional for oscillation/waves
        public int ProjectileID; // identifier for different projectile types.
        public bool IsActive;
        public int LifeSpan;
    }
}
