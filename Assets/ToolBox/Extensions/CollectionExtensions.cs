using System.Collections.Generic;
using System.Linq;

namespace ToolBox.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => source == null || !source.Any();
        
        public static bool IsIndexValid<T>(this IList<T> list, int index) => list != null && index >= 0 && index < list.Count;
        
    }
}
