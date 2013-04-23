namespace ParallelAgg.Serial {

    using ParallelAgg.Aggregation;
    using ParallelAgg.Metadata;

    public class AggregationService : IAggregationService {
        public IAggregationRoot Aggregate(EntitySet set, EntityMetadata metadata, AggregationConfig config) {
            return new AggregationRoot(set, metadata, config);
        }
    }
}