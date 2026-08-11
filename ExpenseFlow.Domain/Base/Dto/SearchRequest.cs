namespace ExpenseFlow.Domain.Base.Dto
{
    public class SearchRequest : SortRequest
    {
        public string Query { get; set; } = "";

    }
}
