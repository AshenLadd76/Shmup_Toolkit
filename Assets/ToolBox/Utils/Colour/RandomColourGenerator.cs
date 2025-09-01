using UnityEngine;

namespace ToolBox.Utils.Colour
{
    public static class RandomColourGenerator
    {
        public static Color GenerateRandomColor(float alpha = 1f) => new Color(Random.value, Random.value, Random.value, alpha);
    }
}