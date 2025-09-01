using System;
using UnityEngine;

namespace ToolBox.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Ensures the array has at least minSize capacity. If not, resizes it using a growth factor (default 2x).
        /// </summary>
        public static void EnsureCapacity<T>(ref T[] array, int threshold, float growthFactor = 2f)
        {
            if (array.Length < threshold) return;

            int newSize = Mathf.Max(threshold, Mathf.CeilToInt(array.Length * growthFactor));
            T[] newArray = new T[newSize];
            Array.Copy(array, newArray, array.Length);
            array = newArray;
        }
        
        /// <summary>
        /// Ensures the array grows only if activeCount >= threshold. Uses growthFactor for resizing.
        /// </summary>
        public static void EnsureCapacity<T>(ref T[] array, int activeCount, int threshold, float growthFactor = 2f)
        {
            if (activeCount < threshold) return; // only grow when threshold reached

            int newSize = Mathf.CeilToInt(array.Length * growthFactor);
            T[] newArray = new T[newSize];
            Array.Copy(array, newArray, array.Length);
            array = newArray;
        }
    }
}
