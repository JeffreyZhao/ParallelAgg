using System;
using System.Collections.Generic;
namespace ParallelAgg.Serial {

    using ParallelAgg.Aggregation;
    using ParallelAgg.Metadata;

    public class AggregationRoot {

        private readonly EntitySet _set;
        private readonly EntityMetadata _metadata;
        private readonly AggregationConfig _config;

        public AggregationRoot(EntitySet set, EntityMetadata metadata, AggregationConfig config) {
            _set = set;
            _metadata = metadata;
            _config = config;
        }

        public AggregationResult Result { get; private set; }

        public void Start() {
            Result = new AggregationResult(_metadata, _config, 0);

            foreach (var entity in _set) {
                AddEntity(entity);
            }

            _set.EntityAdded += (_, args) => AddEntity(args.Entity);
            _set.EntityRemoved += (_, args) => RemoveEntity(args.Entity);
        }

        private void AddEntity(Entity entity) {
            
        }

        private void RemoveEntity(Entity entity) {
            
        }

        private void UpdateEntity(Entity entity) {
            
        }
    }
}