# AI-Assisted Assessment — Rubric Evidence Guide

The supplied `Course-Registration-Rubric-Score.md` was not available when this project was generated, so this is an evidence map rather than a claimed score.

| Assessment area | Evidence in this submission | Reviewer check |
|---|---|---|
| Understandable task decomposition | `DESIGN-SPECIFICATION.md`; service boundary and algorithms | Confirm scope is limited to one offering. |
| Correct business behavior | `EnrollmentService.cs` | Review invariants, duplicate checks, and promotion logic. |
| Focused automated tests | `EnrollmentTests.cs`, `WaitlistAndDropTests.cs` | Run `dotnet test`; inspect all results. |
| Concurrency reasoning | lock in `EnrollmentService`; concurrent test | Verify capacity and uniqueness assertions; note single-process limitation. |
| Responsible AI use | `AI-USAGE-RECORD.md` | Confirm prompt, response summary, decisions, and honest execution status. |
| Independent verification | commands in README and AI record | Execute locally and attach actual console output. |
| Proportionate design and documentation | README and docs folder | Check setup, assumptions, tradeoffs, and risks. |

Add the official rubric and map its exact criteria to this evidence before final submission. Do not assign a numeric score without applying the actual scorecard.
