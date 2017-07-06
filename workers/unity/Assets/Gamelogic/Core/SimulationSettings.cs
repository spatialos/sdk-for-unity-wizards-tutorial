using Improbable;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public static class SimulationSettings
    {
        // Connection
        public static float ClientConnectionTimeoutSecs = 7;

        // Entity Prefab Names
        public static string PlayerPrefabName = "Player";
        public static string NPCPrefabName = "NPCLumberjack";
        public static string NPCWizardPrefabName = "NPCWizard";
        public static string HQPrefabName = "HQ";
        public static string TreePrefabName = "Tree";
        public static string StockpilePrefabName = "Stockpile";
        public static string BarracksPrefabName = "Barracks";
        public static string PlayerSpawnerPrefabName = "PlayerSpawner";
        public static string LightningPrefabName = "Lightning";
        public static string TeamPrefabName = "Team";

        // Resource Prefab Paths
        public static string SpellAOEIndicatorPrefabPath = "UI/SpellAOEIndicator";
        public static string EntityInfoCanvasPrefabPath = "UI/EntityInfoCanvas";
        public static string LightningEffectPrefabPath = "Particles/LightningEffect";
        public static string RainEffectPrefabPath = "Particles/RainEffect";
        public static string FireEffectPrefabPath = "Particles/Fire";
        public static string SmallFireEffectPrefabPath = "Particles/SmallFire";
        public static string DeathEffectPrefabPath = "Particles/DeathEffect";

        // Unity Layers
        public static string TerrainLayerName = "Terrain";
        public static string TreeLayerName = "Tree";
        public static string BarrackLayerName = "Barrack";

        // Build Process
        public static readonly string UnityClientScene = "UnityClient";
        public static readonly string UnityWorkerScene = "UnityWorker";
        public static readonly string SplashScreenScene = "SplashScreen";
        public static readonly string[] ClientScenes = { UnityClientScene, SplashScreenScene };
        public static readonly string[] WorkerScenes = { UnityWorkerScene };
        public static readonly string ClientDefaultActiveScene = UnityClientScene;
        public static readonly string WorkerDefaultActiveScene = UnityWorkerScene;
        public const string SceneDirectory = "Assets";

        // UI
        public static Color TransparentWhite = new Color(1f, 1f, 1f, 0.3f);

        // World
        public static double SpawningWorldEdgeLength = 1000;
        public static Coordinates WorldRootPosition = new Coordinates(-SpawningWorldEdgeLength / 2d, 0d, -SpawningWorldEdgeLength / 2d);
        public static float SimulationTickInterval = 1f;
        public static Vector3 InvalidPosition = Vector3.one * -9999;

        // Tree
        public static int TreeMaxHealth = 3;
        public static int HarvestReturnQuantity = 1;
        public static int TreeBurningTimeSecs = 10;
        public static int TreeStumpRegrowthTimeSecs = 300;
        public static int BurntTreeRegrowthTimeSecs = 600;
        public static float TreeIgnitionTimeBuffer = 0.4f;
        public static float TreeExtinguishTimeBuffer = 1f;
        public static float TreeCutDownTimeBuffer = 1f;

        // Entity counts
        public static int AttemptedTreeCount = 20000;
        public static float TreeJitter = 9.0f;
        public static int NPCCount = 40;

        // Component Updates
        public static int TransformUpdatesToSkipBetweenSends = 5;
        public static float AngleQuantisationFactor = 2f;

        // Worker Connection
        public const int TargetFramerateUnityWorker = 60;
        public static int TargetFramerate = 120;
        public static int FixedFramerate = 10;
        public static float PlayerHeartbeatInterval = 10f;
        public static float PlayerDeletionInterval = 10f;
        public static float PlayerHeartbeatInactivityMaxDuration = 30f;

        // Player Life Cycle
        public static int PlayerMaxHealth = 10;
        public static float PlayerRespawnDelay = 4f;

        // Fire
        public static float FireSpreadInterval = 1f;
        public static float FireSpreadRadius = 6f;
        public static float FireSpreadProbability = 0.5f;
        public static float DefaultFireDamageInterval = 1f;
        public static int FireDamagePerTick = 1;
        public static float OnFireMovementSpeedIncreaseFactor = 3f;

        // Player Controls
        public static float PlayerMovementSpeed = 5f;
        public static float PlayerPositionUpdateMinSqrDistance = 0.001f;
        public static float PlayerPositionUpdateMaxSqrDistance = PlayerMovementSpeed * PlayerMovementSpeed * OnFireMovementSpeedIncreaseFactor * OnFireMovementSpeedIncreaseFactor * 4f;
        public static float MaxRaycastDistance = 40f;
        public static float PlayerMovementTargetSlowingThreshold = 0.005f;

        // Player Spells
        public static float PlayerSpellCastRange = 10f;
        public static float PlayerSpellAOEDiameter = 4f;
        public static int MaxSpellTargets = 64;
        public static float SpellEffectDuration = 2f;
        public static float DeathEffectDuration = 1f;
        public static float SpellCooldown = 10f;
        public static float RainCloudSpawnHeight = 7f;
        public static float PlayerCastAnimationTime = 0.7f;
        public static float PlayerCastAnimationBuffer = 0.5f;
        public static float SpellIndicatorRotationSpeed = 80f;

        // Player Input Buttons
        public static int CastSpellMouseButton = 0;
        public static int RotateCameraMouseButton = 1;
        public static KeyCode CastLightningKey = KeyCode.E;
        public static KeyCode CastRainKey = KeyCode.R;
        public static KeyCode AbortKey = KeyCode.Escape;

        // Camera Controls
        public static float CameraSensitivity = 4f;
        public static float CameraDefaultDistance = 20f;
        public static float CameraMinDistance = 16f;
        public static float CameraMaxDistance = 22f;
        public static float CameraDistanceSensitivity = 2f;
        public static float CameraMaxPitch = 65f;
        public static float CameraInitialPitch = 15f;
        public static float CameraMinPitch = 27f;
        public static float CameraDefaultPitch = 30f;

        // NPC
        public static bool NPCDeathActive = true;
        public static int LumberjackMaxHealth = 5;
        public static int WizardMaxHealth = 5;
        public static int NPCSpawningWorldEdgeLength = Mathf.CeilToInt(4 * (float)SpawningWorldEdgeLength / 5);
        public static float NPCMovementSpeed = 10f;
        public static float NPCInteractionDelay = 0.2f;
        public static float NPCChoppingAnimationStartDelay = 0.2f;
        public static float NPCChoppingAnimationFinishDelay = 2f;
        public static float NPCStockpilingAnimationStartDelay = 0.2f;
        public static float NPCStockpilingAnimationFinishdelay = 2f;
        public static float NPCWizardSpellCastingSqrDistance = 36f;
        public static float NPCWanderWaypointDistance = 10f;
        public static float NPCOnFireWaypointDistance = 10f;
        public static float NPCPerceptionRefreshInterval = 0.5f;
        public static float NPCSpawnDistanceToHQ = 30f;
        public static float NPCDefaultInteractionSqrDistance = 9f;
        public static float NPCViewRadius = 30f;

        // Buildings
        public static int HQMaxHealth = 20;
        public static int BarracksMaxHealth = 10;
        public static float LumberjackSpawningCooldown = 30f;
        public static float WizardSpawningCooldown = 15;
        public static float SpawnOffsetFactor = 5f;
        public static float PlayerSpawnOffsetFactor = 48.0f;
        public static int HQStartingLumberjacksCount = 20;
        public static int HQStartingWizardsCount = 0;
        public static float DefaultHQBarracksSpawnRadius = 25f;
        public static float MaxHQBarracksSpawnRadius = 200f;
        public static float HQBarracksSpawnRadiusIncrease = 10f;
        public static int HQBarracksSpawnPositionPickingRetries = 10;
        public static float HQBarracksSpawnPositionSamplingHeight = 10f;
        public static float HQBarracksSpawnPositionSamplingRadius = 10f;
        public static float NPCWizardDefensivePriority = 0.8f;
        public static float HQBarracksSpawningSeparation = 15f;


        // Heartbeats
        public const uint DefaultHeartbeatsBeforeTimeout = 3;
        public const float HeartbeatCheckInterval = 3.0f;

        // Teams
        public const int TeamCount = 2;
        public const int RedTeamId = 0;
        public const int BlueTeamId = 1;
        public static Color RedTeamColor = new Color(0.917f, 0.329f, 0.329f);
        public static Color BlueTeamColor = new Color(0.102f, 0.455f, 0.859f);
        public static Color[] TeamColors = {RedTeamColor, BlueTeamColor};

        public static Coordinates[] TeamHQLocations =
        {
            new Coordinates(-300.0, 0.0, -275.0),
            new Coordinates(230.0, 0.0, 170.0)
        };

        public static EntityId[] HQEntityIds =
        {
            new EntityId(0), new EntityId(1)
        };

        // Audio Volume
        public static float NPCChopVolume = 0.6f;
        public static float RainVolume = 0.8f;
        public static float LightningStrikeVolume = 0.4f;
        public static float IgnitionVolume = 1f;
        public static float FireVolume = 1f;
        public static float ExtinguishVolume = 0.4f;
        public static float SpellChannelVolume = 0.8f;
        public static float FootstepVolume = 0.8f;

        // Animation
        public static float DeathEffectSpawnHeight = 1f;
    }
}
