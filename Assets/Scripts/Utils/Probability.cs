using UnityEngine;

public class Probability {
    public static LevelType Range(params LevelTypeRange[] ranges) {
        float total = 0f;
        for (int i = 0; i < ranges.Length; i++) total += ranges[i].Weight;

        float randomValue = Random.value;
        float probability = 0f;

        int cnt = ranges.Length - 1;
        for (int i = 0; i < cnt; i++) {
            probability += ranges[i].Weight / total;
            if (probability >= randomValue) {
                return ranges[i].Type;
            }
        }

        return ranges[cnt].Type;
    }
}

public struct LevelTypeRange {
    public LevelType Type;
    public int Weight;
}