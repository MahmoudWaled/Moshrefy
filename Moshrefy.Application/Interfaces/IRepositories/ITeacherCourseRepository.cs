using Moshrefy.Application.Interfaces.IGenericRepository;
using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface ITeacherCourseRepository : IGenericRepository<TeacherCourse, int>
    {

        public Task<IEnumerable<TeacherCourse>> GetByTeacherNameAsync(string teacherName);
        public Task<IEnumerable<TeacherCourse>> GetByTeacherPhoneAsync(string teacherPhone);
        public Task<IEnumerable<TeacherCourse>> GetByTeacherIdAsync(int teacherId);
    }
}