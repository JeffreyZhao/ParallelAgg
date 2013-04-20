namespace ParallelAgg {

    using System;
    using ParallelAgg.Metadata;

    public class EntityEventArgs : EventArgs {

        private readonly Entity _entity;

        public EntityEventArgs(Entity entity) {
            _entity = entity;
        }

        public Entity Entity {
            get { return _entity; }
        }
    }

    public class Entity {

        private readonly int[] _keys;
        private readonly decimal[] _values;
        private readonly EntityEventArgs _eventArgs;

        public Entity(int[] keys, int propertyCount) {
            _keys = keys;
            _values = new decimal[propertyCount];
            _eventArgs = new EntityEventArgs(this);
        }

        public EntityEventArgs EventArgs {
            get { return _eventArgs; }
        }

        public int GetKey(int index) {
            return _keys[index];
        }

        public decimal Get(PropertyMetadata property) {
            return _values[property.Index];
        }

        public void Set(PropertyMetadata property, decimal value) {
            var oldValue = _values[property.Index];
            if (oldValue == value) return;

            var propertyChanging = PropertyChanging;
            if (propertyChanging != null)
                propertyChanging(this, property.EventArgs);

            _values[property.Index] = value;

            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, property.EventArgs);
        }

        public event EventHandler<PropertyEventArgs> PropertyChanged;

        public event EventHandler<PropertyEventArgs> PropertyChanging;
    }
}