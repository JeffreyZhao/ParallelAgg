namespace Shared.Models {

    public class Property {

        private readonly int _index;

        public Property(int index) {
            _index = index;
        }

        public int Index {
            get { return _index; }
        }
    }
}