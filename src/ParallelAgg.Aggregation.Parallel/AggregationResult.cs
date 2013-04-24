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

        private readonly EntityMetadata _metadata;
        private readonly AggregationConfig _config;
        private readonly int _keyIndex;

        private readonly IPropertyAggregator[] _aggregators;
        private readonly IDictionary<int, AggregationResult> _groupResults;

        private readonly CancellationTokenSource _cts;
        private readonly ActionBlock<AggregationChange> _changeBlock;

        private int _pendingCount;

        public AggregationResult(EntityMetadata metadata, AggregationConfig config, int keyIndex) {
            _metadata = metadata;
            _config = config;
            _keyIndex = keyIndex;
            _aggregators = config.Aggregators.Select(c => c.CreateAggregator()).ToArray();
            _groupResults = keyIndex < metadata.KeyCount ? new ConcurrentDictionary<int, AggregationResult>() : null;

            _cts = new CancellationTokenSource();

            var executionOptions = new ExecutionDataflowBlockOptions {
                CancellationToken = _cts.Token,
                SingleProducerConstrained = true
            };

            _changeBlock = new ActionBlock<AggregationChange>((Action<AggregationChange>)ProcessChange, executionOptions);
        }

        public int Count { get; private set; }

        private bool HasGroupResults {
            get { return _groupResults != null; }
        }

        private void ProcessChange(AggregationChange change) {
            switch (change.Type) {
                case ChangeType.Add: ProcessAdd(change); break;
                case ChangeType.Remove: ProcessRemove(change); break;
                default: ProcessUpdate(change); break;
            }

            Interlocked.Decrement(ref _pendingCount);
        }

        private void ProcessAdd(AggregationChange change) {
            Debug.Assert(change.Type == ChangeType.Add);

            if (HasGroupResults) {
                AggregationResult groupResult;

                var key = change.Entity.GetKey(_keyIndex);
                if (!_groupResults.TryGetValue(key, out groupResult)) {
                    groupResult = new AggregationResult(_metadata, _config, _keyIndex + 1);
                    _groupResults.Add(key, groupResult);
                }

                groupResult.Post(change);
            }

            foreach (var u in change.Updates) {
                _aggregators[u.Config.Index].Add(u.Values);
            }
        }

        private void ProcessRemove(AggregationChange change) {
            Debug.Assert(change.Type == ChangeType.Remove);

            if (HasGroupResults) {
                var key = change.Entity.GetKey(_keyIndex);
                var groupResult = _groupResults[key];
                groupResult.Post(change);

                if (groupResult.Count == 0) {
                    _groupResults.Remove(key);
                    groupResult.Dispose();
                }
            }

            foreach (var u in change.Updates) {
                _aggregators[u.Config.Index].Remove(u.Values);
            }
        }

        private void ProcessUpdate(AggregationChange change) {
            Debug.Assert(change.Type == ChangeType.Update);

            if (HasGroupResults) {
                var key = change.Entity.GetKey(_keyIndex);
                var groupResult = _groupResults[key];
                groupResult.Post(change);
            }

            foreach (var u in change.Updates) {
                _aggregators[u.Config.Index].Update(u.Values, change.Property, change.NewValue);
            }
        }

        public void Post(AggregationChange change) {
            Interlocked.Increment(ref _pendingCount);

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
            get { return HasGroupResults ? _groupResults.Keys : Enumerable.Empty<int>(); }
        }

        public bool Running {
            get {
                if (Thread.VolatileRead(ref _pendingCount) > 0) return true;

                return HasGroupResults && _groupResults.Values.Any(r => r.Running);
            }
        }

        public void Dispose() {
            _cts.Cancel();
        }
    }
}
