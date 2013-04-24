namespace ParallelAgg.Aggregation.Parallel {

    using System.Collections.Generic;
    using ParallelAgg.Metadata;

    internal class AggregationChange {

        public static AggregationChange Add(Entity entity, ICollection<PropertyAggregatorUpdate> updates) {
            return new AggregationChange(ChangeType.Add, entity, updates);
        }

        public static AggregationChange Remove(Entity entity, ICollection<PropertyAggregatorUpdate> updates) {
            return new AggregationChange(ChangeType.Remove, entity, updates);
        }

        public static AggregationChange Update(Entity entity, ICollection<PropertyAggregatorUpdate> updates, PropertyMetadata property, decimal newValue) {
            return new AggregationChange(ChangeType.Update, entity, updates, property, newValue);
        }

        public readonly ChangeType Type;
        public readonly Entity Entity;
        public readonly ICollection<PropertyAggregatorUpdate> Updates;
        public readonly PropertyMetadata Property;
        public readonly decimal NewValue;

        private AggregationChange(ChangeType type, Entity entity, ICollection<PropertyAggregatorUpdate> updates, PropertyMetadata property = null, decimal newValue = default(decimal)) {
            Type = type;
            Entity = entity;
            Updates = updates;
            Property = property;
            NewValue = newValue;
        }
    }
}
