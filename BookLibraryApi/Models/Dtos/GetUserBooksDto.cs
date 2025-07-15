namespace BookLibraryApi.Models.Dtos
{
    public class GetUserBooksDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public bool? isRead { get; set; }
        public bool? isFavorite { get; set; }
        public DateTime? Year { get; set; }
        public int Page { get; set; }
        public int PageSize {  get; set; }

    }
}
