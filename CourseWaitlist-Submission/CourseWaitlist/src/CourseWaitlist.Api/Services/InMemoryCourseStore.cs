namespace CourseWaitlist.Api.Services;

/// <summary>State for one offering. Service operations must hold SyncRoot while accessing mutable state.</summary>
public sealed class InMemoryCourseStore
{
    internal object SyncRoot { get; } = new();
    internal HashSet<string> Enrolled { get; } = new(StringComparer.OrdinalIgnoreCase);
    internal List<string> Waitlist { get; } = new();
    internal HashSet<string> EligibleLearners { get; } = new(StringComparer.OrdinalIgnoreCase);

    public string CourseId { get; }
    public int Capacity { get; }

    public InMemoryCourseStore(int capacity = 2, string courseId = "COURSE-101")
    {
        if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be at least one.");
        if (string.IsNullOrWhiteSpace(courseId)) throw new ArgumentException("Course ID is required.", nameof(courseId));

        Capacity = capacity;
        CourseId = courseId;

        // Demo prerequisite directory. Unknown learner IDs are ineligible.
        foreach (var id in new[] { "L001", "L002", "L003", "L004", "L005", "L006", "L007", "L008", "L009", "L010" })
            EligibleLearners.Add(id);
    }

    internal static bool IsValidLearnerId(string? learnerId) =>
        !string.IsNullOrWhiteSpace(learnerId) &&
        learnerId.Length <= 30 &&
        learnerId.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_');

    internal bool MeetsPrerequisite(string learnerId) => EligibleLearners.Contains(learnerId);
}
