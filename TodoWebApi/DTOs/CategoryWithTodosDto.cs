namespace TodoWebApi.DTOs
{
    public class CategoryWithTodosDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TodoItemDto> Todos { get; set; }
    }
}
