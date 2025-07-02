namespace BookLibraryApi.Models.Dtos
{
    public class BookReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string? Description { get; set; }
        public DateTime Year { get; set; }
        public bool isRead { get; set; }
        public bool isFavorite { get; set; }
    }
}
