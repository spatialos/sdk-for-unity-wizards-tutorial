using Assets.Gamelogic.ComponentExtensions;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Life;
using Improbable.Entity.Component;
using Improbable.Life;
using Improbable.Tree;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Tree
{
    public class HarvestableBehaviour : MonoBehaviour
    {
        [Require] private Harvestable.Writer harvestable;
        [Require] private Health.Writer health;

        private void OnEnable()
        {
            harvestable.CommandReceiver.OnHarvest.RegisterResponse(OnHarvest);
        }

        private void OnDisable()
        {
            harvestable.CommandReceiver.OnHarvest.DeregisterResponse();
        }

        private HarvestResponse OnHarvest(HarvestRequest request, ICommandCallerInfo callerinfo)
        {
            var resourcesToGive = Mathf.Min(SimulationSettings.HarvestReturnQuantity, health.Data.currentHealth);
            health.AddCurrentHealthDelta(-resourcesToGive);
            return new HarvestResponse(resourcesToGive);
        }
    }
}
