using UnityEngine;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Core
{
    /// <summary>
    /// Regularly checks to see that the client has sent a heartbeat to indicate it's still connected.
    /// If no heartbeat is received after a certain time (in case of an event like a client crash), the entity is deleted from the world.
    /// </summary>
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class ReceiveClientConnectionHeartbeatBehaviour : MonoBehaviour
    {
        [Require] private ConnectionHeartbeat.Writer heartbeat;

        private Coroutine heartbeatCoroutine;

        private void OnEnable()
        {
            heartbeat.CommandReceiver.OnHeartbeat.RegisterResponse(OnHeartbeat);
            heartbeatCoroutine = StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.HeartbeatCheckInterval, CheckHeartbeat));
        }

        private void OnDisable()
        {
            heartbeat.CommandReceiver.OnHeartbeat.DeregisterResponse();
            StopCoroutine(heartbeatCoroutine);
        }

        private Nothing OnHeartbeat(Nothing request, ICommandCallerInfo callerinfo)
        {
            SetHeartbeat(SimulationSettings.DefaultHeartbeatsBeforeTimeout);
            return new Nothing();
        }

        private void CheckHeartbeat()
        {
            var heartbeatsRemainingBeforeTimeout = heartbeat.Data.timeoutBeats;
            if (heartbeatsRemainingBeforeTimeout == 0)
            {
                StopCoroutine(heartbeatCoroutine);
                DeleteInactiveEntity();
                return;
            }
            SetHeartbeat(heartbeatsRemainingBeforeTimeout - 1);
        }
        private void SetHeartbeat(uint beats)
        {
            var update = new ConnectionHeartbeat.Update();
            update.SetTimeoutBeats(beats);
            heartbeat.Send(update);
        }

        private void DeleteInactiveEntity()
        {
            SpatialOS.Commands.DeleteEntity(heartbeat, gameObject.EntityId())
                .OnFailure(OnDeleteFailure);
        }

        private void OnDeleteFailure(ICommandErrorDetails commandErrorDetails)
        {
            Debug.LogErrorFormat("Failed to Delete Inactive Entity {0}: {1}", gameObject.EntityId(), commandErrorDetails.ErrorMessage);
        }

    }
}