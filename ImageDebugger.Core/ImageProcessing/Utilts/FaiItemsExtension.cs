using System.Collections.Generic;
using System.Linq;
using ImageDebugger.Core.Models;

namespace ImageDebugger.Core.ImageProcessing.Utilts
{
    public static class FaiItemsExtension
    {
        public static FaiItem ByName(this IEnumerable<FaiItem> items, string name)
        {
            return items.First(ele => ele.Name == name);
        }
    }
}