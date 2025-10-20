using PROG6212POE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PROG6212POE.Services
{
    public interface IAdminService
    {
        Task<int> AddAdminAsync(Admin admin);

        Task<Admin?> GetAdminAsync(int id);

        Task<List<Admin>> GetAdminsAsync();

        Task UpdateAdminAsync(Admin admin);

        Task DeleteAdminAsync(int id);
    }
}
