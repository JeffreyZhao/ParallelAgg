namespace ParallelAgg.Aggregation {

    public interface IAggregationService {
        IAggregationResult Aggregate(EntitySet set, AggregationConfig config);
    }
}
