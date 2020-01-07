using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public static CharacterBase GetUniqueCharacter(this IList<CharacterBase> list, List<CharacterBase> existing) {
        int index = Random.Range(0, list.Count);

        while (existing.Any(x => x.stats.characterName == list[index].stats.characterName)) {
            Debug.Log("do you even here");
            index++;
            if (index >= list.Count) {
                index = 0;
            }
        }

        return list[index];
    }
    
}