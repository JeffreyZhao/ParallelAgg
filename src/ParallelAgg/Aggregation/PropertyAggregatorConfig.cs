namespace ParallelAgg.Aggregation {

    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using ParallelAgg.Metadata;

    public abstract class PropertyAggregatorConfig {

        private readonly int _index;
        private readonly ReadOnlyCollection<PropertyMetadata> _properties;
        private readonly string _name;
 
        protected PropertyAggregatorConfig(int index, string functionName, params PropertyMetadata[] properties) {
            Debug.Assert(properties.Length == 1 || properties.Length == 2);

            _index = index;
            _name = String.Format("{0}({1})", functionName, String.Join(", ", properties.Select(p => p.Name)));
            _properties = new ReadOnlyCollection<PropertyMetadata>(properties.ToList());
        }

        public int Index {
            get { return _index; }
        }

        public string Name {
            get { return _name; }
        }

        public ReadOnlyCollection<PropertyMetadata> Properties {
            get { return _properties; }
        }

        public ValueTuple CaptureValues(Entity entity) {
            return _properties.Count == 1
                ? new ValueTuple(entity.Get(_properties[0]))
                : new ValueTuple(entity.Get(_properties[0]), entity.Get(_properties[1]));
        }

        public abstract IPropertyAggregator CreateAggregator();

        public override string ToString() {
            return Name;
        }
    }
}
