namespace CourseWaitlist.Api.Models;

public sealed record OperationResult(
    bool Success,
    string Code,
    string Message,
    string? LearnerId = null,
    string? PromotedLearnerId = null);
