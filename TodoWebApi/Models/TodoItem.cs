namespace TodoWebApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public int UserId { get; set; }  // Foreign key to User
        public User User { get; set; }    // Navigation property to User

    }
}
