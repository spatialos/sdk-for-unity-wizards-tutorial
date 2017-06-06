using Improbable;
using Improbable.Core;
using Improbable.Global;
using Improbable.Unity.Core;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Worker;
using Improbable.Worker.Query;
using System;
using Assets.Gamelogic.Utils;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    /// <summary>
    /// Helper class for handling spawning and deleting of the player character
    /// </summary>
    public static class ClientPlayerSpawner
    {
        public static void SpawnPlayer()
        {
            FindPlayerSpawnerEntityId(RequestPlayerSpawn);
        }

		private static void FindPlayerSpawnerEntityId(Action<EntityId> playerSpawnCallback)
        {
            var playerSpawnerQuery = Query.HasComponent<PlayerSpawning>().ReturnOnlyEntityIds();
			SpatialOS.WorkerCommands.SendQuery(playerSpawnerQuery, queryResult => OnQueryResult(playerSpawnCallback, queryResult));
        }

        private static void OnQueryResult(Action<EntityId> requestPlayerSpawnCallback, ICommandCallbackResponse<EntityQueryResult> queryResult)
        {
            if (!queryResult.Response.HasValue || queryResult.StatusCode != StatusCode.Success)
            {
				Debug.LogError("PlayerSpawner query failed. SpatialOS workers probably haven't started yet.  Try again in a few seconds.");
                return;
            }

            var queriedEntities = queryResult.Response.Value;
            if (queriedEntities.EntityCount < 1)
            {
                Debug.LogError("Failed to find PlayerSpawner. SpatialOS probably hadn't finished spawning the initial snapshot. Try again in a few seconds.");
                return;
            }

            var playerSpawnerEntityId = queriedEntities.Entities.First.Value.Key;
			requestPlayerSpawnCallback(playerSpawnerEntityId);
        }

		private static void RequestPlayerSpawn(EntityId playerSpawnerEntityId)
        {
			SpatialOS.WorkerCommands.SendCommand(PlayerSpawning.Commands.SpawnPlayer.Descriptor, new SpawnPlayerRequest(), playerSpawnerEntityId)
				.OnFailure(error => OnSpawnPlayerFailure(error, playerSpawnerEntityId));
        }

		private static void OnSpawnPlayerFailure(ICommandErrorDetails error, EntityId playerSpawnerEntityId)
        {
            Debug.LogWarning("SpawnPlayer command failed - you probably tried to connect too soon. Try again in a few seconds.");
        }
    }
}
