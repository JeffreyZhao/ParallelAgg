namespace ParallelAgg.Aggregation {

    public class ValueTuple {

        private readonly decimal _value0;
        private readonly decimal _value1;

        public ValueTuple(decimal value0, decimal value1 = default(decimal)) {
            _value0 = value0;
            _value1 = value1;
        }

        public decimal Value0 {
            get { return _value0; }
        }

        public decimal Value1 {
            get { return _value1; }
        }
    }
}