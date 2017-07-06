using Assets.Gamelogic.Core;
using Assets.Gamelogic.EntityTemplate;
using Assets.Gamelogic.Utils;
using Improbable.Building;
using Improbable.Collections;
using Improbable.Npc;
using Improbable.Team;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using System.Collections.Generic;
using Assets.Gamelogic.Team;
using Improbable;
using UnityEngine;

namespace Assets.Gamelogic.Building
{
    public class NPCSpawnerBehaviour : MonoBehaviour
    {
        [Require] private NPCSpawner.Writer npcSpawner;
        [Require] private TeamAssignment.Reader teamAssignment;

        private Coroutine spawnNPCsReduceCooldownCoroutine;
        private string maxWizardsFlagName = "max_npc_wizards";

        private static readonly IDictionary<NPCRole, float> npcRolesToCooldownDictionary = new Dictionary<NPCRole, float>
        {
            { NPCRole.LUMBERJACK, SimulationSettings.LumberjackSpawningCooldown },
            { NPCRole.WIZARD, SimulationSettings.WizardSpawningCooldown }
        };

        private void OnEnable()
        {
            var npcRoles = new System.Collections.Generic.List<NPCRole>(npcRolesToCooldownDictionary.Keys);

            spawnNPCsReduceCooldownCoroutine = StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.SimulationTickInterval, () =>
            {
                ReduceSpawnCooldown(npcRoles, SimulationSettings.SimulationTickInterval);
            }));
        }

        private void OnDisable()
        {
            CancelSpawnNPCsReduceCooldownCoroutine();
        }

        private void CancelSpawnNPCsReduceCooldownCoroutine()
        {
            if (spawnNPCsReduceCooldownCoroutine != null)
            {
                StopCoroutine(spawnNPCsReduceCooldownCoroutine);
                spawnNPCsReduceCooldownCoroutine = null;
            }
        }

        private void ReduceSpawnCooldown(IList<NPCRole> npcRoles, float interval)
        {
            if (!npcSpawner.Data.spawningEnabled)
            {
                return;
            }

            var newCooldowns = new Map<NPCRole, float>(npcSpawner.Data.cooldowns);

            for (var i = 0; i < npcRoles.Count; i++)
            {
                var role = npcRoles[i];
                if (newCooldowns[role] <= 0f) // todo: this is a workaround for WIT-1374
                {
                    var spawningOffset = new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f) * SimulationSettings.SpawnOffsetFactor;
                    var spawnPosition = (gameObject.transform.position + spawningOffset).ToCoordinates();
                    SpawnNpc(role, spawnPosition);
                    newCooldowns[role] = npcRolesToCooldownDictionary[role];
                }
                else
                {
                    newCooldowns[role] = Mathf.Max(newCooldowns[role] - interval, 0f);
                }
            }
            npcSpawner.Send(new NPCSpawner.Update().SetCooldowns(newCooldowns));
        }

        public void SetSpawningEnabled(bool spawningEnabled)
        {
            if (spawningEnabled != npcSpawner.Data.spawningEnabled)
            {
                npcSpawner.Send(new NPCSpawner.Update().SetSpawningEnabled(spawningEnabled));
            }
        }

        private void SpawnNpc(NPCRole npcRoleEnum, Coordinates position)
        {
            switch (npcRoleEnum)
            {
                case NPCRole.LUMBERJACK:
                    SpawnLumberjack(position);
                    break;
                case NPCRole.WIZARD:
                    SpawnWizard(position);
                    break;
            }
        }

        private int GetLumberjackCount()
        {
            var lumberjacks = GameObject.FindGameObjectsWithTag("NPCLumberjack");
            var count = 0;
            for (var i = 0; i < lumberjacks.Length; ++i)
            {
                var teamAssignmentVisualizer = lumberjacks[i].GetComponent<TeamAssignmentVisualizerUnityWorker>();
                if (teamAssignmentVisualizer != null && teamAssignmentVisualizer.TeamId == teamAssignment.Data.teamId)
                {
                    ++count;
                }
            }
            return count;
        }

        private void SpawnLumberjack(Coordinates position)
        {
            var lumberjackCount = GetLumberjackCount();
            if (lumberjackCount >= 20)
            {
                return;
            }
            var template = EntityTemplateFactory.CreateNPCLumberjackTemplate(position, teamAssignment.Data.teamId);
            SpatialOS.Commands.CreateEntity(npcSpawner, template);
        }

        private int GetWizardCount()
        {
            var wizards = GameObject.FindGameObjectsWithTag("NPCWizard");
            var count = 0;
            for (var i = 0; i < wizards.Length; ++i)
            {
                var teamAssignmentVisualizer = wizards[i].GetComponent<TeamAssignmentVisualizerUnityWorker>();
                if (teamAssignmentVisualizer != null && teamAssignmentVisualizer.TeamId == teamAssignment.Data.teamId)
                {
                    ++count;
                }
            }
            return count;
        }

        private void SpawnWizard(Coordinates position)
        {
            var wizardCount = GetWizardCount();
            int maxNpcWizards = 0;

            if(SpatialOS.Connection.GetWorkerFlag("max_npc_wizards").HasValue )
            {
                maxNpcWizards = int.Parse(SpatialOS.Connection.GetWorkerFlag(maxWizardsFlagName).Value);
            }
            if (wizardCount >= maxNpcWizards)
            {
                return;
            }
            var template = EntityTemplateFactory.CreateNPCWizardTemplate(position, teamAssignment.Data.teamId);
            SpatialOS.Commands.CreateEntity(npcSpawner, template);
        }

        public void ResetCooldowns()
        {
			npcSpawner.Send(new NPCSpawner.Update().SetCooldowns(new Map<NPCRole, float> { { NPCRole.LUMBERJACK, SimulationSettings.LumberjackSpawningCooldown }, { NPCRole.WIZARD, SimulationSettings.WizardSpawningCooldown } }));
        }
    }
}
