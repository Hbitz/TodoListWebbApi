namespace TodoWebApi.DTOs
{
    public class CreateTodoDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;

        public int? CategoryId { get; set; } // optional
        public string? CategoryName { get; set; } // optional
    }
}
