namespace ParallelAgg.Aggregation {

    using ParallelAgg.Metadata;

    public class AvgPropertyAggregatorConfig : PropertyAggregatorConfig {

        public AvgPropertyAggregatorConfig(int index, PropertyMetadata property)
            : base(index, "Avg", property) { }

        public override IPropertyAggregator CreateAggregator() {
            return new AvgPropertyAggregator(Properties[0]);
        }
    }
}
