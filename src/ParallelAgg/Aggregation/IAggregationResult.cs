namespace ParallelAgg.Aggregation {

    using System.Collections.Generic;

    public interface IAggregationResult {
        decimal Get(PropertyAggregatorConfig config);
        IAggregationResult Get(int key);
        IEnumerable<int> Keys { get; }
    }

}