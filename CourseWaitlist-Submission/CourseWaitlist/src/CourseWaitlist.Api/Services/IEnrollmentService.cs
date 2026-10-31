using CourseWaitlist.Api.Models;

namespace CourseWaitlist.Api.Services;

public interface IEnrollmentService
{
    OperationResult Enroll(string? learnerId);
    OperationResult Drop(string? learnerId);
    CourseSnapshot GetSnapshot();
}
