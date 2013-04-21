namespace ParallelAgg.Aggregation {

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    public class AggregationConfig {

        private readonly ReadOnlyCollection<PropertyAggregatorConfig> _aggregators;

        public AggregationConfig(IEnumerable<PropertyAggregatorConfig> aggregators) {

            var aggregatorList = aggregators.ToList();

            for (var i = 0; i < aggregatorList.Count; i++) {
                Debug.Assert(aggregatorList[i].Index == i);
            }

            _aggregators = new ReadOnlyCollection<PropertyAggregatorConfig>(aggregatorList);
        }

        public ReadOnlyCollection<PropertyAggregatorConfig> Aggregators {
            get { return _aggregators; }
        } 
    }
}
