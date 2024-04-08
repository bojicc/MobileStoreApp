using MobileStoreApp.Data.Models;

namespace MobileStoreApp.Data.Repositories
{
    public interface IPhoneRepository
    {
        IEnumerable<Phone> GetAllPhones();
        Phone GetPhoneById(int id);
        //void AddPhone(Phone phone);
    }
}
