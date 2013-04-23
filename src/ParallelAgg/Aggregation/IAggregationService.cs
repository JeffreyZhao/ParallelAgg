namespace ParallelAgg.Aggregation {

    using ParallelAgg.Metadata;

    public interface IAggregationService {
        IAggregationRoot Aggregate(EntitySet set, EntityMetadata metadata, AggregationConfig config);
    }
}
