using System.ComponentModel.DataAnnotations;

namespace neuranx.Domain.Entities;

public class TaskItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "NEW"; // Pending, InProgress, Completed

    public Guid AssignedToUserId { get; set; }

    public virtual ApplicationUser AssignedUser { get; set; }
    public Guid UserId { get; set; } // Creator of the task
}

public class TaskList
{
    public string Name { get; set; }
    public TaskItem Tasks { get; set; }
}
