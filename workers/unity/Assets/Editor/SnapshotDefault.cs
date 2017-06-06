using Assets.Gamelogic.Core;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public static class SnapshotDefault
    {
        public static void Build(SnapshotBuilder snapshot)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Resources/perlin.png");
            
            SnapshotUtil.AddHQs(snapshot, SimulationSettings.TeamHQLocations);
            SnapshotUtil.AddNPCsAroundHQs(snapshot, SimulationSettings.TeamHQLocations);
            SnapshotUtil.AddTrees(snapshot, texture, 0.35f, SimulationSettings.AttemptedTreeCount, SimulationSettings.SpawningWorldEdgeLength, SimulationSettings.TreeJitter);
			SnapshotUtil.AddPlayerSpawner(snapshot);
        }
    }
}
