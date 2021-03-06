﻿namespace ParallelAgg.Metadata {

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

        internal PropertyMetadata(int index) {
            _index = index;
            _eventArgs = new PropertyEventArgs(this);
        }

        public int Index {
            get { return _index; }
        }

        public PropertyEventArgs EventArgs {
            get { return _eventArgs; }
        }

        public string Name {
            get { return Char.ToString((char)('A' + _index)); }
        }

        public override string ToString() {
            return Name;
        }
    }
}