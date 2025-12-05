using AutoMapper;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Interfaces;

namespace CourseRegistration.Application.Services;

/// <summary>
/// Service implementation for student operations
/// </summary>
public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the StudentService
    /// </summary>
    public StudentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Gets all students with pagination
    /// </summary>
    public async Task<PagedResponseDto<StudentDto>> GetStudentsAsync(int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var students = await _unitOfWork.Students.GetPagedAsync(page, pageSize);
        var totalStudents = await _unitOfWork.Students.CountAsync(student => ((Student)student).IsActive);

        var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(students);

        return new PagedResponseDto<StudentDto>
        {
            Items = studentDtos,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalStudents,
            TotalPages = (int)Math.Ceiling((double)totalStudents / pageSize)
        };
    }

    /// <summary>
    /// Gets a student by ID
    /// </summary>
    public async Task<StudentDto?> GetStudentByIdAsync(Guid id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        return student != null ? _mapper.Map<StudentDto>(student) : null;
    }

    /// <summary>
    /// Gets a student by email
    /// </summary>
    public async Task<StudentDto?> GetStudentByEmailAsync(string email)
    {
        var student = await _unitOfWork.Students.GetByEmailAsync(email);
        return student != null ? _mapper.Map<StudentDto>(student) : null;
    }

    /// <summary>
    /// Creates a new student
    /// </summary>
    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto)
    {
        // Check if email already exists
        var existingStudent = await _unitOfWork.Students.GetByEmailAsync(createStudentDto.Email);
        if (existingStudent != null)
        {
            throw new InvalidOperationException("A student with this email already exists.");
        }

        var student = _mapper.Map<Student>(createStudentDto);
        await _unitOfWork.Students.AddAsync(student);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<StudentDto>(student);
    }

    /// <summary>
    /// Updates an existing student
    /// </summary>
    public async Task<StudentDto?> UpdateStudentAsync(Guid id, UpdateStudentDto updateStudentDto)
    {
        var existingStudent = await _unitOfWork.Students.GetByIdAsync(id);
        if (existingStudent == null)
        {
            return null;
        }

        // Check if email is being changed and if the new email already exists
        if (existingStudent.Email != updateStudentDto.Email)
        {
            var studentWithEmail = await _unitOfWork.Students.GetByEmailAsync(updateStudentDto.Email);
            if (studentWithEmail != null)
            {
                throw new InvalidOperationException("Another student with this email already exists.");
            }
        }

        _mapper.Map(updateStudentDto, existingStudent);
        _unitOfWork.Students.Update(existingStudent);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<StudentDto>(existingStudent);
    }

    /// <summary>
    /// Deletes a student (soft delete)
    /// </summary>
    public async Task<bool> DeleteStudentAsync(Guid id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
        {
            return false;
        }

        _unitOfWork.Students.Remove(student); // This will perform soft delete
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Searches students by name
    /// </summary>
    public async Task<IEnumerable<StudentDto>> SearchStudentsAsync(string searchTerm)
    {
        var students = await _unitOfWork.Students.SearchByNameAsync(searchTerm);
        return _mapper.Map<IEnumerable<StudentDto>>(students);
    }

    /// <summary>
    /// Gets student's registrations
    /// </summary>
    public async Task<IEnumerable<RegistrationDto>> GetStudentRegistrationsAsync(Guid studentId)
    {
        var registrations = await _unitOfWork.Registrations.GetByStudentIdAsync(studentId);
        return _mapper.Map<IEnumerable<RegistrationDto>>(registrations);
    }
}