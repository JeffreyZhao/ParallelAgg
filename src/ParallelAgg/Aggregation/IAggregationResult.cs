namespace ParallelAgg.Aggregation {

    public interface IAggregationResult {
        decimal Get(PropertyAggregatorConfig config);
        IAggregationResult Get(int key);
    }

}