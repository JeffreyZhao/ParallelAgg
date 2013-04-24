namespace ParallelAgg.Aggregation.Parallel {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks.Dataflow;
    using ParallelAgg.Metadata;

    internal class AggregationResult : IAggregationResult, IDisposable {

        private struct Change {

            private readonly AggregationChange _change;
            private readonly ValueTuple _values;

            public Change(AggregationChange change, ValueTuple values) {
                _change = change;
                _values = values;
            }

            public ChangeType Type {
                get { return _change.Type; }
            }

            public ValueTuple Values {
                get { return _values; }
            }

            public PropertyMetadata Property {
                get { return _change.Property; }
            }

            public decimal NewValue {
                get { return _change.NewValue; }
            }
        }

        private readonly EntityMetadata _metadata;
        private readonly AggregationConfig _config;
        private readonly int _keyIndex;

        private readonly CancellationTokenSource _cts;

        private readonly IPropertyAggregator[] _aggregators;
        private readonly ActionBlock<Change>[] _aggregatorBlocks;

        private readonly ActionBlock<AggregationChange> _changeBlock; 
        private readonly IDictionary<int, AggregationResult> _groupResults;

        public AggregationResult(EntityMetadata metadata, AggregationConfig config, int keyIndex) {
            _metadata = metadata;
            _config = config;
            _keyIndex = keyIndex;
            _groupResults = keyIndex < metadata.KeyCount ? new ConcurrentDictionary<int, AggregationResult>() : null;

            _cts = new CancellationTokenSource();

            var executionOptions = new ExecutionDataflowBlockOptions {
                CancellationToken = _cts.Token,
                SingleProducerConstrained = true
            };

            _changeBlock = new ActionBlock<AggregationChange>((Action<AggregationChange>)ProcessChange, executionOptions);

            _aggregators = new IPropertyAggregator[config.Aggregators.Count];
            _aggregatorBlocks = new ActionBlock<Change>[config.Aggregators.Count];

            for (var i = 0; i < config.Aggregators.Count; i++) {
                var aggregator = config.Aggregators[i].CreateAggregator();
                _aggregators[i] = aggregator;
                _aggregatorBlocks[i] = new ActionBlock<Change>(c => ProcessAggregatorChange(aggregator, c), executionOptions);
            }
        }

        public int Count { get; private set; }

        private bool NoGroupResults {
            get { return _groupResults == null; }
        }

        private void ProcessChange(AggregationChange change) {
            switch (change.Type) {
                case ChangeType.Add: ProcessAdd(change); break;
                case ChangeType.Remove: ProcessRemove(change); break;
                default: ProcessUpdate(change); break;
            }
        }

        private void ProcessAggregatorChange(IPropertyAggregator aggregator, Change change) {
            switch (change.Type) {
                case ChangeType.Add:
                    aggregator.Add(change.Values);
                    break;
                case ChangeType.Remove:
                    aggregator.Remove(change.Values);
                    break;
                default:
                    Debug.Assert(change.Type == ChangeType.Update);
                    aggregator.Update(change.Values, change.Property, change.NewValue);
                    break;
            }
        }

        private void ProcessAdd(AggregationChange change) {
            Debug.Assert(change.Type == ChangeType.Add);

            foreach (var u in change.Updates) {
                _aggregatorBlocks[u.Config.Index].Post(new Change(change, u.Values));
            }

            if (NoGroupResults) return;

            AggregationResult groupResult;

            var key = change.Entity.GetKey(_keyIndex);
            if (!_groupResults.TryGetValue(key, out groupResult)) {
                groupResult = new AggregationResult(_metadata, _config, _keyIndex + 1);
                _groupResults.Add(key, groupResult);
            }

            groupResult.Post(change);
        }

        private void ProcessRemove(AggregationChange change) {
            Debug.Assert(change.Type == ChangeType.Remove);

            foreach (var u in change.Updates) {
                _aggregatorBlocks[u.Config.Index].Post(new Change(change, u.Values));
            }

            if (NoGroupResults) return;

            var key = change.Entity.GetKey(_keyIndex);
            var groupResult = _groupResults[key];
            groupResult.Post(change);

            if (groupResult.Count == 0) {
                _groupResults.Remove(key);
                groupResult.Dispose();
            }
        }

        private void ProcessUpdate(AggregationChange change) {
            Debug.Assert(change.Type == ChangeType.Update);

            foreach (var u in change.Updates) {
                _aggregatorBlocks[u.Config.Index].Post(new Change(change, u.Values));
            }

            if (NoGroupResults) return;

            var key = change.Entity.GetKey(_keyIndex);
            var groupResult = _groupResults[key];
            groupResult.Post(change);
        }

        public void Post(AggregationChange change) {
            if (change.Type == ChangeType.Add)
                Count++;
            else if (change.Type == ChangeType.Remove)
                Count--;

            _changeBlock.Post(change);
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

        public IEnumerable<int> Keys {
            get { return NoGroupResults ? Enumerable.Empty<int>() : _groupResults.Keys; }
        }

        public void WaitForCompletion() {
            _changeBlock.Complete();
            _changeBlock.Completion.Wait();

            foreach (var block in _aggregatorBlocks) {
                block.Complete();
                block.Completion.Wait();
            }

            if (NoGroupResults) return;

            foreach (var result in _groupResults.Values) {
                result.WaitForCompletion();
            }
        }

        public void Dispose() {
            _cts.Cancel();
        }
    }
}
