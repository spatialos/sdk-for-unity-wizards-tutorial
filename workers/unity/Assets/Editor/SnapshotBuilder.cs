using Improbable;
using Improbable.Worker;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Editor
{
    public class SnapshotBuilder
    {
        private string snapshotPath;

        private int currentEntityId = 1;
        private IDictionary<EntityId, Entity> snapshotEntities = new Dictionary<EntityId, Entity>();

        public SnapshotBuilder(string name, string path)
        {
            snapshotPath = path + name;
        }

        public void SaveSnapshot()
        {
            File.Delete(snapshotPath);
            var result = Snapshot.Save(snapshotPath, snapshotEntities);

            if (result.HasValue)
            {
                Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", result.Value);
            }
            else
            {
                Debug.LogFormat("Successfully generated initial world snapshot at {0} with {1} entities", snapshotPath, currentEntityId);
            }
        }

        public void Add(EntityId id, Entity entity)
        {
            snapshotEntities.Add(id, entity);
        }

        public EntityId GenerateId()
        {
            return new EntityId(currentEntityId++);
        }
    }
}
