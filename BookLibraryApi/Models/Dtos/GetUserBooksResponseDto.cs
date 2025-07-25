namespace BookLibraryApi.Models.Dtos
{
    public class GetUserBooksResponseDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<BookReadDto> Data { get; set; }
    }
}
