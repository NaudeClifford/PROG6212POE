using PROG6212POE.Models;

namespace PROG6212POE.Services
{
    public interface ILecturerService
    {

        public int AddLecturer(Lecturers lecturer);

        public Lecturers GetLecturer(int id);

        public List<Lecturers> GetLecturers();


    }
}
