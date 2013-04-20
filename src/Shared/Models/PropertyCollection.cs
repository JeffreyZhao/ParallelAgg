namespace Shared.Models {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class PropertyCollection : ReadOnlyCollection<Property> {

        private static List<Property> CreateProperties(int count) {
            return Enumerable.Range(0, count).Select(i => new Property(i)).ToList();
        }

        public PropertyCollection(int count)
            : base(CreateProperties(count)) { }
    }
}