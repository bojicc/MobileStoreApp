using Microsoft.EntityFrameworkCore;
using MobileStoreApp.Data;

namespace MobileStoreApp.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int CartItemCount()
        {
            return _context.OrderItems.Sum(item => item.Quantity);
        }
    }
}
