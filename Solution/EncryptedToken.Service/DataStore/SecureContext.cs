namespace EncryptedToken.Service.DataStore
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SecureContext : DbContext
    {
        public SecureContext()
            : base("name=SecureContext")
        {
        }

        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<RolePermissionMap> RolePermissionMaps { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Config>()
                .Property(e => e.ItemKey)
                .IsUnicode(false);

            modelBuilder.Entity<Config>()
                .Property(e => e.ItemValue)
                .IsUnicode(false);

            modelBuilder.Entity<Permission>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Token>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<Token>()
                .Property(e => e.Roles)
                .IsUnicode(false);

            modelBuilder.Entity<Token>()
                .Property(e => e.EncryptedValue)
                .IsUnicode(false);

            modelBuilder.Entity<UserRole>()
                .Property(e => e.RoleTitle)
                .IsUnicode(false);
        }
    }
}
