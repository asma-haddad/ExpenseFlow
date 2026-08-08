namespace ExpenseFlow.Application.Dto
{

    public class GetAllDataResponse<T>
    {
        public GetAllDataResponse()
        {
            Data = [];
        }

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }

        public int TotalDataCount { get; set; }

        public List<T> Data { get; set; }
    }
}