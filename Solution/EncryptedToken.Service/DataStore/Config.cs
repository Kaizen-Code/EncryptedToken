namespace EncryptedToken.Service.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EncryptedToken.Config")]
    public partial class Config
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemKey { get; set; }

        [Required]
        [StringLength(500)]
        public string ItemValue { get; set; }
    }
}
