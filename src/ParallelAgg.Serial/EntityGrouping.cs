using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelAgg.Serial
{
    public class EntityGrouping {

        private readonly int _keyIndex;
        private readonly EntitySet _set;

        public EntityGrouping(int keyIndex, EntitySet set)
        {
            _keyIndex = keyIndex;
            _set = set;
        }

        protected void AddEntity(int key, Entity entity)
        {

        }

        protected void RemoveEntity(int key, Entity entity)
        {

        }
    }
}
