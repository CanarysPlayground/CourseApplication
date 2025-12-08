using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Interfaces;
using CourseRegistration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseRegistration.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for WaitlistEntry operations
/// </summary>
public class WaitlistRepository : Repository<WaitlistEntry>, IWaitlistRepository
{
    /// <summary>
    /// Initializes a new instance of the WaitlistRepository
    /// </summary>
    public WaitlistRepository(CourseRegistrationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets active waitlist entries for a specific course, ordered by position
    /// </summary>
    public async Task<IEnumerable<WaitlistEntry>> GetActiveWaitlistForCourseAsync(Guid courseId)
    {
        return await _context.WaitlistEntries
            .Include(w => w.Student)
            .Include(w => w.Course)
            .Where(w => w.CourseId == courseId && w.IsActive)
            .OrderBy(w => w.Position)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a student's active waitlist entry for a specific course
    /// </summary>
    public async Task<WaitlistEntry?> GetActiveWaitlistEntryAsync(Guid studentId, Guid courseId)
    {
        return await _context.WaitlistEntries
            .Include(w => w.Student)
            .Include(w => w.Course)
            .FirstOrDefaultAsync(w => w.StudentId == studentId && w.CourseId == courseId && w.IsActive);
    }

    /// <summary>
    /// Gets all active waitlist entries for a student
    /// </summary>
    public async Task<IEnumerable<WaitlistEntry>> GetStudentActiveWaitlistEntriesAsync(Guid studentId)
    {
        return await _context.WaitlistEntries
            .Include(w => w.Student)
            .Include(w => w.Course)
            .Where(w => w.StudentId == studentId && w.IsActive)
            .OrderBy(w => w.JoinedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Checks if a student is on the active waitlist for a course
    /// </summary>
    public async Task<bool> IsStudentOnWaitlistAsync(Guid studentId, Guid courseId)
    {
        return await _context.WaitlistEntries
            .AnyAsync(w => w.StudentId == studentId && w.CourseId == courseId && w.IsActive);
    }

    /// <summary>
    /// Gets the next available position for a course waitlist
    /// </summary>
    public async Task<int> GetNextPositionAsync(Guid courseId)
    {
        var activeEntries = await _context.WaitlistEntries
            .Where(w => w.CourseId == courseId && w.IsActive)
            .ToListAsync();

        if (!activeEntries.Any())
        {
            return 1;
        }

        return activeEntries.Max(w => w.Position) + 1;
    }

    /// <summary>
    /// Gets waitlist entry with student and course details
    /// </summary>
    public async Task<WaitlistEntry?> GetWithDetailsAsync(Guid waitlistEntryId)
    {
        return await _context.WaitlistEntries
            .Include(w => w.Student)
            .Include(w => w.Course)
            .FirstOrDefaultAsync(w => w.WaitlistEntryId == waitlistEntryId);
    }

    /// <summary>
    /// Reorders waitlist positions after a student is removed
    /// </summary>
    public async Task ReorderWaitlistAsync(Guid courseId, int removedPosition)
    {
        var entriesToUpdate = await _context.WaitlistEntries
            .Where(w => w.CourseId == courseId && w.IsActive && w.Position > removedPosition)
            .ToListAsync();

        foreach (var entry in entriesToUpdate)
        {
            entry.Position--;
        }

        await _context.SaveChangesAsync();
    }
}
