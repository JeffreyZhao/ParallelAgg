namespace Shared.Models {

    public class Entity {

        private readonly decimal[] _values;

        private Entity(PropertyCollection properties) {
            _values = new decimal[properties.Count];
        }

        public decimal Get(Property property) {
            return _values[property.Index];
        }

        public void Set(Property property, decimal value) {
            _values[property.Index] = value;
        }
    }
}