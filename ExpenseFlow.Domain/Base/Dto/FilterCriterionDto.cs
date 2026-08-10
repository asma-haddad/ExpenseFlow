using ExpenseFlow.Domain.Shared.Enum;

namespace ExpenseFlow.Domain.Base.Dto
{
    public class FilterCriterionDto : PaginationRequest
    {
        public string PropertyName { get; set; }
        public FilterOperator Operator { get; set; }
        public string Value { get; set; }
    }
}
