using UnityEngine;

namespace ToolBox.Data.Importer
{
    [CreateAssetMenu(menuName = "CSVData/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public string Name;
        public int Score;
        public bool IsActive;
    }
}
