using PROG6212POE.Data;
using PROG6212POE.Models;
using Microsoft.EntityFrameworkCore;

namespace PROG6212POE.Services
{
    public class LecturerService : ILecturerService
    {
        private readonly ClaimsDBContext _context;

        public LecturerService(ClaimsDBContext context)
        {
            _context = context;
        }

        public async Task<int> AddLecturerAsync(Lecturers lecturer)
        {
            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();
            return lecturer.Id;
        }

        public async Task<Lecturers?> GetLecturerAsync(int id)
        {
            return await _context.Lecturers.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Lecturers>> GetLecturersAsync()
        {
            return await _context.Lecturers.ToListAsync();
        }

        public async Task UpdateLecturerAsync(Lecturers lecturer)
        {
            _context.Lecturers.Update(lecturer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLecturerAsync(int id)
        {
            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer != null)
            {
                _context.Lecturers.Remove(lecturer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
