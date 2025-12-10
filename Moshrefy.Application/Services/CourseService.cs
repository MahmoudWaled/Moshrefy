using AutoMapper;
using Moshrefy.Application.DTOs.Course;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Services
{
    public class CourseService(IUnitOfWork unitOfWork, IMapper mapper) : ICourseService
    {
        public async Task<CourseResponseDTO> CreateAsync(CreateCourseDTO createCourseDTO)
        {
            var course = mapper.Map<Course>(createCourseDTO);
            await unitOfWork.Courses.AddAsync(course);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<CourseResponseDTO>(course);
        }

        public async Task<CourseResponseDTO?> GetByIdAsync(int id)
        {
            var course = await unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(course), "course", id);

            return mapper.Map<CourseResponseDTO>(course);
        }

        public async Task<List<CourseResponseDTO>> GetAllAsync(PaginationParamter paginationParamter)
        {
            var courses = await unitOfWork.Courses.GetAllAsync(paginationParamter);
            return mapper.Map<List<CourseResponseDTO>>(courses);
        }

        public async Task<List<CourseResponseDTO>> GetByNameAsync(string name)
        {
            var courses = await unitOfWork.Courses.GetByName(name);
            return mapper.Map<List<CourseResponseDTO>>(courses);
        }

        public async Task<List<CourseResponseDTO>> GetActiveAsync(PaginationParamter paginationParamter)
        {
            var courses = await unitOfWork.Courses.GetAllAsync(paginationParamter);
            var activeCourses = courses.Where(c => c.IsActive).ToList();
            return mapper.Map<List<CourseResponseDTO>>(activeCourses);
        }

        public async Task<List<CourseResponseDTO>> GetInactiveAsync(PaginationParamter paginationParamter)
        {
            var courses = await unitOfWork.Courses.GetAllAsync(paginationParamter);
            var inactiveCourses = courses.Where(c => !c.IsActive).ToList();
            return mapper.Map<List<CourseResponseDTO>>(inactiveCourses);
        }

        public async Task<List<CourseResponseDTO>> GetByAcademicYearIdAsync(int academicYearId)
        {
            var courses = await unitOfWork.Courses.GetAllAsync(new PaginationParamter());
            var coursesByYear = courses.Where(c => c.AcademicYearId == academicYearId).ToList();
            return mapper.Map<List<CourseResponseDTO>>(coursesByYear);
        }

        public async Task UpdateAsync(int id, UpdateCourseDTO updateCourseDTO)
        {
            var course = await unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(course), "course", id);

            mapper.Map(updateCourseDTO, course);
            unitOfWork.Courses.UpdateAsync(course);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var course = await unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(course), "course", id);

            unitOfWork.Courses.DeleteAsync(course);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var course = await unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(course), "course", id);

            course.IsActive = true;
            unitOfWork.Courses.UpdateAsync(course);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var course = await unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(course), "course", id);

            course.IsActive = false;
            unitOfWork.Courses.UpdateAsync(course);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var course = await unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(course), "course", id);

            course.IsDeleted = true;
            unitOfWork.Courses.UpdateAsync(course);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var course = await unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException<int>(nameof(course), "course", id);

            course.IsDeleted = false;
            unitOfWork.Courses.UpdateAsync(course);
            await unitOfWork.SaveChangesAsync();
        }
    }
}