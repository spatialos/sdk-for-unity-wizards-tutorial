using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Abilities;
using Improbable.Building;
using Improbable.Collections;
using Improbable.Core;
using Improbable.Fire;
using Improbable.Global;
using Improbable.Life;
using Improbable.Npc;
using Improbable.Player;
using Improbable.Team;
using Improbable.Tree;
using Improbable.Unity.Core.Acls;
using Improbable.Unity.Entity;
using Improbable.Worker;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Gamelogic.EntityTemplate
{
    public static class EntityTemplateFactory
    {
        public static Entity CreatePlayerTemplate(string clientWorkerId, string playerPrefabName)
        {
            var teamId = (uint) Random.Range(0, SimulationSettings.TeamCount);
            var spawningOffset = new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f) * SimulationSettings.PlayerSpawnOffsetFactor;
            var hqPosition = SimulationSettings.TeamHQLocations[teamId].ToVector3();
            var spawnPosition = (hqPosition + spawningOffset).ToCoordinates();

            var template = EntityBuilder.Begin()
                .AddPositionComponent(spawnPosition.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(playerPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new ClientAuthorityCheck.Data(), CommonRequirementSets.SpecificClientOnly(clientWorkerId))
                .AddComponent(new UnityWorkerAuthorityCheck.Data(), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TransformComponent.Data(0), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new PlayerInfo.Data(true, spawnPosition), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new PlayerControls.Data(spawnPosition), CommonRequirementSets.SpecificClientOnly(clientWorkerId))
                .AddComponent(new Health.Data(SimulationSettings.PlayerMaxHealth, SimulationSettings.PlayerMaxHealth,
                    true), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Flammable.Data(false, true, FireEffectType.SMALL), CommonRequirementSets.PhysicsOnly)
                .AddComponent(
                    new Spells.Data(new Map<SpellType, float> {{SpellType.LIGHTNING, 0f}, {SpellType.RAIN, 0f}}, true), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Inventory.Data(0), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new ConnectionHeartbeat.Data(SimulationSettings.DefaultHeartbeatsBeforeTimeout), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TeamAssignment.Data(teamId), CommonRequirementSets.PhysicsOnly)
                .Build();

            return template;
        }

        public static Entity CreateBarracksTemplate(Coordinates initialPosition, BarracksState barracksState, uint teamId)
        {
            var template = EntityBuilder.Begin()
                .AddPositionComponent(initialPosition.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(SimulationSettings.BarracksPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new UnityWorkerAuthorityCheck.Data(), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TransformComponent.Data((uint)(UnityEngine.Random.value * 360)), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new BarracksInfo.Data(barracksState), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Health.Data(barracksState == BarracksState.CONSTRUCTION_FINISHED ? SimulationSettings.BarracksMaxHealth : 0, SimulationSettings.BarracksMaxHealth, true), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Flammable.Data(false, false, FireEffectType.BIG), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new StockpileDepository.Data(barracksState == BarracksState.UNDER_CONSTRUCTION), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new NPCSpawner.Data(barracksState == BarracksState.CONSTRUCTION_FINISHED, new Map<NPCRole, float> { { NPCRole.LUMBERJACK, SimulationSettings.LumberjackSpawningCooldown }, { NPCRole.WIZARD, SimulationSettings.WizardSpawningCooldown } }), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TeamAssignment.Data(teamId), CommonRequirementSets.PhysicsOnly)
                .Build();

            return template;
        }

        public static Entity CreateTreeTemplate(Coordinates initialPosition, uint initialRotation)
        {
            var template = EntityBuilder.Begin()
                .AddPositionComponent(initialPosition.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(SimulationSettings.TreePrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new UnityWorkerAuthorityCheck.Data(), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TransformComponent.Data(initialRotation), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Harvestable.Data(), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Health.Data(SimulationSettings.TreeMaxHealth, SimulationSettings.TreeMaxHealth, true), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Flammable.Data(false, true, FireEffectType.BIG), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TreeState.Data((TreeType) Random.Range(0, 2), TreeFSMState.HEALTHY), CommonRequirementSets.PhysicsOnly)
                .Build();
            return template;
        }

        public static Entity CreateNPCLumberjackTemplate(Coordinates initialPosition, uint teamId)
        {
            var template = EntityBuilder.Begin()
                .AddPositionComponent(initialPosition.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(SimulationSettings.NPCPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new TransformComponent.Data(0), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new UnityWorkerAuthorityCheck.Data(), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Health.Data(SimulationSettings.LumberjackMaxHealth, SimulationSettings.LumberjackMaxHealth, true), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Flammable.Data(false, true, FireEffectType.SMALL), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TargetNavigation.Data(NavigationState.INACTIVE, Vector3f.ZERO, new EntityId(), 0f), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Inventory.Data(0), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new NPCLumberjack.Data(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition.ToVector3f()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TeamAssignment.Data(teamId), CommonRequirementSets.PhysicsOnly)
                .Build();

            return template;
        }

        public static Entity CreateNPCWizardTemplate(Coordinates initialPosition, uint teamId)
        {
            var template = EntityBuilder.Begin()
                .AddPositionComponent(initialPosition.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(SimulationSettings.NPCWizardPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new TransformComponent.Data(0), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new UnityWorkerAuthorityCheck.Data(), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Health.Data(SimulationSettings.WizardMaxHealth, SimulationSettings.WizardMaxHealth,
                    true), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Flammable.Data(false, true, FireEffectType.SMALL), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TargetNavigation.Data(NavigationState.INACTIVE, Vector3f.ZERO, new EntityId(), 0f), CommonRequirementSets.PhysicsOnly)
                .AddComponent(
                    new Spells.Data(new Map<SpellType, float> {{SpellType.LIGHTNING, 0f}, {SpellType.RAIN, 0f}}, true), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new NPCWizard.Data(WizardFSMState.StateEnum.IDLE, new EntityId(),
                    SimulationSettings.InvalidPosition.ToVector3f()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TeamAssignment.Data(teamId), CommonRequirementSets.PhysicsOnly)
                .Build();

            return template;
        }

        public static Entity CreateHQTemplate(Coordinates initialPosition, uint initialRotation, uint teamId)
        {
            var template = EntityBuilder.Begin()
                .AddPositionComponent(initialPosition.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(SimulationSettings.HQPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new UnityWorkerAuthorityCheck.Data(), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new HQInfo.Data(new List<EntityId>()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TransformComponent.Data(initialRotation), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Health.Data(SimulationSettings.HQMaxHealth, SimulationSettings.HQMaxHealth, true), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new TeamAssignment.Data(teamId), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Flammable.Data(false, true, FireEffectType.BIG), CommonRequirementSets.PhysicsOnly)
                .Build();

            return template;
        }

        public static Entity CreatePlayerSpawnerTemplate()
        {
            var template = EntityBuilder.Begin()
                .AddPositionComponent(Vector3.zero, CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(SimulationSettings.PlayerSpawnerPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new TransformComponent.Data(0), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new UnityWorkerAuthorityCheck.Data(), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new PlayerSpawning.Data(), CommonRequirementSets.PhysicsOnly)
                .Build();

            return template;
        }
    }
}
