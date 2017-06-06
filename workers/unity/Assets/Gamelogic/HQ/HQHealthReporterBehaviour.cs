using Assets.Gamelogic.UI;
using Improbable.Life;
using Improbable.Team;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.HQ
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class HQHealthReporterBehaviour : MonoBehaviour
    {
        [Require] private Health.Reader health;
        [Require] private TeamAssignment.Reader teamAssignment;

        private void OnEnable()
        {
            health.ComponentUpdated.Add(OnHealthUpdated);
            UpdateHQHealthBar(teamAssignment.Data.teamId, health.Data.currentHealth);
        }

        private void OnDisable()
        {
            health.ComponentUpdated.Remove(OnHealthUpdated);
        }

        private void OnHealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue)
            {
                UpdateHQHealthBar(teamAssignment.Data.teamId, update.currentHealth.Value);
            }
        }

        private void UpdateHQHealthBar(uint teamId, float healthValue)
        {
            HQsPanelController.SetHQHealth(teamId, healthValue);
        }
    }
}
