using Assets.Gamelogic.EntityTemplate;
using Improbable;
using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Global;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class PlayerSpawningBehaviour : MonoBehaviour
    {
        [Require] private PlayerSpawning.Writer playerSpawning;
        
        private void OnEnable()
        {
            playerSpawning.CommandReceiver.OnSpawnPlayer.RegisterResponse(OnSpawnPlayer);
        }

        private void OnDisable()
        {
            playerSpawning.CommandReceiver.OnSpawnPlayer.DeregisterResponse();
        }

        private Nothing OnSpawnPlayer(SpawnPlayerRequest request, ICommandCallerInfo callerinfo)
        {
            var clientWorkerId = callerinfo.CallerWorkerId;
            SpawnPlayerWithReservedId(clientWorkerId);
            return new Nothing();
        }

        private void SpawnPlayerWithReservedId(string clientWorkerId)
        {
            SpatialOS.Commands.ReserveEntityId(playerSpawning)
                .OnFailure(_ =>
                {
                    Debug.LogError("Failed to Reserve EntityId for Player. Retrying...");
                    SpawnPlayerWithReservedId(clientWorkerId);
                })
                .OnSuccess(result =>
                {
                    SpawnPlayer(clientWorkerId, result.ReservedEntityId);
                });
        }

        private void SpawnPlayer(string clientWorkerId, EntityId entityId)
        {
            var playerEntityTemplate = EntityTemplateFactory.CreatePlayerTemplate(clientWorkerId, SimulationSettings.PlayerPrefabName);
            SpatialOS.Commands.CreateEntity(playerSpawning, entityId, playerEntityTemplate)
                .OnFailure(_ =>
                {
                    Debug.LogError("Failed to Create Player Entity. Retrying...");
                    SpawnPlayer(clientWorkerId, entityId);
                });
        }
    }
}
