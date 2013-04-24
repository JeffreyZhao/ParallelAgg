namespace ParallelAgg.Aggregation {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ParallelAgg.Metadata;

    public abstract class AggregationRootBase : IAggregationRoot {

        private readonly EntitySet _set;
        private readonly AggregationConfig _config;

        private readonly EventHandler<PropertyEventArgs> _entityOnPropertyChangingHandler;
        private readonly EventHandler<PropertyEventArgs> _entityOnPropertyChangedHandler;

        private PropertyMetadata _changingProperty;
        private List<PropertyAggregatorUpdate> _updates;

        protected AggregationRootBase(EntitySet set, AggregationConfig config) {
            _set = set;
            _config = config;

            _entityOnPropertyChangingHandler = EntityOnPropertyChanging;
            _entityOnPropertyChangedHandler = EntityOnPropertyChanged;
        }

        public abstract IAggregationResult Result { get; }

        public abstract bool Running { get; }

        public void Start() {
            foreach (var entity in _set) {
                SetOnEntityAdded(_set, entity.EventArgs);
            }

            _set.EntityAdded += SetOnEntityAdded;
            _set.EntityRemoved += SetOnEntityRemoved;
        }

        private void SetOnEntityAdded(object sender, EntityEventArgs args) {
            var entity = args.Entity;

            entity.PropertyChanging += _entityOnPropertyChangingHandler;
            entity.PropertyChanged += _entityOnPropertyChangedHandler;

            AddEntity(entity, GetUpdates(entity, _config.Aggregators));
        }

        private void SetOnEntityRemoved(object sender, EntityEventArgs args) {
            var entity = args.Entity;

            entity.PropertyChanging += _entityOnPropertyChangingHandler;
            entity.PropertyChanged += _entityOnPropertyChangedHandler;

            RemoveEntity(entity, GetUpdates(entity, _config.Aggregators));
        }

        private List<PropertyAggregatorUpdate> GetUpdates(Entity entity, IEnumerable<PropertyAggregatorConfig> configs) {
            return configs.Select(c => new PropertyAggregatorUpdate(c, c.CaptureValues(entity))).ToList();
        }

        private void EntityOnPropertyChanging(object sender, PropertyEventArgs args) {
            Debug.Assert(_changingProperty == null);
            Debug.Assert(_updates == null);

            var entity = (Entity)sender;
            _changingProperty = args.Property;

            var aggregatorsToUpdate = _config.GetAggregatorsUpdateWith(args.Property);
            if (aggregatorsToUpdate != null)
            {
                _updates = GetUpdates(entity, aggregatorsToUpdate);
            }
        }

        private void EntityOnPropertyChanged(object sender, PropertyEventArgs args) {
            Debug.Assert(_changingProperty == args.Property);
            Debug.Assert(_updates != null);

            var entity = (Entity)sender;
            var newValue = entity.Get(_changingProperty);

            UpdateEntity(entity, _updates, _changingProperty, newValue);

            _changingProperty = null;
            _updates = null;
        }

        protected abstract void AddEntity(Entity entity, ICollection<PropertyAggregatorUpdate> updates);

        protected abstract void RemoveEntity(Entity entity, ICollection<PropertyAggregatorUpdate> updates);

        protected abstract void UpdateEntity(Entity entity, ICollection<PropertyAggregatorUpdate> updates, PropertyMetadata property, decimal newValue);
    }
}
