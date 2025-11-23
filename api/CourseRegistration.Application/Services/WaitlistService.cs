using AutoMapper;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Interfaces;

namespace CourseRegistration.Application.Services;

/// <summary>
/// Service implementation for waitlist operations
/// </summary>
public class WaitlistService : IWaitlistService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    /// <summary>
    /// Initializes a new instance of the WaitlistService
    /// </summary>
    public WaitlistService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    /// <summary>
    /// Adds a student to a course waitlist
    /// </summary>
    public async Task<WaitlistEntryDto> JoinWaitlistAsync(CreateWaitlistEntryDto createWaitlistEntryDto)
    {
        // Validate student exists
        var student = await _unitOfWork.Students.GetByIdAsync(createWaitlistEntryDto.StudentId);
        if (student == null)
        {
            throw new InvalidOperationException("Student not found.");
        }

        // Validate course exists
        var course = await _unitOfWork.Courses.GetByIdAsync(createWaitlistEntryDto.CourseId);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found.");
        }

        // Check if course is active
        if (!course.IsActive)
        {
            throw new InvalidOperationException("Cannot join waitlist for an inactive course.");
        }

        // Check if student is already registered for this course
        var isRegistered = await _unitOfWork.Registrations.IsStudentRegisteredForCourseAsync(
            createWaitlistEntryDto.StudentId, createWaitlistEntryDto.CourseId);
        if (isRegistered)
        {
            throw new InvalidOperationException("Student is already registered for this course.");
        }

        // Check if student is already on the waitlist
        var isOnWaitlist = await _unitOfWork.Waitlists.IsStudentOnWaitlistAsync(
            createWaitlistEntryDto.StudentId, createWaitlistEntryDto.CourseId);
        if (isOnWaitlist)
        {
            throw new InvalidOperationException("Student is already on the waitlist for this course.");
        }

        // Get next position in the waitlist
        var nextPosition = await _unitOfWork.Waitlists.GetNextPositionAsync(createWaitlistEntryDto.CourseId);

        // Create waitlist entry
        var waitlistEntry = _mapper.Map<WaitlistEntry>(createWaitlistEntryDto);
        waitlistEntry.Position = nextPosition;
        waitlistEntry.JoinedAt = DateTime.UtcNow;
        waitlistEntry.IsActive = true;

        await _unitOfWork.Waitlists.AddAsync(waitlistEntry);
        await _unitOfWork.SaveChangesAsync();

        // Get the waitlist entry with related entities
        var savedEntry = await _unitOfWork.Waitlists.GetWithDetailsAsync(waitlistEntry.WaitlistEntryId);
        return _mapper.Map<WaitlistEntryDto>(savedEntry);
    }

    /// <summary>
    /// Removes a student from a course waitlist
    /// </summary>
    public async Task<bool> LeaveWaitlistAsync(Guid waitlistEntryId)
    {
        var waitlistEntry = await _unitOfWork.Waitlists.GetByIdAsync(waitlistEntryId);
        if (waitlistEntry == null || !waitlistEntry.IsActive)
        {
            return false;
        }

        var courseId = waitlistEntry.CourseId;
        var position = waitlistEntry.Position;

        // Mark as inactive instead of deleting
        waitlistEntry.IsActive = false;
        _unitOfWork.Waitlists.Update(waitlistEntry);
        await _unitOfWork.SaveChangesAsync();

        // Reorder remaining waitlist entries
        await _unitOfWork.Waitlists.ReorderWaitlistAsync(courseId, position);

        return true;
    }

    /// <summary>
    /// Gets active waitlist entries for a course
    /// </summary>
    public async Task<IEnumerable<WaitlistEntryDto>> GetCourseWaitlistAsync(Guid courseId)
    {
        var waitlistEntries = await _unitOfWork.Waitlists.GetActiveWaitlistForCourseAsync(courseId);
        return _mapper.Map<IEnumerable<WaitlistEntryDto>>(waitlistEntries);
    }

    /// <summary>
    /// Gets a student's active waitlist entries
    /// </summary>
    public async Task<IEnumerable<WaitlistEntryDto>> GetStudentWaitlistsAsync(Guid studentId)
    {
        var waitlistEntries = await _unitOfWork.Waitlists.GetStudentActiveWaitlistEntriesAsync(studentId);
        return _mapper.Map<IEnumerable<WaitlistEntryDto>>(waitlistEntries);
    }

    /// <summary>
    /// Gets a specific waitlist entry by ID
    /// </summary>
    public async Task<WaitlistEntryDto?> GetWaitlistEntryAsync(Guid waitlistEntryId)
    {
        var waitlistEntry = await _unitOfWork.Waitlists.GetWithDetailsAsync(waitlistEntryId);
        return waitlistEntry != null ? _mapper.Map<WaitlistEntryDto>(waitlistEntry) : null;
    }

    /// <summary>
    /// Updates a waitlist entry (admin function)
    /// </summary>
    public async Task<WaitlistEntryDto?> UpdateWaitlistEntryAsync(Guid waitlistEntryId, UpdateWaitlistEntryDto updateDto)
    {
        var existingEntry = await _unitOfWork.Waitlists.GetWithDetailsAsync(waitlistEntryId);
        if (existingEntry == null)
        {
            return null;
        }

        // Update only the fields that are provided
        if (updateDto.NotificationPreference.HasValue)
        {
            existingEntry.NotificationPreference = updateDto.NotificationPreference.Value;
        }

        if (updateDto.Notes != null)
        {
            existingEntry.Notes = updateDto.Notes;
        }

        // Handle position change if provided (admin reordering)
        if (updateDto.Position.HasValue && updateDto.Position.Value != existingEntry.Position)
        {
            var oldPosition = existingEntry.Position;
            var newPosition = updateDto.Position.Value;

            // Get all active waitlist entries for the course
            var allEntries = await _unitOfWork.Waitlists.GetActiveWaitlistForCourseAsync(existingEntry.CourseId);
            var entriesList = allEntries.ToList();

            // Validate new position
            if (newPosition < 1 || newPosition > entriesList.Count)
            {
                throw new InvalidOperationException("Invalid position specified.");
            }

            // Reorder entries
            if (newPosition < oldPosition)
            {
                // Moving up in the list
                foreach (var entry in entriesList.Where(e => e.Position >= newPosition && e.Position < oldPosition))
                {
                    entry.Position++;
                }
            }
            else
            {
                // Moving down in the list
                foreach (var entry in entriesList.Where(e => e.Position > oldPosition && e.Position <= newPosition))
                {
                    entry.Position--;
                }
            }

            existingEntry.Position = newPosition;
        }

        _unitOfWork.Waitlists.Update(existingEntry);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<WaitlistEntryDto>(existingEntry);
    }

    /// <summary>
    /// Notifies the next student on the waitlist when a spot becomes available
    /// </summary>
    public async Task NotifyNextStudentAsync(Guid courseId)
    {
        var waitlistEntries = await _unitOfWork.Waitlists.GetActiveWaitlistForCourseAsync(courseId);
        var nextEntry = waitlistEntries.OrderBy(w => w.Position).FirstOrDefault();

        if (nextEntry != null)
        {
            // Send notification based on preference
            await _notificationService.SendWaitlistNotificationAsync(
                nextEntry.Student.Email,
                nextEntry.Student.FullName,
                nextEntry.Course.CourseName,
                nextEntry.Position,
                nextEntry.NotificationPreference);

            // Mark as notified
            nextEntry.NotifiedAt = DateTime.UtcNow;
            _unitOfWork.Waitlists.Update(nextEntry);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Clears the entire waitlist for a course (admin function)
    /// </summary>
    public async Task<bool> ClearWaitlistAsync(Guid courseId)
    {
        var waitlistEntries = await _unitOfWork.Waitlists.GetActiveWaitlistForCourseAsync(courseId);
        
        foreach (var entry in waitlistEntries)
        {
            entry.IsActive = false;
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Reorders waitlist entries (admin function)
    /// </summary>
    public async Task<bool> ReorderWaitlistAsync(Guid courseId, Dictionary<Guid, int> newPositions)
    {
        var waitlistEntries = await _unitOfWork.Waitlists.GetActiveWaitlistForCourseAsync(courseId);
        var entriesList = waitlistEntries.ToList();

        // Validate that all entries are included in the reorder
        if (newPositions.Count != entriesList.Count)
        {
            throw new InvalidOperationException(
                $"All waitlist entries must be included. Expected {entriesList.Count} positions but got {newPositions.Count}.");
        }

        // Validate that all entry IDs are present
        var missingEntries = entriesList.Where(e => !newPositions.ContainsKey(e.WaitlistEntryId)).ToList();
        if (missingEntries.Any())
        {
            throw new InvalidOperationException(
                $"Missing position assignments for {missingEntries.Count} waitlist entry/entries.");
        }

        // Validate that all positions are unique and sequential
        var positions = newPositions.Values.OrderBy(p => p).ToList();
        for (int i = 0; i < positions.Count; i++)
        {
            if (positions[i] != i + 1)
            {
                throw new InvalidOperationException("Positions must be sequential starting from 1.");
            }
        }

        // Update positions
        foreach (var entry in entriesList)
        {
            entry.Position = newPositions[entry.WaitlistEntryId];
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Checks if a student is on the waitlist for a course
    /// </summary>
    public async Task<bool> IsStudentOnWaitlistAsync(Guid studentId, Guid courseId)
    {
        return await _unitOfWork.Waitlists.IsStudentOnWaitlistAsync(studentId, courseId);
    }
}
