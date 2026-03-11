using Microsoft.EntityFrameworkCore;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Interfaces;
using CourseRegistration.Infrastructure.Data;

namespace CourseRegistration.Infrastructure.Repositories;

/// <summary>
/// Student repository implementation with specific operations
/// </summary>
public class StudentRepository : Repository<Student>, IStudentRepository
{
    /// <summary>
    /// Initializes a new instance of the StudentRepository
    /// </summary>
    /// <param name="context">Database context</param>
    public StudentRepository(CourseRegistrationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets a student by email address asynchronously
    /// </summary>
    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Where(student => student.IsActive)
            .FirstOrDefaultAsync(student => student.Email.ToLower() == email.ToLower());
    }

    /// <summary>
    /// Gets a student with their registrations asynchronously
    /// </summary>
    public async Task<Student?> GetWithRegistrationsAsync(Guid studentId)
    {
        return await _dbSet
            .Include(student => student.Registrations)
                .ThenInclude(registration => registration.Course)
            .Where(student => student.IsActive)
            .FirstOrDefaultAsync(student => student.StudentId == studentId);
    }

    /// <summary>
    /// Searches students by name asynchronously
    /// </summary>
    public async Task<IEnumerable<Student>> SearchByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetActiveStudentsAsync();

        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Where(student => student.IsActive && 
                       (student.FirstName.ToLower().Contains(lowerSearchTerm) ||
                        student.LastName.ToLower().Contains(lowerSearchTerm) ||
                        student.Email.ToLower().Contains(lowerSearchTerm)))
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets active students asynchronously
    /// </summary>
    public async Task<IEnumerable<Student>> GetActiveStudentsAsync()
    {
        return await _dbSet
            .Where(student => student.IsActive)
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Override GetPagedAsync to only return active students
    /// </summary>
    public override async Task<IEnumerable<Student>> GetPagedAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        return await _dbSet
            .Where(student => student.IsActive)
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Override GetByIdAsync to only return active students
    /// </summary>
    public override async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Where(student => student.IsActive)
            .FirstOrDefaultAsync(student => student.StudentId == id);
    }

    /// <summary>
    /// Soft delete implementation - mark student as inactive
    /// </summary>
    public override void Remove(Student entity)
    {
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        Update(entity);
    }
}