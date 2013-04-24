namespace ParallelAgg.Aggregation.Tests.Parallel {

    using ParallelAgg.Aggregation.Parallel;
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
