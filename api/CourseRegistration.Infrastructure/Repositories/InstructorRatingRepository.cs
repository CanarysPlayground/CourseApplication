using Microsoft.EntityFrameworkCore;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Interfaces;
using CourseRegistration.Infrastructure.Data;

namespace CourseRegistration.Infrastructure.Repositories;

/// <summary>
/// InstructorRating repository implementation with specific operations
/// </summary>
public class InstructorRatingRepository : Repository<InstructorRating>, IInstructorRatingRepository
{
    /// <summary>
    /// Initializes a new instance of the InstructorRatingRepository
    /// </summary>
    /// <param name="context">Database context</param>
    public InstructorRatingRepository(CourseRegistrationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets all ratings for a specific course
    /// </summary>
    public async Task<IEnumerable<InstructorRating>> GetByCourseIdAsync(Guid courseId)
    {
        return await _dbSet
            .Include(ir => ir.Student)
            .Include(ir => ir.Course)
            .Where(ir => ir.CourseId == courseId)
            .OrderByDescending(ir => ir.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all ratings submitted by a specific student
    /// </summary>
    public async Task<IEnumerable<InstructorRating>> GetByStudentIdAsync(Guid studentId)
    {
        return await _dbSet
            .Include(ir => ir.Course)
            .Include(ir => ir.Student)
            .Where(ir => ir.StudentId == studentId)
            .OrderByDescending(ir => ir.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all ratings for a specific instructor across all courses
    /// </summary>
    public async Task<IEnumerable<InstructorRating>> GetByInstructorNameAsync(string instructorName)
    {
        return await _dbSet
            .Include(ir => ir.Course)
            .Include(ir => ir.Student)
            .Where(ir => ir.Course.InstructorName == instructorName)
            .OrderByDescending(ir => ir.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a specific rating by student and course
    /// </summary>
    public async Task<InstructorRating?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        return await _dbSet
            .Include(ir => ir.Course)
            .Include(ir => ir.Student)
            .FirstOrDefaultAsync(ir => ir.StudentId == studentId && ir.CourseId == courseId);
    }

    /// <summary>
    /// Checks if a student has already rated a course
    /// </summary>
    public async Task<bool> HasStudentRatedCourseAsync(Guid studentId, Guid courseId)
    {
        return await _dbSet
            .AnyAsync(ir => ir.StudentId == studentId && ir.CourseId == courseId);
    }

    /// <summary>
    /// Gets average rating for an instructor
    /// </summary>
    public async Task<double?> GetAverageRatingForInstructorAsync(string instructorName)
    {
        var ratings = await _dbSet
            .Include(ir => ir.Course)
            .Where(ir => ir.Course.InstructorName == instructorName)
            .ToListAsync();

        return ratings.Any() ? ratings.Average(ir => ir.Rating) : null;
    }

    /// <summary>
    /// Gets rating with student and course details
    /// </summary>
    public async Task<InstructorRating?> GetWithDetailsAsync(Guid ratingId)
    {
        return await _dbSet
            .Include(ir => ir.Course)
            .Include(ir => ir.Student)
            .FirstOrDefaultAsync(ir => ir.RatingId == ratingId);
    }
}
