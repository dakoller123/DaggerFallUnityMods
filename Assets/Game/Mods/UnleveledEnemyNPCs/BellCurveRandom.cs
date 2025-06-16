using UnityEngine;

namespace Game.Mods.UnleveledEnemyNPCs
{
    public static class BellCurveRandom
    {
        public static int GetBellCurveRandom(int min, int max, int mostCommon)
        {
            Debug.Log($"{nameof(GetBellCurveRandom)} called. {min}, {max}, {mostCommon}");
            const int samples = 12; // Number of samples to shape the curve

            int sum = 0;
            for (int i = 0; i < samples; i++)
            {
                sum += Random.Range(0, 10001); // [0, 10000] to preserve integer range
            }

            // Average the sum to get an approximation of a bell curve
            int avg = sum / samples; // Result in range [0, 10000]

            // Skew toward mostCommon
            int range = max - min;
            int offsetFromCenter = mostCommon - (min + range / 2);
            int skew = (offsetFromCenter * (avg - 5000)) / 5000; // [-range/2, +range/2]

            int value = min + ((range * avg) / 10000) + skew;

            // Clamp to [min, max]
            if (value < min) value = min;
            if (value > max) value = max;
            Debug.Log($"{nameof(GetBellCurveRandom)} return with value {value}");
            return value;
        }
    }
}