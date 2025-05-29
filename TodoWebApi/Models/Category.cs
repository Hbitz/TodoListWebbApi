namespace TodoWebApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int UserId { get; set; } // FK
        public User User { get; set; }

        // Best practice - It's more common to use interface type for properties in domain models
        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}
