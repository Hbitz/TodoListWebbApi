using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TodoWebApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Category> Categories { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
