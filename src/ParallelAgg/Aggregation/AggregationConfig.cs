namespace ParallelAgg.Aggregation {

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using ParallelAgg.Metadata;

    public class AggregationConfig {

        private readonly ReadOnlyCollection<PropertyAggregatorConfig> _aggregators;
        private readonly ReadOnlyCollection<PropertyAggregatorConfig>[] _aggregatorsUpdateWith; 

        public AggregationConfig(EntityMetadata metadata, IEnumerable<PropertyAggregatorConfig> aggregators) {

            var aggregatorList = aggregators.ToList();

            for (var i = 0; i < aggregatorList.Count; i++) {
                Debug.Assert(aggregatorList[i].Index == i);
            }

            _aggregatorsUpdateWith = new ReadOnlyCollection<PropertyAggregatorConfig>[metadata.Properties.Count];

            var aggregatorsGroupByProperty = aggregatorList
                .SelectMany(a => a.Properties.Select(p => Tuple.Create(p, a)))
                .GroupBy(t => t.Item1);

            foreach (var g in aggregatorsGroupByProperty) {
                var property = g.Key;
                Debug.Assert(metadata.Properties.Contains(property));

                _aggregatorsUpdateWith[property.Index] = new ReadOnlyCollection<PropertyAggregatorConfig>(g.Select(t => t.Item2).ToList());
            }

            _aggregators = new ReadOnlyCollection<PropertyAggregatorConfig>(aggregatorList);
        }

        public ReadOnlyCollection<PropertyAggregatorConfig> Aggregators {
            get { return _aggregators; }
        }

        public ReadOnlyCollection<PropertyAggregatorConfig> GetAggregatorsUpdateWith(PropertyMetadata property) {
            return _aggregatorsUpdateWith[property.Index];
        }
    }
}
