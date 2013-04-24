namespace ParallelAgg.Aggregation {

    public interface IAggregationRoot {
        IAggregationResult Result { get; }
        void Start();
        void WaitForCompletion();
    }
}
