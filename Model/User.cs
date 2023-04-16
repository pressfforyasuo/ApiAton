﻿using System.ComponentModel.DataAnnotations;

namespace ApiAton.Model
{
    public class User
    {
        [Key]
        public string Guid { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Login { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$")]
        public string Name { get; set; }

        [Range(0, 2)]
        public int Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public bool Admin { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? RevokedOn { get; set; }

        public string RevokedBy { get; set; }
    }
}
