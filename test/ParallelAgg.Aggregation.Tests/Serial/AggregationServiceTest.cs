namespace ParallelAgg.Aggregation.Tests.Serial {

    using ParallelAgg.Aggregation.Serial;
    using Xunit;

    public class AggregationServiceTest : AggregationServiceTestBase<AggregationService> {

        [Fact]
        public void Verify() {
            VerifyInternal();
        }

        [Fact]
        public void Remove() {
            RemoveInternal();
        }

        [Fact]
        public void Update() {
            UpdateInternal();
        }
    }
}
