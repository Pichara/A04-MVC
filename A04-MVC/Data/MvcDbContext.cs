/*
* FILE         : MvcDbContext.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Bramdyy
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : Database context for Entity Framework Core
*                Manages database connection and entity mappings
*/

using A04_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace A04_MVC.Data
{
    /// <summary>
    /// Entity Framework Core database context for the MVC inventory system
    /// </summary>
    public class MvcDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance with specified options
        /// </summary>
        /// <param name="options">Database context options.</param>
        public MvcDbContext(DbContextOptions<MvcDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet for LoginInfo entities
        /// </summary>
        public DbSet<LoginInfo> LoginInfos { get; set; } = null!;

        /// <summary>
        /// DbSet for Category entities
        /// </summary>
        public DbSet<Category> Categories { get; set; } = null!;

        /// <summary>
        /// DbSet for ItemCatalog entities
        /// </summary>
        public DbSet<ItemCatalog> ItemCatalogs { get; set; } = null!;

        /// <summary>
        /// Configures entity relationships and table mappings
        /// </summary>
        /// <param name="modelBuilder">Model builder for entity configuration.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite key for ItemCatalog
            modelBuilder.Entity<ItemCatalog>()
                .HasKey(ic => new { ic.UserId, ic.Item, ic.CategoryId });

            // Configure User-Item relationship
            modelBuilder.Entity<ItemCatalog>()
                .HasOne(ic => ic.User)
                .WithMany(u => u.ItemCatalogs)
                .HasForeignKey(ic => ic.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Category-Item relationship
            modelBuilder.Entity<ItemCatalog>()
                .HasOne(ic => ic.Category)
                .WithMany(c => c.ItemCatalogs)
                .HasForeignKey(ic => ic.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Furniture" },
                new Category { CategoryId = 2, CategoryName = "Electronics" },
                new Category { CategoryId = 3, CategoryName = "Appliances" },
                new Category { CategoryId = 4, CategoryName = "Clothing" },
                new Category { CategoryId = 5, CategoryName = "Decor & Bedding" },
                new Category { CategoryId = 6, CategoryName = "Kitchenwares" },
                new Category { CategoryId = 7, CategoryName = "Tools & Equipment" },
                new Category { CategoryId = 8, CategoryName = "Valuables & Collectables" },
                new Category { CategoryId = 9, CategoryName = "Personal Health" }
            );

            return;
        }
    }
}
