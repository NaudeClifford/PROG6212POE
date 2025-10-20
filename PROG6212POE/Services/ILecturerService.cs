using PROG6212POE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PROG6212POE.Services
{
    public interface ILecturerService
    {
        Task<int> AddLecturerAsync(Lecturers lecturer);

        Task<Lecturers?> GetLecturerAsync(int id);

        Task<List<Lecturers>> GetLecturersAsync();

        Task UpdateLecturerAsync(Lecturers lecturer);

        Task DeleteLecturerAsync(int id);
    }
}
