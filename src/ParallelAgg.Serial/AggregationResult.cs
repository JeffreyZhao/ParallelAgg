namespace ParallelAgg.Serial {

    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using ParallelAgg.Aggregation;
    using ParallelAgg.Metadata;

    public class AggregationResult : IAggregationResult {

        private readonly EntityMetadata _metadata;
        private readonly AggregationConfig _config;
        private readonly int _keyIndex;
        private readonly IPropertyAggregator[] _aggregators;
        private readonly IDictionary<int, AggregationResult> _groupResults;

        public AggregationResult(EntityMetadata metadata, AggregationConfig config, int keyIndex) {
            _metadata = metadata;
            _config = config;
            _keyIndex = keyIndex;
            _aggregators = config.Aggregators.Select(c => c.CreateAggregator()).ToArray();
            _groupResults = keyIndex < metadata.KeyCount ? new ConcurrentDictionary<int, AggregationResult>() : null;
        }

        public int Count { get; private set; }

        private bool NoGroupResults {
            get { return _groupResults == null; }
        }

        public void Add(Entity entity, ICollection<PropertyAggregatorUpdate> updates) {
            Count++;

            foreach (var u in updates) {
                _aggregators[u.Config.Index].Add(u.Values);
            }

            if (NoGroupResults) return;

            AggregationResult groupResult;

            var key = entity.GetKey(_keyIndex);
            if (!_groupResults.TryGetValue(key, out groupResult)) {
                groupResult = new AggregationResult(_metadata, _config, _keyIndex + 1);
                _groupResults.Add(key, groupResult);
            }

            groupResult.Add(entity, updates);
        }

        public void Remove(Entity entity, ICollection<PropertyAggregatorUpdate> updates) {
            Count--;

            foreach (var u in updates) {
                _aggregators[u.Config.Index].Remove(u.Values);
            }

            if (NoGroupResults) return;

            var key = entity.GetKey(_keyIndex);
            var groupResult = _groupResults[key];
            groupResult.Remove(entity, updates);

            if (groupResult.Count == 0) {
                _groupResults.Remove(key);
            }
        }

        public void Update(Entity entity, ICollection<PropertyAggregatorUpdate> updates, PropertyMetadata property, decimal newValue) {
            foreach (var u in updates) {
                _aggregators[u.Config.Index].Update(u.Values, property, newValue);
            }

            if (NoGroupResults) return;

            var key = entity.GetKey(_keyIndex);
            var groupResult = _groupResults[key];
            groupResult.Update(entity, updates, property, newValue);
        }

        public decimal Get(PropertyAggregatorConfig config) {
            return _aggregators[config.Index].Value;
        }

        public AggregationResult Get(int key) {
            AggregationResult result;
            return _groupResults.TryGetValue(key, out result) ? result : null;
        }

        IAggregationResult IAggregationResult.Get(int key) {
            return Get(key);
        }
    }
}