using System.Collections.Generic;
using System.Linq;

namespace UI.Helpers
{
    public static class MegaListHelper
    {
        public static List<List<T>> AsRows<T>(this List<List<T>> megaList)
        {
            // Init output
            var rows = new List<List<T>>();
            var numRows = megaList.ElementAt(0).Count;
            var numCols = megaList.Count;
            for (int i = 0; i < numRows; i++)
            {
                var row = new List<T>();
                for (int j = 0; j < numCols; j++)
                {
                    row.Add(default(T));
                }
                rows.Add(row);
            }

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    rows[row][col] = megaList.ElementAt(col)[row];
                }
            }

            return rows;
        }
    }
}