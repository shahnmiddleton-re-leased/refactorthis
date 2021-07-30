public class RefactorThisContext : DbContext
{
    public virtual DbSet<Invoice>? Invoices { get; set; }
    public virtual DbSet<Payment>? Payments { get; set; }

    public RefactorThisContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}