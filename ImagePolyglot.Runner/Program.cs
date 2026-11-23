using Microsoft.AspNetCore.Builder;
using ImagePolyglot.Runner;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/run", (RunnerRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Code))
    {
        return Results.BadRequest(new RunnerResponse
        {
            Success = false,
            Output = "No code received."
        });
    }

    var runner = new ChallengeRunner();
    string result = runner.Run(request.Code);

    return Results.Ok(new RunnerResponse
    {
        Success = result.Contains("CHALLENGE_COMPLETED"),
        Output = result
    });
});

app.Run("http://localhost:5100");