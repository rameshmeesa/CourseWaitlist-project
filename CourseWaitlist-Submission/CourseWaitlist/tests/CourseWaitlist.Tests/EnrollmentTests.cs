using CourseWaitlist.Api.Services;
using Xunit;

namespace CourseWaitlist.Tests;

public sealed class EnrollmentTests
{
    private static EnrollmentService CreateService() => new(new InMemoryCourseStore());

    [Fact]
    public void EnrollsWhenCapacityIsAvailable()
    {
        var service = CreateService();
        var result = service.Enroll("L001");
        Assert.True(result.Success);
        Assert.Equal("ENROLLED", result.Code);
        Assert.Contains("L001", service.GetSnapshot().EnrolledLearners);
        Assert.Equal(1, service.GetSnapshot().AvailableSeats);
    }

    [Fact]
    public void FullCoursePlacesEligibleLearnerOnWaitlist()
    {
        var service = CreateService();
        service.Enroll("L001");
        service.Enroll("L002");
        var result = service.Enroll("L003");
        Assert.True(result.Success);
        Assert.Equal("WAITLISTED", result.Code);
        Assert.Equal(new[] { "L003" }, service.GetSnapshot().Waitlist);
        Assert.Equal(0, service.GetSnapshot().AvailableSeats);
    }

    [Fact]
    public void DuplicateEnrollmentDoesNotChangeState()
    {
        var service = CreateService();
        service.Enroll("L001");
        var result = service.Enroll("l001");
        Assert.False(result.Success);
        Assert.Equal("ALREADY_ENROLLED", result.Code);
        Assert.Single(service.GetSnapshot().EnrolledLearners);
    }

    [Fact]
    public void DuplicateWaitlistRequestDoesNotAddSecondEntry()
    {
        var service = CreateService();
        service.Enroll("L001"); service.Enroll("L002"); service.Enroll("L003");
        var result = service.Enroll("l003");
        Assert.Equal("ALREADY_WAITLISTED", result.Code);
        Assert.Equal(new[] { "L003" }, service.GetSnapshot().Waitlist);
    }

    [Fact]
    public void RejectsLearnerWhoDoesNotMeetPrerequisite()
    {
        var service = CreateService();
        var result = service.Enroll("L999");
        Assert.False(result.Success);
        Assert.Equal("PREREQUISITE_NOT_MET", result.Code);
        Assert.Empty(service.GetSnapshot().EnrolledLearners);
        Assert.Empty(service.GetSnapshot().Waitlist);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("bad id")]
    [InlineData("learner/1")]
    public void RejectsInvalidLearnerIds(string? learnerId)
    {
        var service = CreateService();
        var result = service.Enroll(learnerId);
        Assert.Equal("INVALID_LEARNER", result.Code);
        Assert.Empty(service.GetSnapshot().EnrolledLearners);
    }
}
