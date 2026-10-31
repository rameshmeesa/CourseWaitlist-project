using CourseWaitlist.Api.Models;

namespace CourseWaitlist.Api.Services;

/// <summary>Atomic enrollment/drop transitions for a single in-memory course offering.</summary>
public sealed class EnrollmentService(InMemoryCourseStore store) : IEnrollmentService
{
    public OperationResult Enroll(string? learnerId)
    {
        lock (store.SyncRoot)
        {
            if (!InMemoryCourseStore.IsValidLearnerId(learnerId))
                return Failure("INVALID_LEARNER", "Learner ID is required and may contain only letters, digits, '-' or '_'.", learnerId);

            var id = learnerId.Trim();
            if (store.Enrolled.Contains(id))
                return Failure("ALREADY_ENROLLED", "Learner is already enrolled.", id);
            if (store.Waitlist.Contains(id, StringComparer.OrdinalIgnoreCase))
                return Failure("ALREADY_WAITLISTED", "Learner is already on the waitlist.", id);
            if (!store.MeetsPrerequisite(id))
                return Failure("PREREQUISITE_NOT_MET", "Learner does not satisfy the course prerequisite.", id);

            if (store.Enrolled.Count < store.Capacity)
            {
                store.Enrolled.Add(id);
                return new(true, "ENROLLED", "Enrollment successful.", id);
            }

            store.Waitlist.Add(id);
            return new(true, "WAITLISTED", "Course is full; learner added to the waitlist.", id);
        }
    }

    public OperationResult Drop(string? learnerId)
    {
        lock (store.SyncRoot)
        {
            if (!InMemoryCourseStore.IsValidLearnerId(learnerId))
                return Failure("INVALID_LEARNER", "Learner ID is invalid.", learnerId);

            var id = learnerId.Trim();
            if (!store.Enrolled.Remove(id))
                return Failure("NOT_ENROLLED", "Learner is not currently enrolled; no state was changed.", id);

            string? promoted = null;
            // FIFO policy: discard any now-ineligible queue entries, then promote the first eligible one.
            while (store.Waitlist.Count > 0)
            {
                var candidate = store.Waitlist[0];
                store.Waitlist.RemoveAt(0);
                if (!store.MeetsPrerequisite(candidate))
                    continue;

                store.Enrolled.Add(candidate);
                promoted = candidate;
                break;
            }

            return new(true, "DROPPED",
                promoted is null
                    ? "Enrollment dropped; no eligible waitlisted learner was available."
                    : $"Enrollment dropped; {promoted} was promoted.",
                id, promoted);
        }
    }

    public CourseSnapshot GetSnapshot()
    {
        lock (store.SyncRoot)
        {
            return new CourseSnapshot(
                store.CourseId,
                store.Capacity,
                store.Enrolled.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray(),
                store.Waitlist.ToArray(),
                store.Capacity - store.Enrolled.Count);
        }
    }

    private static OperationResult Failure(string code, string message, string? learnerId) =>
        new(false, code, message, learnerId);
}
