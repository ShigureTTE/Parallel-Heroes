using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions {
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static T GetRandom<T>(this IList<T> list) {
        int index = Random.Range(0, list.Count);
        return list[index];
    }
}