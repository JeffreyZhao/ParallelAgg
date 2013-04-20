namespace ParallelAgg.Aggregation {

    using System.Diagnostics;
    using ParallelAgg.Metadata;

    public class WgtAvgPropertyAggregator : IPropertyAggregator {

        private readonly PropertyMetadata _valueProperty;
        private readonly PropertyMetadata _weightProperty;

        private decimal _sum;
        private decimal _weightSum;

        public WgtAvgPropertyAggregator(PropertyMetadata valueProperty, PropertyMetadata weightProperty) {
            _valueProperty = valueProperty;
            _weightProperty = weightProperty;
        }

        public void Add(ValueTuple values) {
            var value = values.Value0;
            var weight = values.Value1;

            _sum += value * weight;
            _weightSum += weight;
        }

        public void Remove(ValueTuple values) {
            var value = values.Value0;
            var weight = values.Value1;

            _sum -= value * weight;
            _weightSum -= weight;
        }

        public void Update(ValueTuple oldValues, PropertyMetadata property, decimal newValue) {
            Debug.Assert(property == _valueProperty || property == _weightProperty);

            var oldValue = oldValues.Value0;
            var oldWeight = oldValues.Value1;

            if (property == _valueProperty) {
                _sum += (newValue - oldValue) * oldWeight;
            }
            else {
                _sum += oldValue * (newValue - oldWeight);
                _weightSum += newValue - oldWeight;
            }
        }

        public decimal Value {
            get { return _sum / _weightSum; }
        }
    }
}
