using Entity.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class WebApisContext : DbContext
    {
        public WebApisContext(DbContextOptions<WebApisContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<ProductModel> Products { get; set; }

        public DbSet<CategoryModel> Categories { get; set; }

        public DbSet<UserModel> Users { get; set; }

        public DbSet<Images> Images { get; set; }

        public DbSet<ProductAttributeModel> ProductAttributes { get; set; }

        public DbSet<ProductImage> ProductImage { get; set; }

        public DbSet<ProductAttributeValues> ProductAttributeValues { get; set; }

        public DbSet<RoleModel> Role { get; set; }

        public DbSet<PasswordResetModel> PasswordReset { get; set; }

        public DbSet<ContentModel> Content { get; set; }

        public DbSet<UserRolesModel> UserRoles { get; set; }

        public DbSet<ProductRatingReviewModel> ProductRatingReviews { get; set; }

        public DbSet<WishlistModel> Wishlist { get; set; }

        public DbSet<CartModel> Cart { get; set; }

    }
}
