namespace TodoWebApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TodoItem> TodoItems { get; set; }
    }
}
