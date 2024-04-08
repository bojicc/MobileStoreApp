using MobileStoreApp.Data.Models;

namespace MobileStoreApp.Data.Repositories
{
    public interface IOrderRepository
    {
        void PlaceOrder(Order order);
    }
}
