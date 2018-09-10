namespace EncryptedToken.Service.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EncryptedToken.UserRole")]
    public partial class UserRole
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string RoleTitle { get; set; }
    }
}
