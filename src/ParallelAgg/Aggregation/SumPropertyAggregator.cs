namespace ParallelAgg.Aggregation {

    using System.Diagnostics;
    using ParallelAgg.Metadata;

    public class SumPropertyAggregator : IPropertyAggregator {

        private readonly PropertyMetadata _property;

        public SumPropertyAggregator(PropertyMetadata property) {
            _property = property;
        }

        public void Add(ValueTuple values) {
            Value += values.Value0;
        }

        public void Remove(ValueTuple values) {
            Value -= values.Value0;
        }

        public void Update(ValueTuple oldValues, PropertyMetadata property, decimal newValue) {
            Debug.Assert(_property == property);
            Value = Value - oldValues.Value0 + newValue;
        }

        public decimal Value { get; private set; }
    }
}
