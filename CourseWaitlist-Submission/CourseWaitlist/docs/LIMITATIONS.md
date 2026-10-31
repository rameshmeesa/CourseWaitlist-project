# Limitations and Follow-up Risks

- State is in memory and is lost when the process restarts.
- The lock coordinates only requests handled by this process; it does not coordinate multiple replicas.
- Course ID and capacity are fixed demo values, not administration features.
- Prerequisite eligibility is a demo in-memory set, not an external learner/course record.
- Learner IDs are validated syntactically but there is no authentication or authorization; identity integration is out of scope.
- A skipped ineligible learner is removed from the queue and must reapply after meeting the prerequisite.
- No audit/event history, notification, metrics, or operational health probes beyond the root endpoint are included.
- API errors use HTTP 400 for all unsuccessful domain outcomes; a production API may distinguish 404, 409, and validation errors.
- The included concurrency test covers concurrent enrollment in one process; it does not test multi-instance behavior or database transaction isolation.
