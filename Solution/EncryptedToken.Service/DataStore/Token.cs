namespace EncryptedToken.Service.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EncryptedToken.Token")]
    public partial class Token
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(700)]
        public string Roles { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime ExpiryDateTime { get; set; }

        public DateTime? LogoutDateTime { get; set; }

        [StringLength(1000)]
        public string EncryptedValue { get; set; }
    }
}
