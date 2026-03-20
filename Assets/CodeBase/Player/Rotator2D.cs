using UnityEngine;

namespace CodeBase.Player
{
    public class Rotator2D : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [Tooltip("Degrees per second")]
        [SerializeField] private float rotationSpeed = 90f;

        [Tooltip("Clockwise if true, counterclockwise if false")]
        [SerializeField] private bool clockwise = true;

        private void Update()
        {
            // Determine rotation direction
            float direction = clockwise ? -1f : 1f;

            // Rotate around Z-axis (2D rotation)
            transform.Rotate(Vector3.forward * (direction * rotationSpeed * Time.deltaTime));
        }
    }
}