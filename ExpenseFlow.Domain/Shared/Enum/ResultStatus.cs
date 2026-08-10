namespace ExpenseFlow.Domain.Shared.Enum
{
    public enum ResultStatus
    {
        Failed,
        IsExist,
        Success,
        NotFound,
        BadRequest,
        UnOthorized,
        ValidationError,
        UnAuthenticated,
        InternalServerError,
        Conflict,
    }
}
