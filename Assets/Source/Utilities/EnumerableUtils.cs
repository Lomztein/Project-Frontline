using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class EnumerableUtils
{
    public static IEnumerable<T> SingleObjectAsEnumerable<T> (this T obj)
    {
        yield return obj;
    }
}
