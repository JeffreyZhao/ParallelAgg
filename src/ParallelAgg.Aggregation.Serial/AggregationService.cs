namespace ParallelAgg.Aggregation.Serial {

    using ParallelAgg.Aggregation;
    using ParallelAgg.Metadata;

    public class AggregationService : IAggregationService {
        public IAggregationRoot Aggregate(EntitySet set, AggregationConfig config, EntityMetadata metadata) {
            return new AggregationRoot(set, config, metadata);
        }
    }
}