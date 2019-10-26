using System.Collections.Generic;
using System.Linq;
using UI.Models;

namespace UI.ImageProcessing.Utilts
{
    public static class FaiItemsExtension
    {
        public static FaiItem ByName(this IEnumerable<FaiItem> items, string name)
        {
            return items.First(ele => ele.Name == name);
        }
    }
}