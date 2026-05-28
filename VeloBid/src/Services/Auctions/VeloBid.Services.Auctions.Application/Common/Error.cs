namespace VeloBid.Services.Auctions.Application.Common;

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string message)
    {
        return new Error("not_found", message);
    }

    public static Error Validation(string message)
    {
        return new Error("validation_error", message);
    }

    public static Error Conflict(string message)
    {
        return new Error("conflict", message);
    }
}