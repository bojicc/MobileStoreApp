using MobileStoreApp.Data.Models;

namespace MobileStoreApp.Data.Repositories
{
    public class PhoneRepository : IPhoneRepository
    {
        private readonly ApplicationDbContext _context;
        public PhoneRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Phone> GetAllPhones()
        {
            return _context.Phones.ToList();
        }

        public Phone GetPhoneById(int id)
        {
            return _context.Phones.FirstOrDefault(phone => phone.PhoneId == id);
        }
    }
}
