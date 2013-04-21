namespace ParallelAgg.Aggregation {

    using ParallelAgg.Metadata;

    public class WgtAvgPropertyAggregatorConfig : PropertyAggregatorConfig {

        public WgtAvgPropertyAggregatorConfig(int index, PropertyMetadata valueProperty, PropertyMetadata weightProperty)
            : base(index, "WgtAvg", valueProperty, weightProperty) { }

        public override IPropertyAggregator CreateAggregator() {
            return new WgtAvgPropertyAggregator(Properties[0], Properties[1]);
        }
    }
}
