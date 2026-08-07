namespace ExpenseFlow.Domain.Model.Base;

public abstract class BaseModel
{
    public long Id { get; set; }
    public bool IsValid { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}