using Microsoft.EntityFrameworkCore;
using utools.Models;

namespace utools.Data
{
    public partial class DataContext : DbContext
    {
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Cnae> Cnaes { get; set; }

        public DataContext()
        { }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=" + Settings.Server + ";User Id="
                    + Settings.DbUser + ";Password=" + Settings.DbPwd
                    + ";Database=" + Settings.DbName);  
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }
    }
}