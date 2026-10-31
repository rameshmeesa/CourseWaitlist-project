using CourseWaitlist.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<InMemoryCourseStore>();
builder.Services.AddSingleton<IEnrollmentService, EnrollmentService>();

var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { service = "CourseWaitlist", status = "running" }));
app.MapGet("/api/course", (IEnrollmentService service) => Results.Ok(service.GetSnapshot()));
app.MapGet("/api/waitlist", (IEnrollmentService service) => Results.Ok(service.GetSnapshot().Waitlist));
app.MapPost("/api/enrollments/{learnerId}", (string learnerId, IEnrollmentService service) =>
{
    var result = service.Enroll(learnerId);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});
app.MapDelete("/api/enrollments/{learnerId}", (string learnerId, IEnrollmentService service) =>
{
    var result = service.Drop(learnerId);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});
app.Run();

// Allows WebApplicationFactory-based tests to locate the entry point if added later.
public partial class Program { }
