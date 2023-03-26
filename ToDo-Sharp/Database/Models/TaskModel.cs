using System.ComponentModel.DataAnnotations;

namespace ToDo_Sharp.Database.Models
{
    public class TaskModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        public User User { get; set; }
    }
}
