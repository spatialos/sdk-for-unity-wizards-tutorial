using Improbable.Life;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Life
{
    public class HealthVisualizer : MonoBehaviour
    {
        [Require] private Health.Reader health;

        public int CurrentHealth { get { return health.Data.currentHealth; } }
        public int MaxHealth { get { return health.Data.maxHealth; } }
    }
}
