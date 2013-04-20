namespace ParallelAgg {

    using System;
    using System.Collections.Generic;

    public class EntitySet {

        private readonly HashSet<Entity> _set = new HashSet<Entity>();

        public void Add(Entity entity) {
            if (!_set.Add(entity)) return;

            var entityAdded = EntityAdded;
            if (entityAdded == null) return;

            entityAdded(this, entity.EventArgs);
        }

        public void Remove(Entity entity) {
            if (!_set.Remove(entity)) return;

            var entityRemoved = EntityRemoved;
            if (entityRemoved == null) return;

            entityRemoved(this, entity.EventArgs);
        }

        public int Count {
            get { return _set.Count; }
        }

        public event EventHandler<EntityEventArgs> EntityAdded;

        public event EventHandler<EntityEventArgs> EntityRemoved;
    }
}