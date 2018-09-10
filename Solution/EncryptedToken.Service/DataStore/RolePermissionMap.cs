namespace EncryptedToken.Service.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EncryptedToken.RolePermissionMap")]
    public partial class RolePermissionMap
    {
        public int Id { get; set; }

        public int PermissionId { get; set; }

        public int RoleId { get; set; }
    }
}
