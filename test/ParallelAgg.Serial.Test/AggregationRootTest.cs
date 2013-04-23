namespace ParallelAgg.Serial.Test {

    using System.Linq;
    using ParallelAgg.Aggregation;
    using ParallelAgg.Metadata;
    using Xunit;
    
    public class AggregationRootTest {

        private readonly PropertyMetadata _property0;
        private readonly PropertyMetadata _property1;
        private readonly PropertyMetadata _property2;

        private readonly WgtAvgPropertyAggregatorConfig _aggregatorConfig0;
        private readonly WgtAvgPropertyAggregatorConfig _aggregatorConfig1;
        private readonly WgtAvgPropertyAggregatorConfig _aggregatorConfig2;

        private readonly Entity _entity0;
        private readonly Entity _entity1;
        private readonly Entity _entity2;

        private readonly EntityMetadata _metadata;
        private readonly EntitySet _set;
        private readonly AggregationResult _rootResult;

        private Entity CreateEntity(int key, decimal value0, decimal value1, decimal value2) {
            var entity = _metadata.CreateEntity(new[] { key });
            entity.Set(_property0, value0);
            entity.Set(_property1, value1);
            entity.Set(_property2, value2);

            return entity;
        }

        public AggregationRootTest() {
            _property0 = new PropertyMetadata(0);
            _property1 = new PropertyMetadata(1);
            _property2 = new PropertyMetadata(2);

            _aggregatorConfig0 = new WgtAvgPropertyAggregatorConfig(0, _property0, _property1);
            _aggregatorConfig1 = new WgtAvgPropertyAggregatorConfig(1, _property1, _property2);
            _aggregatorConfig2 = new WgtAvgPropertyAggregatorConfig(2, _property0, _property2);

            _metadata = new EntityMetadata(1, new[] { _property0, _property1, _property2 });

            _entity0 = CreateEntity(0, 10, 20, 30);
            _entity1 = CreateEntity(1, 11, 21, 31);
            _entity2 = CreateEntity(2, 12, 22, 32);

            _set = new EntitySet { _entity0, _entity1 };

            var config = new AggregationConfig(_metadata, new[] { _aggregatorConfig0, _aggregatorConfig1, _aggregatorConfig2 });
            var root = new AggregationRoot(_set, _metadata, config);
            root.Start();

            _set.Add(_entity2);

            _rootResult = root.Result;
        }

        private decimal CalculateWgtAvg(Entity[] entities, PropertyMetadata valueMetadata, PropertyMetadata weightMetadata) {
            return entities.Sum(e => e.Get(valueMetadata) * e.Get(weightMetadata)) / entities.Sum(e => e.Get(weightMetadata));
        }

        private void Verify(Entity[] entities, AggregationResult result, WgtAvgPropertyAggregatorConfig config) {
            Assert.Equal(CalculateWgtAvg(entities, config.Properties[0], config.Properties[1]), result.Get(config));
        }

        private void Verify(AggregationResult result, params Entity[] entities) {
            Verify(entities, result, _aggregatorConfig0);
            Verify(entities, result, _aggregatorConfig1);
            Verify(entities, result, _aggregatorConfig2);
        }

        [Fact]
        public void Verify() {
            Verify(_rootResult, _entity0, _entity1, _entity2);
            Verify(_rootResult.Get(0), _entity0);
            Verify(_rootResult.Get(1), _entity1);
            Verify(_rootResult.Get(2), _entity2);
        }

        [Fact]
        public void Remove() {
            _set.Remove(_entity0);
            _set.Remove(_entity2);

            Verify(_rootResult, _entity1);
            Verify(_rootResult.Get(1), _entity1);

            Assert.Null(_rootResult.Get(0));
            Assert.Null(_rootResult.Get(2));
        }

        [Fact]
        public void Update() {
            _entity0.Set(_property0, 100);
            _entity1.Set(_property1, 200);
            _entity2.Set(_property2, 300);

            Verify();
        }
    }
}
