namespace ParallelAgg.Aggregation.Serial {

    using System.Collections.Generic;
    using ParallelAgg.Aggregation;
    using ParallelAgg.Metadata;

    internal class AggregationRoot : AggregationRootBase {

        private readonly AggregationResult _result;

        public AggregationRoot(EntitySet set, AggregationConfig config, EntityMetadata metadata)
            : base(set, config) {

            _result = new AggregationResult(metadata, config, 0);
        }

        public override IAggregationResult Result { get { return _result; } }

        public override bool Running {
            get { return false; }
        }

        protected override void AddEntity(Entity entity, ICollection<PropertyAggregatorUpdate> updates) {
            _result.Add(entity, updates);
        }

        protected override void RemoveEntity(Entity entity, ICollection<PropertyAggregatorUpdate> updates) {
            _result.Remove(entity, updates);
        }

        protected override void UpdateEntity(Entity entity, ICollection<PropertyAggregatorUpdate> updates, PropertyMetadata property, decimal newValue) {
            _result.Update(entity, updates, property, newValue);
        }
    }
}