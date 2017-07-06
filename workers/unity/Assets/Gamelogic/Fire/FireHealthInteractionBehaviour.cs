using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Assets.Gamelogic.ComponentExtensions;

namespace Assets.Gamelogic.Fire
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    class FireHealthInteractionBehaviour : MonoBehaviour
    {
        [Require] private Health.Writer health;
        [Require] private Flammable.Reader flammable;

        // note: This parameter is overwritable if you want to control the speed at which an entity takes damage, e.g. for trees
        public float FireDamageInterval = SimulationSettings.DefaultFireDamageInterval;
        private Coroutine takeDamageFromFireCoroutine;

        private void OnEnable()
        {
            if (flammable.Data.isOnFire)
            {
                StartDamageRoutine();
            }

            flammable.ComponentUpdated.Add(FlammableOnComponentUpdated);
        }

        private void OnDisable()
        {
            CancelDamageRoutine();

            flammable.ComponentUpdated.Remove(FlammableOnComponentUpdated);
        }

        private void FlammableOnComponentUpdated(Flammable.Update update)
        {
            if (update.isOnFire.HasValue)
            {
                if (flammable.Data.isOnFire)
                {
                    StartDamageRoutine();
                }
                else
                {
                    CancelDamageRoutine();
                }
            }
        }

        private void StartDamageRoutine()
        {
            if (takeDamageFromFireCoroutine != null)
            {
                return;
            }
            takeDamageFromFireCoroutine = StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.SimulationTickInterval * FireDamageInterval, TakeDamageFromFire));
        }

        private void CancelDamageRoutine()
        {
            if (takeDamageFromFireCoroutine != null)
            {
                StopCoroutine(takeDamageFromFireCoroutine);
                takeDamageFromFireCoroutine = null;
            }
        }

        private void TakeDamageFromFire()
        {
            health.AddCurrentHealthDelta(-SimulationSettings.FireDamagePerTick);
        }
    }
}
