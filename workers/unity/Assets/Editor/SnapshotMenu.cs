using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [InitializeOnLoad]
    public static class SnapshotMenu
    {
        private static Action improbableBuild;

        static SnapshotMenu()
        {
            improbableBuild = Improbable.Unity.EditorTools.Build.SimpleBuildSystem.BuildAction;
            Improbable.Unity.EditorTools.Build.SimpleBuildSystem.BuildAction = InjectBuild;
        }

        [MenuItem("Improbable/Snapshots/Generate Default Snapshot %#&w")]
        private static void GenerateSnapshotDefault()
        {
            var path = Application.dataPath + "/../../../snapshots/";
            var snapshot = new SnapshotBuilder("default.snapshot", path);
            SnapshotDefault.Build(snapshot);
            snapshot.SaveSnapshot();
        }

        [MenuItem("Improbable/Snapshots/Generate Benchmark Snapshot")]
        private static void GenerateSnapshotBenchmark()
        {
            var path = Application.dataPath + "/../../../snapshots/";
            var snapshot = new SnapshotBuilder("benchmark.snapshot", path);
            SnapshotBenchmark.Build(snapshot);
            snapshot.SaveSnapshot();
        }

        private static void InjectBuild()
        {
            improbableBuild();
            GenerateSnapshotDefault();
        }
    }
}
