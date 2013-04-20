namespace ParallelAgg.Metadata {

    using System;

    public class PropertyEventArgs : EventArgs {

        private readonly PropertyMetadata _property;

        public PropertyEventArgs(PropertyMetadata property) {
            _property = property;
        }

        public PropertyMetadata Property {
            get { return _property; }
        }
    }

    public class PropertyMetadata {

        private readonly int _index;
        private readonly PropertyEventArgs _eventArgs;

        public PropertyMetadata(int index) {
            _index = index;
            _eventArgs = new PropertyEventArgs(this);
        }

        public int Index {
            get { return _index; }
        }

        public PropertyEventArgs EventArgs {
            get { return _eventArgs; }
        }
    }
}