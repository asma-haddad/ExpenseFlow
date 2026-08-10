namespace ExpenseFlow.Domain.Base.Dto
{
    public class FilterRequest : PaginationRequest
    {
        public bool IsAnd { get; set; } = true;
        public List<FilterCriterionDto> Filters { get; set; } = new();
    }
}
