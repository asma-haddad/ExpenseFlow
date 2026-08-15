using ExpenseFlow.Domain.Model.Base;

namespace ExpenseFlow.Domain.Model.User
{
    public class RefreshTokenModel : BaseModel
    {
        public string TokenHash { get; set; } = default!;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByTokenHash { get; set; }
        public string? CreatedByIp { get; set; }
        public string? UserAgent { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
        public bool IsActive => RevokedAtUtc == null && !IsExpired;

        public Guid UserId { get; set; }
        public UserModel User { get; set; }
    }
}
