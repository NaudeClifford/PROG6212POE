using PROG6212POE.Data;
using PROG6212POE.Models;
using Microsoft.EntityFrameworkCore;

namespace PROG6212POE.Services
{
    public class AdminService : IAdminService
    {
        private readonly ClaimsDBContext _context;

        public AdminService(ClaimsDBContext context)
        {
            _context = context;
        }

        public async Task<int> AddAdminAsync(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return admin.Id;
        }

        public async Task<Admin?> GetAdminAsync(int id)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Admin>> GetAdminsAsync()
        {
            return await _context.Admins.ToListAsync();
        }

        public async Task UpdateAdminAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAdminAsync(int id)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Id == id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }
        }
    }
}
