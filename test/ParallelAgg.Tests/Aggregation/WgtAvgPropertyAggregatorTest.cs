namespace ParallelAgg.Tests.Aggregation {

    using ParallelAgg.Aggregation;
    using ParallelAgg.Metadata;
    using Xunit;

    public class WgtAvgPropertyAggregatorTest {

        private readonly PropertyMetadata _valueProperty;
        private readonly PropertyMetadata _weightProperty;
        private readonly WgtAvgPropertyAggregator _aggregator;

        public WgtAvgPropertyAggregatorTest() {
            _valueProperty = new PropertyMetadata(0);
            _weightProperty = new PropertyMetadata(1);
            _aggregator = new WgtAvgPropertyAggregator(_valueProperty, _weightProperty);
        }

        [Fact]
        public void Add() {
            _aggregator.Add(new ValueTuple(10, 5));
            Assert.Equal(10m, _aggregator.Value);

            _aggregator.Add(new ValueTuple(8, 3));
            Assert.Equal(9.25m, _aggregator.Value);
        }

        [Fact]
        public void AddThenRemove() {
            _aggregator.Add(new ValueTuple(10, 5));
            _aggregator.Add(new ValueTuple(7, 2));
            _aggregator.Add(new ValueTuple(8, 3));
            _aggregator.Remove(new ValueTuple(7, 2));

            Assert.Equal(9.25m, _aggregator.Value);
        }

        [Fact]
        public void AddThenUpdate() {
            _aggregator.Add(new ValueTuple(10, 5));
            _aggregator.Add(new ValueTuple(7, 2));

            _aggregator.Update(new ValueTuple(7, 2), _valueProperty, 8m);
            _aggregator.Update(new ValueTuple(8, 2), _weightProperty, 3m);

            Assert.Equal(9.25m, _aggregator.Value);
        }
    }
}