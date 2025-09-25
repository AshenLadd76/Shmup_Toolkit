using ToolBox.Extensions;
using ToolBox.Utils;

namespace ToolBox.TileManagement.Editor
{
    public static class TileImageMapValidator
    {
        public static bool ValidateTileImageMap(TileImageMap map, string mapName)
        {
            if (map == null)
            {
                Logger.LogError($"TileImageMap is null for {mapName}");
                return false;
            }

            if (map.Rows == 0 || map.Columns == 0)
            {
                Logger.LogError($"TileImageMap {mapName} has invalid dimensions: Rows={map.Rows}, Columns={map.Columns}");
                return false;
            }

            if (map.Cells.IsNullOrEmpty())
            {
                Logger.LogError($"TileImageMap {mapName} has no cells");
                return false;
            }

            int expectedLength = map.Rows * map.Columns;
            if (map.Cells.Count != expectedLength)
            {
                Logger.LogWarning($"TileImageMap {mapName} has {map.Cells.Count} cells but expected {expectedLength}. Some cells may be ignored or missing.");
            }

            // Optional: check for null cell entries
            for (int i = 0; i < map.Cells.Count; i++)
            {
                if (map.Cells[i] == null)
                {
                    Logger.LogWarning($"TileImageMap {mapName} has null cell at index {i}");
                }
            }

            return true;
        }
    }
}