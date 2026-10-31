# AI Prompt and Use Record

## Engineering activity and prompt

AI was used as an engineering assistant to plan the service boundary, reason through FIFO promotion and edge cases, and draft the implementation/test checklist. The prompt used for this work was:

> Implement a bounded one-course registration slice in C# ASP.NET Core with in-memory storage and xUnit. Support enrollment, dropping, prerequisite validation, duplicate prevention, FIFO waitlist promotion, and concurrent requests. State the policy for a waitlisted learner who becomes ineligible. Keep the design proportionate and provide focused tests plus design, setup, limitations, and AI-use documentation. Do not add a frontend or database.

## Relevant AI response summary

The proposed design used a service interface, a singleton in-memory state object, a `HashSet` for enrolled learners, an ordered `List` for the FIFO queue, and one lock around each state transition. The promotion algorithm scans the queue in order, discards candidates who no longer meet the prerequisite, and promotes the first eligible candidate. Suggested tests covered capacity, duplicate requests, prerequisites, FIFO, invalid/repeated drops, and concurrency.

## Review and disposition

| AI-assisted suggestion | Decision | Review / verification planned or encoded |
|---|---|---|
| Separate `IEnrollmentService` from HTTP endpoints | Accepted | Service tests invoke the implementation directly, independent of ASP.NET hosting. |
| `HashSet` + ordered `List` | Accepted | Case-insensitive collections and tests check duplicates and queue ordering. |
| One lock around state reads and mutations | Accepted for this single-process prototype | Concurrent enrollment test asserts capacity and uniqueness. Not a distributed lock. |
| Skip and remove a waiter who is ineligible at promotion time | Accepted | Explicitly documented and covered by a test that changes eligibility after queue entry. |
| Demo prerequisite directory | Edited to a small explicit sample set | `L001`–`L010` are eligible; `L999` demonstrates prerequisite failure. Replace with the supplied learner/prerequisite source in a real integration. |
| In-memory persistence | Accepted because it is expressly permitted by the fallback scope | Restart-loss and multi-instance limitations are documented. |

## Independent verification evidence and status

The source includes focused xUnit tests for:
- available capacity and full-capacity waitlisting;
- duplicate enrollment and duplicate waitlist requests;
- prerequisite failure and invalid learner IDs;
- FIFO promotion and ineligible-head skipping;
- successful, invalid, and repeated drops;
- concurrent enrollment capacity and uniqueness.

Run these commands on a machine with .NET 8 SDK and NuGet access:

```bash
dotnet --version
dotnet restore CourseWaitlist.sln
dotnet test CourseWaitlist.sln --logger "console;verbosity=normal"
dotnet build CourseWaitlist.sln --configuration Release
```

**Execution status:** These commands have not been executed in the file-generation environment because the .NET SDK is not installed there. Therefore no passing-test claim is made. The test assertions are provided as verification evidence to be executed by the reviewer/candidate. Record the actual SDK version, test count, and command output before submission. AI-generated code should be reviewed and run locally before relying on it.
