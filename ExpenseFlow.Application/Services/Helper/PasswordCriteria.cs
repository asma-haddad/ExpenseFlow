namespace ExpenseFlow.Application.Services.Helper
{
    public class PasswordCriteria
    {
        public int MinRequiredLength { get; set; } = 8;
        public int MaxRequiredLength { get; set; } = 16;
        public int RequiredUniqueChars { get; set; } = 1;
        public bool RequireDigit { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireNonAlphanumeric { get; set; } = true;
    }
}
