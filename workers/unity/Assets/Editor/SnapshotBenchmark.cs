using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public static class SnapshotBenchmark
    {
        public static void Build(SnapshotBuilder snapshot)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Resources/full.png");
            
            SnapshotUtil.AddTrees(snapshot, texture, 0.35f, 10000, 500, 0);
			SnapshotUtil.AddPlayerSpawner(snapshot);
        }
    }
}