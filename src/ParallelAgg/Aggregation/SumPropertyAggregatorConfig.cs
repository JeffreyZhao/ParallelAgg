namespace ParallelAgg.Aggregation {

    using ParallelAgg.Metadata;

    public class SumPropertyAggregatorConfig : PropertyAggregatorConfig {

        public SumPropertyAggregatorConfig(int index, PropertyMetadata property)
            : base(index, "Sum", property) { }

        public override IPropertyAggregator CreateAggregator() {
            return new SumPropertyAggregator(Properties[0]);
        }
    }
}
