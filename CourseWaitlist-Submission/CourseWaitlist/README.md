# CourseWaitlist

A bounded single-course enrollment API implemented with ASP.NET Core 8, in-memory storage, and xUnit. It covers enrollment, dropping, prerequisite checks, duplicate prevention, and FIFO waitlist promotion.

## Requirements
- .NET 8 SDK
- NuGet access on the first restore/test run

## Restore, test, build, run
Run these commands from the repository root:

```bash
dotnet restore CourseWaitlist.sln
dotnet test CourseWaitlist.sln --logger "console;verbosity=normal"
dotnet build CourseWaitlist.sln --configuration Release
dotnet run --project src/CourseWaitlist.Api
```

The API prints its listening URL (typically `http://localhost:5xxx` or the configured URL). The root endpoint returns service health.

## HTTP API
Assuming the app is listening at `http://localhost:5080`:

```bash
curl http://localhost:5080/api/course
curl -X POST http://localhost:5080/api/enrollments/L001
curl -X POST http://localhost:5080/api/enrollments/L002
curl -X POST http://localhost:5080/api/enrollments/L003
curl -X DELETE http://localhost:5080/api/enrollments/L001
curl http://localhost:5080/api/waitlist
```

- `GET /api/course` — course ID, capacity, enrolled learners, FIFO waitlist, and available seats.
- `GET /api/waitlist` — current waitlist in FIFO order.
- `POST /api/enrollments/{learnerId}` — enroll when a seat is available or join the waitlist.
- `DELETE /api/enrollments/{learnerId}` — drop an enrollment and promote the first eligible waiter, if any.

Demo course: `COURSE-101`, capacity 2. Demo learners `L001`–`L010` meet the prerequisite. `L999` is syntactically valid but does not meet the prerequisite. Learner IDs are case-insensitive.

## Assessment documents
- `docs/DESIGN-SPECIFICATION.md`
- `docs/AI-USAGE-RECORD.md`
- `docs/LIMITATIONS.md`
- `docs/RUBRIC-EVIDENCE.md`

## Important
This is an in-memory prototype. State resets when the process stops. No database, authentication, frontend, or production deployment is included, in line with the assessment scope. Test execution must be performed in an environment with the .NET 8 SDK; see the AI-use record for verification status.
