using AutoMapper;
using Moshrefy.Application.DTOs.TeacherCourse;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class TeacherCourseService(IUnitOfWork unitOfWork, IMapper mapper) : ITeacherCourseService
    {
        public async Task<TeacherCourseResponseDTO> CreateAsync(CreateTeacherCourseDTO createTeacherCourseDTO)
        {
            // Check if already exists
            var existing = await unitOfWork.TeacherCourses.GetAllAsync(new PaginationParamter());
            if (existing.Any(tc => tc.TeacherId == createTeacherCourseDTO.TeacherId && tc.CourseId == createTeacherCourseDTO.CourseId))
            {
                throw new BadRequestException("Teacher is already assigned to this course.");
            }

            var teacherCourse = mapper.Map<TeacherCourse>(createTeacherCourseDTO);
            await unitOfWork.TeacherCourses.AddAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<TeacherCourseResponseDTO>(teacherCourse);
        }

        public async Task<TeacherCourseResponseDTO?> GetByIdAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            return mapper.Map<TeacherCourseResponseDTO>(teacherCourse);
        }

        public async Task<List<TeacherCourseResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetAllAsync(paginationParamter);
            return mapper.Map<List<TeacherCourseResponseDTO>>(teacherCourses);
        }

        public async Task<List<TeacherCourseResponseDTO>> GetByTeacherIdAsync(int teacherId)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetAllAsync(new PaginationParamter());
            var filtered = teacherCourses.Where(tc => tc.TeacherId == teacherId).ToList();
            return mapper.Map<List<TeacherCourseResponseDTO>>(filtered);
        }

        public async Task<List<TeacherCourseResponseDTO>> GetByCourseIdAsync(int courseId)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetAllAsync(new PaginationParamter());
            var filtered = teacherCourses.Where(tc => tc.CourseId == courseId).ToList();
            return mapper.Map<List<TeacherCourseResponseDTO>>(filtered);
        }

        public async Task<List<TeacherCourseResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetAllAsync(paginationParamter);
            var active = teacherCourses.Where(tc => tc.IsActive).ToList();
            return mapper.Map<List<TeacherCourseResponseDTO>>(active);
        }

        public async Task<List<TeacherCourseResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetAllAsync(paginationParamter);
            var inactive = teacherCourses.Where(tc => !tc.IsActive).ToList();
            return mapper.Map<List<TeacherCourseResponseDTO>>(inactive);
        }

        public async Task UpdateAsync(int id, UpdateTeacherCourseDTO updateTeacherCourseDTO)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            mapper.Map(updateTeacherCourseDTO, teacherCourse);
            unitOfWork.TeacherCourses.UpdateAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            unitOfWork.TeacherCourses.DeleteAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            teacherCourse.IsDeleted = true;
            unitOfWork.TeacherCourses.UpdateAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            teacherCourse.IsDeleted = false;
            unitOfWork.TeacherCourses.UpdateAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            teacherCourse.IsActive = true;
            unitOfWork.TeacherCourses.UpdateAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var teacherCourse = await unitOfWork.TeacherCourses.GetByIdAsync(id);
            if (teacherCourse == null)
                throw new NotFoundException<int>(nameof(teacherCourse), "teacherCourse", id);

            teacherCourse.IsActive = false;
            unitOfWork.TeacherCourses.UpdateAsync(teacherCourse);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsTeacherAssignedToCourseAsync(int teacherId, int courseId)
        {
            var teacherCourses = await unitOfWork.TeacherCourses.GetAllAsync(new PaginationParamter());
            return teacherCourses.Any(tc => tc.TeacherId == teacherId && tc.CourseId == courseId && !tc.IsDeleted);
        }
    }
}