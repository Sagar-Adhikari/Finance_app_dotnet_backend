using Microsoft.EntityFrameworkCore;
using pw.Commons.Models;


namespace pw.Auth.DAL
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions options) : base(options) { }
        public DbSet<TblUsersModel> Users {get; set;}
        public DbSet<TblRoleModel> Roles { get; set; }
        public DbSet<UserRolesModel> UserRoles { get; set; }        
        public DbSet<TblAuthModel> UserRoleTasks { get; set; }
        public DbSet<TblTaskListModel> Tasks { get; set; }
        public DbSet<TblLogModel> TblLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRolesModel>()
                .HasKey(c => new { c.UserNo, c.RoleId });
        }
    }
}
