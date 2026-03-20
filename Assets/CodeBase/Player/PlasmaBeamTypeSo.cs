using UnityEngine;

namespace CodeBase.Player
{
    [CreateAssetMenu(fileName = "PlasmaBeamType", menuName = "Shmup/PlasmaBeamType")]
    public class PlasmaBeamTypeSo : ScriptableObject
    {
        [field: SerializeField] public string BeamName { get; private set; }

        [Header("Beam Dimensions & Behavior")]
        [field: SerializeField] public float DefaultBeamLength { get; private set; } = 14f;
        [field: SerializeField] public float Width { get; private set; } = 1f;
        [field: SerializeField] public float BeamWidthOffset { get; private set; } = 0.2f;
        [field: SerializeField] public float GrowSpeed { get; private set; } = 8f;
        [field: SerializeField] public float ShrinkSpeed { get; private set; } = 40f;
        [field: SerializeField] public int DamagePerSecond { get; private set; } = 1;

        [Header("Visuals")]
        [field: SerializeField] public Material BeamMaterial { get; private set; }       // Scrolling middle beam
        [field: SerializeField] public Sprite BeamSprite { get; private set; }           // Middle sprite
        [field: SerializeField] public Sprite TipSprite { get; private set; }            // Front tip
        [field: SerializeField] public Sprite BaseSprite { get; private set; }           // Base at origin

        [field: SerializeField] public Vector3 ScrollSpeed { get; private set; } = Vector3.up;
    }
}