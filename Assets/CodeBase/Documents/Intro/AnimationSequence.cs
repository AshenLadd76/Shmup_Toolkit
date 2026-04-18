using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Documents.Intro
{
    [CreateAssetMenu(fileName = "IntroAssetsSo", menuName = "Game Assets/Intro Assets")]
    public class AnimationSequenceSo : ScriptableObject
    {
        [SerializeField] private List<AnimationSegment> segments = new List<AnimationSegment>();
        public List<AnimationSegment> Segments => segments;
        
    }
}