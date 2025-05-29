namespace TodoWebApi.Models
{
    public class User
    {
        public int Id {  get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] passwordSalt { get; set; }

        public ICollection<TodoItem> TodoItems { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
