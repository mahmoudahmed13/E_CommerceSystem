namespace E_Commerce.Application.Common
{
    public sealed record Error(string Code, string Message, ErrorType ErrorType = ErrorType.Failure)
    {
        public static Error Failure(string code = "General.Failure", string message = "General.Failure has Occured")
            => new(code, message, ErrorType.Failure);

        public static Error Validation(string code = "General.Validation", string message = "General Validation Error has Occured")
           => new(code, message, ErrorType.Validation);

        public static Error NotFound(string code = "General.NotFound", string message = "Resource NotFound")
            => new(code, message, ErrorType.NotFound);

        public static Error Conflict(string code = "General.Conflict", string message = "General Conflict has Occured")
            => new(code, message, ErrorType.Conflict);

        public static Error Unauthorized(string code = "General.UnAuthorized", string message = "Acess Is Denied to bad UnAuthorized")
            => new(code, message, ErrorType.UnAuthorized);

        public static Error Forbidden(string code = "General.Forbidden", string message = "This Operation Forbidden")
            => new(code, message, ErrorType.Forbidden);
        public static Error InvalidCredentials(string code = "General.InvalidCredentials", string message = "This Operation InvalidCredentials")
            => new(code, message, ErrorType.InvalidCredentials);
    }
}
