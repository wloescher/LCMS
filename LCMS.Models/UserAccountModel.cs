using System.ComponentModel.DataAnnotations;

namespace LCMS.Models
{
    public class UserAccountModel
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = new();

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Password { get; set; }

        [MaxLength(255)]
        public string? PasswordHash { get; set; }

        public int PasswordAttemptCount { get; set; }

        public DateTime CreatedDate { get; set; } = new DateTime(1900, 1, 1);

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime ModifiedDate { get; set; } = new DateTime(1900, 1, 1);

        public string ModifiedBy { get; set; } = string.Empty;
    }
}
