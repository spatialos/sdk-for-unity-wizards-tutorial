using Improbable.Life;
using UnityEngine;

namespace Assets.Gamelogic.ComponentExtensions
{
    static class HealthExtensions
    {
        public static void SetCanBeChanged(this Health.Writer health, bool canBeChanged)
        {
            health.Send(new Health.Update().SetCanBeChanged(canBeChanged));
        }

        public static void SetCurrentHealth(this Health.Writer health, int newHealth)
        {
            if (health.Data.canBeChanged)
            {
                health.Send(new Health.Update().SetCurrentHealth(Mathf.Max(newHealth, 0)));
            }
        }

        public static void AddCurrentHealthDelta(this Health.Writer health, int delta)
        {
            if (health.Data.canBeChanged)
            {
                if (health.TryingToDecreaseHealthBelowZero(delta))
                {
                    return;
                }
                health.Send(new Health.Update().SetCurrentHealth(Mathf.Max(health.Data.currentHealth + delta, 0)));
            }
        }

        private static bool TryingToDecreaseHealthBelowZero(this Health.Reader health, int delta)
        {
            return health.Data.currentHealth == 0 && delta < 0;
        }
    }
}
