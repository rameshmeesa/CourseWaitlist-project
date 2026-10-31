. Recommended implementation
I recommend using C# / ASP.NET Core Web API with an in-memory repository and xUnit tests, if the supplied starter does not mandate another technology.

This also gives you a good opportunity to demonstrate your .NET backend engineering skills.
Core functionality
CourseWaitlist workflow
1. Learner requests enrollment
   
   Validate learner ID, course capacity, prerequisite eligibility, and duplicate enrollment.
2. Check available seats
   
   If seats are available and prerequisites are satisfied, enroll the learner.
3. Join waitlist
   
   If the course is full, add an eligible learner to the FIFO waitlist exactly once.
4. Learner drops the course
   
   Remove the enrolled learner and release one seat.
5. Promote the next eligible learner
   
   Check the queue in FIFO order, skip ineligible learners, and promote the earliest eligible learner.

2. Business rules and assumptions
I recommend documenting these rules:
Scenario	Expected behavior
Available capacity	Enroll eligible learner
Full capacity	Add eligible learner to waitlist
Duplicate enrollment	Return duplicate result
Missing prerequisite	Reject enrollment
Duplicate waitlist request	Do not add another entry
Course drop	Release seat
Ineligible learner at queue front	Skip and continue
Repeated drop	Return clear failure
Invalid learner ID	Reject request

Waitlist policy: FIFO with eligibility skipping. An ineligible learner is removed from the active queue and skipped during promotion. They must satisfy the prerequisite before submitting a new enrollment request.
3. Proposed project structure
CourseWaitlist/
│
├── src/
│   └── CourseWaitlist/
│       ├── Models/
│       │   ├── CourseOffering.cs
│       │   ├── Learner.cs
│       │   └── Enrollment.cs
│       │
│       ├── Services/
│       │   ├── IEnrollmentService.cs
│       │   └── EnrollmentService.cs
│       │
│       ├── Repositories/
│       │   └── InMemoryEnrollmentRepository.cs
│       │
│       └── Program.cs
│
├── tests/
│   └── CourseWaitlist.Tests/
│       ├── EnrollmentTests.cs
│       ├── WaitlistTests.cs
│       └── DropTests.cs
│
├── docs/
│   ├── DESIGN-SPECIFICATION.md
│   ├── AI-USAGE-RECORD.md
│   └── LIMITATIONS.md
│
└── README.md


4. Automated test coverage
I will include focused tests for:
1. Successful enrollment when seats are available.
2. Waitlist entry when capacity is full.
3. Duplicate enrollment prevention.
4. Duplicate waitlist prevention.
5. FIFO promotion after a drop.
6. Prerequisite failure.
7. Skipping ineligible waitlisted learners.
8. Invalid learner requests.
9. Repeated drop prevention.
10. Capacity and enrollment consistency.
11. Concurrent enrollment requests, if appropriate for the implementation.
5. Required submission documents
I will prepare these alongside the source code:
Deliverable	Contents
Runnable source code	Complete implementation
Automated tests	xUnit test project
Design specification	Architecture, rules, invariants and concurrency
AI usage record	Actual prompts, AI suggestions, decisions and verification
README	Setup and execution instructions
Limitations	Known risks and future improvements
