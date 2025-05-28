namespace TodoWebApi.DTOs
{
    public class UpdateTodoDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
