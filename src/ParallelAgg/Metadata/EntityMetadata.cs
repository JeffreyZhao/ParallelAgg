namespace ParallelAgg.Metadata {

    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    public class EntityMetadata {

        private readonly int _keyCount;
        private readonly ReadOnlyCollection<PropertyMetadata> _properties;

        public EntityMetadata(int keyCount, int propertyCount) {
            Debug.Assert(keyCount > 0);
            Debug.Assert(propertyCount > 0);

            _keyCount = keyCount;
            _properties = new ReadOnlyCollection<PropertyMetadata>(
                Enumerable.Range(0, propertyCount).Select(i => new PropertyMetadata(i)).ToList());
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
