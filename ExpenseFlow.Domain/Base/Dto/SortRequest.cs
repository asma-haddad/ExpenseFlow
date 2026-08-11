namespace ExpenseFlow.Domain.Base.Dto
{
    public class SortRequest : FilterRequest
    {

        public string SortProperty { get; set; }
        public bool IsAsc { get; set; }
    }
}
