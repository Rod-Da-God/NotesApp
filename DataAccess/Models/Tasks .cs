using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Tasks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Required]
        [Column("status")]
        public TaskStatus Status { get; set; }

        [ForeignKey("AssignedUser")]
        [Column("assigned_user_id")]
        public int AssignedUserId { get; set; }
        public User AssignedUser { get; set; }
    }

    public enum TaskStatus
    {
        New,
        InProgress,
        Completed,
        Failed,
        Postponed
    }
}