using System.ComponentModel.DataAnnotations;

namespace ToDo_Sharp.Database.Models
{
    public class User
    {
        [Key]
        public string Login { get; set; }

        public string Password { get; set; }

        public List<TaskModel> TaskModels { get; set; } = new();
    }
}
