using AutoMapper;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Interfaces;

namespace CourseRegistration.Application.Services;

/// <summary>
/// Service implementation for course operations
/// </summary>
public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the CourseService
    /// </summary>
    public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Gets all courses with pagination and filtering
    /// </summary>
    public async Task<PagedResponseDto<CourseDto>> GetCoursesAsync(int page = 1, int pageSize = 10, string? searchTerm = null, string? instructor = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        IEnumerable<Course> courses;
        int totalCourses;

        if (!string.IsNullOrWhiteSpace(searchTerm) || !string.IsNullOrWhiteSpace(instructor))
        {
            courses = await _unitOfWork.Courses.SearchCoursesAsync(searchTerm, instructor);
            totalCourses = courses.Count();
            courses = courses.Skip((page - 1) * pageSize).Take(pageSize);
        }
        else
        {
            courses = await _unitOfWork.Courses.GetPagedAsync(page, pageSize);
            totalCourses = await _unitOfWork.Courses.CountAsync(course => ((Course)course).IsActive);
        }

        var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);

        return new PagedResponseDto<CourseDto>
        {
            Items = courseDtos,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCourses,
            TotalPages = (int)Math.Ceiling((double)totalCourses / pageSize)
        };
    }

    /// <summary>
    /// Gets a course by ID
    /// </summary>
    public async Task<CourseDto?> GetCourseByIdAsync(Guid id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        return course != null ? _mapper.Map<CourseDto>(course) : null;
    }

    /// <summary>
    /// Creates a new course
    /// </summary>
    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto)
    {
        // Validate start date is in the future
        if (createCourseDto.StartDate <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Course start date must be in the future.");
        }

        // Validate end date is after start date
        if (createCourseDto.EndDate <= createCourseDto.StartDate)
        {
            throw new InvalidOperationException("Course end date must be after start date.");
        }

        var course = _mapper.Map<Course>(createCourseDto);
        await _unitOfWork.Courses.AddAsync(course);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CourseDto>(course);
    }

    /// <summary>
    /// Updates an existing course
    /// </summary>
    public async Task<CourseDto?> UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto)
    {
        var existingCourse = await _unitOfWork.Courses.GetByIdAsync(id);
        if (existingCourse == null)
        {
            return null;
        }

        // Validate end date is after start date
        if (updateCourseDto.EndDate <= updateCourseDto.StartDate)
        {
            throw new InvalidOperationException("Course end date must be after start date.");
        }

        _mapper.Map(updateCourseDto, existingCourse);
        _unitOfWork.Courses.Update(existingCourse);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CourseDto>(existingCourse);
    }

    /// <summary>
    /// Deletes a course (soft delete)
    /// </summary>
    public async Task<bool> DeleteCourseAsync(Guid id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        if (course == null)
        {
            return false;
        }

        // Check if course has active registrations
        var activeRegistrations = await _unitOfWork.Registrations.GetByCourseIdAsync(id);
        if (activeRegistrations.Any(registration => registration.Status == Domain.Enums.RegistrationStatus.Confirmed))
        {
            throw new InvalidOperationException("Cannot delete a course with active registrations.");
        }

        _unitOfWork.Courses.Remove(course); // This will perform soft delete
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Searches courses by name, instructor, or description
    /// </summary>
    public async Task<IEnumerable<CourseDto>> SearchCoursesAsync(string? searchTerm, string? instructor)
    {
        var courses = await _unitOfWork.Courses.SearchCoursesAsync(searchTerm, instructor);
        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    /// <summary>
    /// Gets available courses for registration
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetAvailableCoursesAsync()
    {
        var courses = await _unitOfWork.Courses.GetAvailableCoursesAsync();
        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    /// <summary>
    /// Gets courses by instructor
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(string instructorName)
    {
        var courses = await _unitOfWork.Courses.GetCoursesByInstructorAsync(instructorName);
        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    /// <summary>
    /// Gets course registrations
    /// </summary>
    public async Task<IEnumerable<RegistrationDto>> GetCourseRegistrationsAsync(Guid courseId)
    {
        var registrations = await _unitOfWork.Registrations.GetByCourseIdAsync(courseId);
        return _mapper.Map<IEnumerable<RegistrationDto>>(registrations);
    }
}