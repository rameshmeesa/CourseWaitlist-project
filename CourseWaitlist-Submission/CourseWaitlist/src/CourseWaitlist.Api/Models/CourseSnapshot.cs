namespace CourseWaitlist.Api.Models;

public sealed record CourseSnapshot(
    string CourseId,
    int Capacity,
    IReadOnlyList<string> EnrolledLearners,
    IReadOnlyList<string> Waitlist,
    int AvailableSeats);
