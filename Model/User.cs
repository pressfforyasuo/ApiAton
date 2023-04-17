using System.ComponentModel.DataAnnotations;

namespace ApiAton.Model
{
    public class User
    {
        [Key]
        public Guid Guid { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, 2)]
        public int Gender { get; set; }

        public DateTime? Birthday { get; set; }

        [Required]
        public bool Admin { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime ModifiedOn { get; set; }

        [Required]
        public string ModifiedBy { get; set; }

        public DateTime? RevokedOn { get; set; }

        public string? RevokedBy { get; set; }
    }
}
