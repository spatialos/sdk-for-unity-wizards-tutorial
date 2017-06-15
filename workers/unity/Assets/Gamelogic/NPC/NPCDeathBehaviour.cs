using Assets.Gamelogic.Core;
using Improbable.Core;
using Improbable.Life;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class NPCDeathBehaviour : MonoBehaviour
    {
        [Require] private UnityWorkerAuthorityCheck.Writer unityWorkerAuthorityCheck;
        [Require] private Health.Reader health;

        private bool npcDeathActive;

        private void OnEnable()
        {
            npcDeathActive = SimulationSettings.NPCDeathActive;
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
                DieUponHealthDepletion(update);
            }
        }

        private void DieUponHealthDepletion(Health.Update update)
        {
            if (npcDeathActive && update.currentHealth.Value <= 0)
            {
                SpatialOS.Commands.DeleteEntity(unityWorkerAuthorityCheck, gameObject.EntityId());
            }
        }
    }
}