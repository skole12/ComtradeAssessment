using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Entities
{
    [PrimaryKey(nameof(Id))]
    public class User
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(60), MinLength(60)]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
    }
}
