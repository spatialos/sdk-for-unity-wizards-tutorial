using Assets.Gamelogic.Core;
using Assets.Gamelogic.Fire;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.HQ
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class HQFlammabilityBehaviour : MonoBehaviour
    {
        [Require] private Health.Writer health;
        [Require] private Flammable.Writer flammable;
        [SerializeField] private FlammableBehaviour flammableBehaviour;

        private void Awake()
        {
            flammableBehaviour = gameObject.GetComponentIfUnassigned(flammableBehaviour);
        }

        private void OnEnable()
        {
            health.ComponentUpdated.Add(OnHealthUpdated);
        }

        private void OnDisable()
        {
            health.ComponentUpdated.Remove(OnHealthUpdated);
        }

        private void OnHealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue)
            {
                UpdateHQFlammablility(update.currentHealth.Value);
            }
        }

        private void UpdateHQFlammablility(int healthValue)
        {
            if (healthValue <= 0)
            {
                flammableBehaviour.SelfExtinguish(health, false);
            }
            else
            {
                var canBeIgnited = healthValue > 0;
                flammableBehaviour.SelfSetCanBeIgnited(health, canBeIgnited);
            }
        }
    }
}
