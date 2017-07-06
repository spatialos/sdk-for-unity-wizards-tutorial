using Assets.Gamelogic.ComponentExtensions;
using Improbable;
using Improbable.Building;
using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Life;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Building
{
    public class StockpileDepositoryBehaviour : MonoBehaviour
    {
        [Require] private StockpileDepository.Writer stockpileDepository;
        [Require] private Health.Writer health;

        private void OnEnable ()
        { 
            stockpileDepository.CommandReceiver.OnAddResource.RegisterResponse(OnAddResource);
        }

        private void OnDisable()
        {
            stockpileDepository.CommandReceiver.OnAddResource.DeregisterResponse();
        }

        private Nothing OnAddResource(AddResource request, ICommandCallerInfo callerinfo)
        {
            if (stockpileDepository.Data.canAcceptResources)
            {
                health.AddCurrentHealthDelta(request.quantity);
            }
            return new Nothing();
        }
    }
}
