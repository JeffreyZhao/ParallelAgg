namespace ParallelAgg.Aggregation {

    using ParallelAgg.Metadata;

    public interface IPropertyAggregator {

        void Add(ValueTuple values);

        void Remove(ValueTuple values);

        void Update(ValueTuple oldValues, PropertyMetadata property, decimal newValue);

        decimal Value { get; }
    }
}