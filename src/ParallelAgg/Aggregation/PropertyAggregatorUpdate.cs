namespace ParallelAgg.Aggregation {

    public struct PropertyAggregatorUpdate {

        private readonly PropertyAggregatorConfig _config;
        private readonly ValueTuple _values;

        public PropertyAggregatorUpdate(PropertyAggregatorConfig config, ValueTuple values) {
            _config = config;
            _values = values;
        }

        public PropertyAggregatorConfig Config {
            get { return _config; }
        }

        public ValueTuple Values {
            get { return _values; }
        }
    }
}
