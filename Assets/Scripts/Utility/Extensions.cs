using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static partial class Extensions
{
    /// <summary>
    /// Add/Remove elements until this list is count elements long.
    /// </summary>
    public static void Resize<T>(this List<T> list, int count)
    {
        if (count < list.Count)
        {
            list.RemoveRange(count, list.Count - count);
        }

        for (int i = list.Count; i < count; ++i)
        {
            list.Add(default(T));
        }
    }
}
