using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class EnumerableExtensions 
    {
        public static IEnumerable<T> ObjectToEnumerable<T> (this T obj)
        {
            yield return obj;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            Random random = new Random();
            var array = collection.ToArray();
            int n = array.Length;
            while (n > 1)
            {
                int k = random.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
            return array;
        }
    }
}
