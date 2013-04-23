namespace ParallelAgg.Serial {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ParallelAgg.Aggregation;
    using ParallelAgg.Metadata;

    public class AggregationRoot {

        private readonly EntitySet _set;
        private readonly EntityMetadata _metadata;
        private readonly AggregationConfig _config;

        private readonly EventHandler<PropertyEventArgs> _entityOnPropertyChangingHandler;
        private readonly EventHandler<PropertyEventArgs> _entityOnPropertyChangedHandler;

        private PropertyMetadata _changingProperty;
        private List<PropertyAggregatorUpdate> _updates;

        public AggregationRoot(EntitySet set, EntityMetadata metadata, AggregationConfig config) {
            _set = set;
            _metadata = metadata;
            _config = config;

            _entityOnPropertyChangingHandler = EntityOnPropertyChanging;
            _entityOnPropertyChangedHandler = EntityOnPropertyChanged;
        }

        public AggregationResult Result { get; private set; }

        public void Start() {
            Result = new AggregationResult(_metadata, _config, 0);

            foreach (var entity in _set) {
                AddEntity(entity);
            }

            _set.EntityAdded += (_, args) => AddEntity(args.Entity);
            _set.EntityRemoved += (_, args) => RemoveEntity(args.Entity);
        }

        private List<PropertyAggregatorUpdate> GetUpdates(Entity entity, IEnumerable<PropertyAggregatorConfig> configs) {
            return configs.Select(c => new PropertyAggregatorUpdate(c, c.CaptureValues(entity))).ToList();
        } 

        private void AddEntity(Entity entity) {
            entity.PropertyChanging += _entityOnPropertyChangingHandler;
            entity.PropertyChanged += _entityOnPropertyChangedHandler;

            Result.Add(entity, GetUpdates(entity, _config.Aggregators));
        }

        private void RemoveEntity(Entity entity) {
            entity.PropertyChanging -= _entityOnPropertyChangingHandler;
            entity.PropertyChanged -= _entityOnPropertyChangedHandler;

            Result.Remove(entity, GetUpdates(entity, _config.Aggregators));
        }

        private void EntityOnPropertyChanging(object sender, PropertyEventArgs args) {
            Debug.Assert(_changingProperty == null);
            Debug.Assert(_updates == null);

            var entity = (Entity)sender;
            _changingProperty = args.Property;
            _updates = GetUpdates(entity, _config.GetAggregatorsUpdateWith(args.Property));
        }

        private void EntityOnPropertyChanged(object sender, PropertyEventArgs args) {
            Debug.Assert(_changingProperty == args.Property);
            Debug.Assert(_updates != null);

            var entity = (Entity)sender;
            var newValue = entity.Get(_changingProperty);

            Result.Update(entity, _updates, _changingProperty, newValue);

            _changingProperty = null;
            _updates = null;
        }
    }
}