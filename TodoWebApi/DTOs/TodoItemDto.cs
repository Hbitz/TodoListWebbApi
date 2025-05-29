namespace TodoWebApi.DTOs
{
    public class TodoItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public int? CategoryId { get; set; } // optional
        public string? CategoryName { get; set; } // optional
    }
}
