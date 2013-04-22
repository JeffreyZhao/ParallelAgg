namespace ParallelAgg.Metadata {

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    public class EntityMetadata {

        private readonly int _keyCount;
        private readonly ReadOnlyCollection<PropertyMetadata> _properties;

        public EntityMetadata(int keyCount, IEnumerable<PropertyMetadata> properties) {
            _keyCount = keyCount;
            _properties = new ReadOnlyCollection<PropertyMetadata>(properties.ToList());
        }

        public int KeyCount {
            get { return _keyCount; }
        }

        public ReadOnlyCollection<PropertyMetadata> Properties {
            get { return _properties; }
        }

        public Entity CreateEntity(int[] keys) {
            Debug.Assert(keys.Length == _keyCount);

            return new Entity(keys, _properties.Count);
        }
    }
}
