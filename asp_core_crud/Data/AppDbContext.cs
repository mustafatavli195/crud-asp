using asp_core_crud.Models;
using Microsoft.EntityFrameworkCore;



namespace asp_core_crud.Data
{
    public class AppDbContext : DbContext
    {

        //Ctor. base ile inherite aldığımız optionsa aktarıyor.galiba di bu
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tablo oluşturuyoruz
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }


        // Burada ise ilişkileri kuracağız
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)");
            });


            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Books" }
                );

            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductDetail)
                .WithOne(pd => pd.Product)
                .HasForeignKey<ProductDetail>(pd => pd.ProductId);
        }
    }
}
