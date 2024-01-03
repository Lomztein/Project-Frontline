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

    public static IList<IList<T>> Permute<T>(T[] nums)
    {
        var list = new List<IList<T>>();
        return DoPermute(nums, 0, nums.Length - 1, list);
    }

    private static IList<IList<T>> DoPermute<T>(T[] nums, int start, int end, IList<IList<T>> list)
    {
        if (start == end)
        {
            // We have one of our possible n! solutions,
            // add it to the list.
            list.Add(new List<T>(nums));
        }
        else
        {
            for (var i = start; i <= end; i++)
            {
                Swap(ref nums[start], ref nums[i]);
                DoPermute(nums, start + 1, end, list);
                Swap(ref nums[start], ref nums[i]);
            }
        }

        return list;
    }

    private static void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }
}
