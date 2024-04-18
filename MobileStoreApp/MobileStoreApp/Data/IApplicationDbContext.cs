using Microsoft.EntityFrameworkCore;
using MobileStoreApp.Data.Models;

namespace MobileStoreApp.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Phone> Phones { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ApplicationUser> ApplicationsUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void Update<T>(T entity) where T : class;
        void Add<T>(T entity) where T : class;
    }
}
