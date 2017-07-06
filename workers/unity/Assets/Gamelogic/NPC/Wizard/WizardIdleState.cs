using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.Team;
using Improbable;
using Improbable.Npc;
using System.Collections.Generic;
using Assets.Gamelogic.Utils;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard
{
    public class WizardIdleState : FsmBaseState<WizardStateMachine, WizardFSMState.StateEnum>
    {
        private readonly TeamAssignmentVisualizerUnityWorker teamAssignment;
        private readonly IList<Coordinates> cachedTeamHqCoordinates;

        public WizardIdleState(WizardStateMachine owner,
                               TeamAssignmentVisualizerUnityWorker inTeamAssignment,
                               IList<Coordinates> inCachedTeamHqCoordinates)
            : base(owner)
        {
            teamAssignment = inTeamAssignment;
            cachedTeamHqCoordinates = inCachedTeamHqCoordinates;
        }

        public override void Enter()
        {
            MoveTowardsRandomEnemyHQ();
        }

        public override void Exit(bool disabled)
        {
        }

        public override void Tick()
        {
        }

        private void MoveTowardsRandomEnemyHQ()
        {
            var enemyTeamId = GetRandomEnemyTeamId();
            if (enemyTeamId < 0)
            {
                Debug.LogError("GetRandomEnemyTeamId() returned a negative value.");
                return;
            }

            var approximateHqPosition = cachedTeamHqCoordinates[enemyTeamId];
            var x = approximateHqPosition.X + (-SimulationSettings.NPCSpawnDistanceToHQ / 2) + (SimulationSettings.NPCSpawnDistanceToHQ * Random.value);
            var z = approximateHqPosition.Z + (-SimulationSettings.NPCSpawnDistanceToHQ / 2) + (SimulationSettings.NPCSpawnDistanceToHQ * Random.value);
            cachedTeamHqCoordinates[enemyTeamId] = new Coordinates(x, approximateHqPosition.y, z);

            MoveToPosition(approximateHqPosition.ToVector3());
        }

        private int GetRandomEnemyTeamId()
        {
            if (SimulationSettings.TeamHQLocations.Length >= 2)
            {
                var randomEnemyTeamId = Random.Range(0, SimulationSettings.TeamHQLocations.Length - 1);
                return (randomEnemyTeamId == teamAssignment.TeamId) ? (SimulationSettings.TeamHQLocations.Length - 1) : randomEnemyTeamId;
            }
            return -1;
        }

        private void MoveToPosition(Vector3 position)
        {
            Owner.TriggerTransition(WizardFSMState.StateEnum.MOVING_TO_TARGET, new EntityId(), position);
        }
    }
}
