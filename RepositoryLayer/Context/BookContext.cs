using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Context
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<WishList> Wishlists { get; set; }
        public DbSet<Order> Orders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Book>()
                .Property(b => b.DiscountPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Cart>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18, 2)");  // Add this line for Cart.Price
        }


    }
}
