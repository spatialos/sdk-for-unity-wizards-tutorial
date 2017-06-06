using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable.Abilities;
using Improbable.Collections;
using Improbable.Core;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using System.Collections;
using Assets.Gamelogic.Fire;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class PlayerInfoBehaviour : MonoBehaviour
    {
        [Require] private PlayerInfo.Writer playerInfo;
        [Require] private Health.Writer health;
        [Require] private Flammable.Writer flammable;
        [Require] private Spells.Writer spells;
        [Require] private Inventory.Writer inventory;

        [SerializeField] private TransformSender transformSender;
        [SerializeField] private FlammableBehaviour flammableInterface;

        private void Awake()
        {
            transformSender = gameObject.GetComponentIfUnassigned(transformSender);
            flammableInterface = gameObject.GetComponentIfUnassigned(flammableInterface);
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
                DieUponHealthDepletion(update);
            }
        }

        private void DieUponHealthDepletion(Health.Update update)
        {
            if (update.currentHealth.Value <= 0)
            {
                Die();
                StartCoroutine(RespawnDelayed(SimulationSettings.PlayerRespawnDelay));
            }
        }

        private void Die()
        {
            playerInfo.Send(new PlayerInfo.Update().SetIsAlive(false));
            health.Send(new Health.Update().SetCanBeChanged(false));
            flammableInterface.SelfExtinguish(flammable, false);
            spells.Send(new Spells.Update().SetCooldowns(new Map<SpellType, float> { { SpellType.LIGHTNING, 0f }, { SpellType.RAIN, 0f } }).SetCanCastSpells(false));
        }

        private IEnumerator RespawnDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            Respawn();
        }

        private void Respawn()
        {
            transformSender.TriggerTeleport(playerInfo.Data.initialSpawnPosition.ToVector3());
            health.Send(new Health.Update().SetCurrentHealth(SimulationSettings.PlayerMaxHealth).SetCanBeChanged(true));
            flammableInterface.SelfSetCanBeIgnited(flammable, true);
            spells.Send(new Spells.Update().SetCanCastSpells(true));
            inventory.Send(new Inventory.Update().SetResources(0));
            playerInfo.Send(new PlayerInfo.Update().SetIsAlive(true));
        }
    }
}
