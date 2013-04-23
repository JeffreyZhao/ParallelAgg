namespace ParallelAgg.Test.Aggregation {

    using ParallelAgg.Aggregation;
    using ParallelAgg.Metadata;
    using Xunit;

    public class AggregationConfigTest {

        private readonly PropertyMetadata _property0;
        private readonly PropertyMetadata _property1;
        private readonly PropertyMetadata _property2;

        private readonly WgtAvgPropertyAggregatorConfig _aggregatorConfig0;
        private readonly WgtAvgPropertyAggregatorConfig _aggregatorConfig1;
        private readonly WgtAvgPropertyAggregatorConfig _aggregatorConfig2;

        private readonly AggregationConfig _config;

        public AggregationConfigTest() {
            _property0 = new PropertyMetadata(0);
            _property1 = new PropertyMetadata(1);
            _property2 = new PropertyMetadata(2);

            _aggregatorConfig0 = new WgtAvgPropertyAggregatorConfig(0, _property0, _property1);
            _aggregatorConfig1 = new WgtAvgPropertyAggregatorConfig(1, _property1, _property2);
            _aggregatorConfig2 = new WgtAvgPropertyAggregatorConfig(2, _property0, _property2);

            _config = new AggregationConfig(
                new EntityMetadata(1, new[] { _property0, _property1, _property2 }),
                new[] { _aggregatorConfig0, _aggregatorConfig1, _aggregatorConfig2 });
        }

        [Fact]
        public void GetAggregatorsUpdateWith() {
            Assert.Equal(new[]{_aggregatorConfig0, _aggregatorConfig2}, _config.GetAggregatorsUpdateWith(_property0));
            Assert.Equal(new[]{_aggregatorConfig0, _aggregatorConfig1}, _config.GetAggregatorsUpdateWith(_property1));
            Assert.Equal(new[]{_aggregatorConfig1, _aggregatorConfig2}, _config.GetAggregatorsUpdateWith(_property2));
        }
    }
}
