using PROG6212POE.Models;

namespace PROG6212POE.Services
{
    public class LecturerService : ILecturerService
    {

        private readonly ClaimsDBContext _context;

        public LecturerService(ClaimsDBContext claim)
        {
            this._context = claim;
        }
        public int AddLecturer(Lecturers lecturer)
        {
            _context.LecturerDB.Add(lecturer);
            return lecturer.Id;
        }

        public Lecturers GetLecturer(int id)
        {
            var lecturer = _context.LecturerDB.FirstOrDefault(x => x.Id == id);
            return lecturer!;
        }

        public List<Lecturers> GetLecturers()
        {
            return _context.LecturerDB.ToList();
        }

        private void FinalPayment() { }

    }
}
