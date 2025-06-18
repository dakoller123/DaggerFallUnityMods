using UnityEngine;

namespace Game.Mods.UnleveledEnemyNPCs
{
    public static class BellCurveRandom
    {
        public static int GetBellCurveRandom(int min, int max, int offsetFromCenter)
        {
            const int samples = 6;
            const int multiplier = 1000;
            var sum = 0;

            for (var i = 0; i < samples; i++)
            {
                sum += Random.Range(min*multiplier, max*multiplier);
            }

            var avg = sum / (samples * multiplier);

            var value = avg + offsetFromCenter;

            if (value < min) value = min;
            if (value > max) value = max;
            return value;
        }
    }
}