namespace ParallelAgg.Aggregation {

    using System.Diagnostics;
    using ParallelAgg.Metadata;

    internal class AvgPropertyAggregator : IPropertyAggregator {

        private readonly PropertyMetadata _property;

        private decimal _sum;
        private int _count;

        public AvgPropertyAggregator(PropertyMetadata property) {
            _property = property;
        }

        public void Add(ValueTuple values) {
            _sum += values.Value0;
            _count++;
        }

        public void Remove(ValueTuple values) {
            _sum -= values.Value0;
            _count--;
        }

        public void Update(ValueTuple oldValues, PropertyMetadata property, decimal newValue) {
            Debug.Assert(_property == property);
            _sum = _sum - oldValues.Value0 + newValue;
        }

        public decimal Value {
            get { return _sum / _count; }
        }
    }
}
