namespace ParallelAgg.Aggregation {

    using ParallelAgg.Metadata;

    public interface IAggregationService {
        IAggregationResult Aggregate(EntitySet set, EntityMetadata metadata, AggregationConfig config);
    }
}
