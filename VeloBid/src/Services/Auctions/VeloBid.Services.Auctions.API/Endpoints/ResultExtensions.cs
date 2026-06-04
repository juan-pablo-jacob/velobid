using VeloBid.Services.Auctions.Application.Common.Results;

namespace VeloBid.Services.Auctions.API.Endpoints;

internal static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return result.Error.Code switch
        {
            "not_found" => Results.NotFound(new ProblemDetailsResponse(
                result.Error.Code,
                result.Error.Message)),

            "validation_error" => Results.BadRequest(new ProblemDetailsResponse(
                result.Error.Code,
                result.Error.Message)),

            "conflict" => Results.Conflict(new ProblemDetailsResponse(
                result.Error.Code,
                result.Error.Message)),

            _ => Results.Problem(
                title: "Unexpected error",
                detail: result.Error.Message,
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private sealed record ProblemDetailsResponse(
        string Code,
        string Message);
}