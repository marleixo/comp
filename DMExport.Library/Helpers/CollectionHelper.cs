using System;
using System.Collections.Generic;

namespace DMExport.Library.Helpers
{
    public static class CollectionHelper
    {
        /// <summary>
        /// ForEach method for IEnumerable collection
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="collection">IEnumerable Collection</param>
        /// <param name="action">Action</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}
