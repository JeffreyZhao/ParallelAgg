namespace ParallelAgg.Serial {

    using System;
    using ParallelAgg.Aggregation;
    using ParallelAgg.Metadata;

    public class AggregationService : IAggregationService {
        public IAggregationResult Aggregate(EntitySet set, EntityMetadata metadata, AggregationConfig config) {
            throw new NotImplementedException();
        }
    }
}