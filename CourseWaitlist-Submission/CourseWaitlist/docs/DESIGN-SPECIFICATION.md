# Design Specification — CourseWaitlist

## 1. Scope and service boundary

This implementation supports one offering only: `COURSE-101`. The application is an ASP.NET Core minimal API. HTTP handlers delegate to `IEnrollmentService`, which owns the enrollment/drop rules. `InMemoryCourseStore` holds the offering state and a small prerequisite directory. The service can be tested directly without starting the web host.

Out of scope: frontend, catalog, multiple terms/courses, payments, identity integration, notifications, database persistence, deployment, and configurable policy engines.

## 2. Data and API design

- `InMemoryCourseStore`: course ID, fixed capacity, case-insensitive enrolled set, ordered waitlist list, and eligible-learner set.
- `CourseSnapshot`: immutable response view containing course ID, capacity, enrolled IDs, queue, and available seats.
- `OperationResult`: success flag, stable result code, human-readable message, learner ID, and optional promoted learner ID.
- `IEnrollmentService`: `Enroll`, `Drop`, and `GetSnapshot`.

Endpoints:

| Method | Route | Behavior |
|---|---|---|
| GET | `/api/course` | Return offering state and available seats |
| GET | `/api/waitlist` | Return FIFO queue |
| POST | `/api/enrollments/{learnerId}` | Enroll or enqueue learner |
| DELETE | `/api/enrollments/{learnerId}` | Drop learner and attempt promotion |

Invalid operations return HTTP 400 with a structured `OperationResult`. Successful enrollment, waitlisting, and drops return HTTP 200. A successful drop with no promotion is still a successful drop.

## 3. Prerequisite and waitlist assumptions

The prerequisite is a boolean membership check in `EligibleLearners`. The sample store marks `L001` through `L010` eligible; other syntactically valid IDs (for example `L999`) fail. This demo directory represents the prerequisite source that a real starter project might provide.

Only eligible learners may enroll or enter the waitlist. A learner may appear in only one state (enrolled or waiting). IDs are case-insensitive. If a learner already in the waitlist becomes ineligible before promotion, the promotion scan removes that entry and continues to the next queue entry. The skipped learner must submit a new enrollment request after satisfying the prerequisite. This skip-and-remove policy is applied consistently in the drop/promotion path.

## 4. Invariants

1. `0 <= Enrolled.Count <= Capacity`.
2. A learner cannot be both enrolled and waitlisted.
3. Each learner appears at most once across both collections.
4. Waitlist order is append order (FIFO); promotion selects the earliest currently eligible learner.
5. `AvailableSeats = Capacity - Enrolled.Count`.
6. Invalid requests, prerequisite failures, duplicate enrollments, duplicate waitlist requests, and invalid/repeated drops do not mutate state.
7. Dropping an enrolled learner releases exactly one seat; promotion fills at most that seat.

## 5. Enrollment algorithm

Under the store lock: validate learner ID; reject an already-enrolled or already-waitlisted learner; validate prerequisite; if `Enrolled.Count < Capacity`, add to enrolled; otherwise append to the end of the waitlist. All decisions and mutations occur in one critical section.

## 6. Drop and promotion algorithm

Under the same lock: validate ID; remove it from enrolled or return `NOT_ENROLLED` without mutation. Then inspect the queue from index zero. Remove each candidate from the queue; discard it if no longer eligible; otherwise move the first eligible candidate into enrolled and stop. The result reports the promoted learner, if any.

## 7. Concurrency and persistence

A single `lock` protects every read and mutation of the in-memory state. This serializes competing requests within one process and prevents over-enrollment, duplicate cross-state membership, and interleaved promotion. A focused concurrent-enrollment test launches ten distinct eligible learners against capacity two and asserts two enrolled, eight waiting, and no duplicates.

The lock is not distributed: multiple application instances would each have independent state. For a persistent implementation, use a database transaction and an appropriate row/aggregate lock or optimistic concurrency token; add a unique constraint for `(offering, learner)` and make the drop-plus-promotion transition atomic.

## 8. Tradeoffs

The set/list combination gives straightforward membership checks and FIFO ordering. A coarse lock is intentionally favored over more complex synchronization for one offering. The fixed offering and demo eligibility directory keep the slice bounded and replaceable. The API exposes result codes for deterministic clients/tests without introducing a larger error framework.
