using Microsoft.EntityFrameworkCore;

namespace RefactorThis.Persistence
{
    public class RefactorThisContext : DbContext
    {
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;

        public RefactorThisContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}