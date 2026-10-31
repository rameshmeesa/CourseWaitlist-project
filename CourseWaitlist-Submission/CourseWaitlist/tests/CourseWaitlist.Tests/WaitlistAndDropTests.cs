using CourseWaitlist.Api.Services;
using Xunit;

namespace CourseWaitlist.Tests;

public sealed class WaitlistAndDropTests
{
    [Fact]
    public void DropPromotesEarliestEligibleLearnerInFifoOrder()
    {
        var service = new EnrollmentService(new InMemoryCourseStore());
        service.Enroll("L001"); service.Enroll("L002");
        service.Enroll("L003"); service.Enroll("L004");

        var result = service.Drop("L001");
        var snapshot = service.GetSnapshot();

        Assert.True(result.Success);
        Assert.Equal("L003", result.PromotedLearnerId);
        Assert.Contains("L003", snapshot.EnrolledLearners);
        Assert.Equal(new[] { "L004" }, snapshot.Waitlist);
        Assert.Equal(0, snapshot.AvailableSeats);
    }

    [Fact]
    public void IneligibleWaitlistHeadIsRemovedAndNextEligibleLearnerPromoted()
    {
        var store = new InMemoryCourseStore();
        var service = new EnrollmentService(store);
        service.Enroll("L001"); service.Enroll("L002");
        service.Enroll("L003"); service.Enroll("L004");
        // Simulate prerequisite status changing after L003 joined the queue.
        lock (store.SyncRoot) store.EligibleLearners.Remove("L003");

        var result = service.Drop("L001");
        var snapshot = service.GetSnapshot();

        Assert.Equal("L004", result.PromotedLearnerId);
        Assert.DoesNotContain("L003", snapshot.Waitlist);
        Assert.DoesNotContain("L003", snapshot.EnrolledLearners);
        Assert.Contains("L004", snapshot.EnrolledLearners);
    }

    [Fact]
    public void DropWithoutEligibleWaitlistLeavesOneSeatAvailable()
    {
        var service = new EnrollmentService(new InMemoryCourseStore());
        service.Enroll("L001");
        var result = service.Drop("L001");
        var snapshot = service.GetSnapshot();
        Assert.Equal("DROPPED", result.Code);
        Assert.Null(result.PromotedLearnerId);
        Assert.Equal(2, snapshot.AvailableSeats);
    }

    [Fact]
    public void InvalidAndRepeatedDropsDoNotCorruptState()
    {
        var service = new EnrollmentService(new InMemoryCourseStore());
        service.Enroll("L001"); service.Enroll("L002"); service.Enroll("L003");
        var invalid = service.Drop("bad id");
        var first = service.Drop("L001");
        var afterFirst = service.GetSnapshot();
        var repeated = service.Drop("L001");
        var afterRepeated = service.GetSnapshot();

        Assert.Equal("INVALID_LEARNER", invalid.Code);
        Assert.Equal("DROPPED", first.Code);
        Assert.Equal("NOT_ENROLLED", repeated.Code);
        Assert.Equal(afterFirst.EnrolledLearners, afterRepeated.EnrolledLearners);
        Assert.Equal(afterFirst.Waitlist, afterRepeated.Waitlist);
        Assert.Equal(afterFirst.AvailableSeats, afterRepeated.AvailableSeats);
    }

    [Fact]
    public async Task ConcurrentEnrollmentsNeverExceedCapacityOrDuplicateLearners()
    {
        var service = new EnrollmentService(new InMemoryCourseStore());
        var tasks = Enumerable.Range(1, 10)
            .Select(i => Task.Run(() => service.Enroll($"L{i:000}")));
        await Task.WhenAll(tasks);

        var snapshot = service.GetSnapshot();
        Assert.Equal(2, snapshot.EnrolledLearners.Count);
        Assert.Equal(8, snapshot.Waitlist.Count);
        Assert.Equal(snapshot.Capacity, snapshot.EnrolledLearners.Count + snapshot.AvailableSeats);
        Assert.Equal(10, snapshot.EnrolledLearners.Concat(snapshot.Waitlist).Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }
}
