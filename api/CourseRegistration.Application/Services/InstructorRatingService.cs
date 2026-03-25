using AutoMapper;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Interfaces;

namespace CourseRegistration.Application.Services;

/// <summary>
/// Service implementation for instructor rating operations
/// </summary>
public class InstructorRatingService : IInstructorRatingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the InstructorRatingService
    /// </summary>
    public InstructorRatingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Creates a new instructor rating
    /// </summary>
    public async Task<InstructorRatingDto> CreateRatingAsync(CreateInstructorRatingDto createRatingDto)
    {
        // Validate that the course exists
        var course = await _unitOfWork.Courses.GetByIdAsync(createRatingDto.CourseId);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found.");
        }

        // Validate that the student exists
        var student = await _unitOfWork.Students.GetByIdAsync(createRatingDto.StudentId);
        if (student == null)
        {
            throw new InvalidOperationException("Student not found.");
        }

        // Check if student has already rated this course
        var existingRating = await _unitOfWork.InstructorRatings
            .GetByStudentAndCourseAsync(createRatingDto.StudentId, createRatingDto.CourseId);
        
        if (existingRating != null)
        {
            throw new InvalidOperationException("Student has already rated this course. Use update instead.");
        }

        // Validate that student is enrolled in the course
        var isEnrolled = await _unitOfWork.Registrations
            .IsStudentRegisteredForCourseAsync(createRatingDto.StudentId, createRatingDto.CourseId);
        
        if (!isEnrolled)
        {
            throw new InvalidOperationException("Student must be enrolled in the course to submit a rating.");
        }

        var rating = _mapper.Map<InstructorRating>(createRatingDto);
        await _unitOfWork.InstructorRatings.AddAsync(rating);
        await _unitOfWork.SaveChangesAsync();

        // Retrieve with details for response
        var ratingWithDetails = await _unitOfWork.InstructorRatings.GetWithDetailsAsync(rating.RatingId);
        return _mapper.Map<InstructorRatingDto>(ratingWithDetails);
    }

    /// <summary>
    /// Gets a rating by ID
    /// </summary>
    public async Task<InstructorRatingDto?> GetRatingByIdAsync(Guid ratingId)
    {
        var rating = await _unitOfWork.InstructorRatings.GetWithDetailsAsync(ratingId);
        return rating != null ? _mapper.Map<InstructorRatingDto>(rating) : null;
    }

    /// <summary>
    /// Gets all ratings for a specific course
    /// </summary>
    public async Task<IEnumerable<InstructorRatingDto>> GetRatingsByCourseIdAsync(Guid courseId)
    {
        var ratings = await _unitOfWork.InstructorRatings.GetByCourseIdAsync(courseId);
        return _mapper.Map<IEnumerable<InstructorRatingDto>>(ratings);
    }

    /// <summary>
    /// Gets all ratings submitted by a specific student
    /// </summary>
    public async Task<IEnumerable<InstructorRatingDto>> GetRatingsByStudentIdAsync(Guid studentId)
    {
        var ratings = await _unitOfWork.InstructorRatings.GetByStudentIdAsync(studentId);
        return _mapper.Map<IEnumerable<InstructorRatingDto>>(ratings);
    }

    /// <summary>
    /// Gets all ratings for a specific instructor
    /// </summary>
    public async Task<IEnumerable<InstructorRatingDto>> GetRatingsByInstructorNameAsync(string instructorName)
    {
        var ratings = await _unitOfWork.InstructorRatings.GetByInstructorNameAsync(instructorName);
        return _mapper.Map<IEnumerable<InstructorRatingDto>>(ratings);
    }

    /// <summary>
    /// Gets rating statistics for an instructor
    /// </summary>
    public async Task<InstructorRatingStatsDto?> GetInstructorStatsAsync(string instructorName)
    {
        var ratings = await _unitOfWork.InstructorRatings.GetByInstructorNameAsync(instructorName);
        var ratingsList = ratings.ToList();

        if (!ratingsList.Any())
        {
            return null;
        }

        var courses = await _unitOfWork.Courses.GetCoursesByInstructorAsync(instructorName);
        
        return new InstructorRatingStatsDto
        {
            InstructorName = instructorName,
            AverageRating = ratingsList.Average(r => r.Rating),
            TotalRatings = ratingsList.Count,
            FiveStarCount = ratingsList.Count(r => r.Rating == 5),
            FourStarCount = ratingsList.Count(r => r.Rating == 4),
            ThreeStarCount = ratingsList.Count(r => r.Rating == 3),
            TwoStarCount = ratingsList.Count(r => r.Rating == 2),
            OneStarCount = ratingsList.Count(r => r.Rating == 1),
            CourseIds = courses.Select(c => c.CourseId).ToList()
        };
    }

    /// <summary>
    /// Updates an existing rating
    /// </summary>
    public async Task<InstructorRatingDto?> UpdateRatingAsync(Guid ratingId, UpdateInstructorRatingDto updateRatingDto)
    {
        var existingRating = await _unitOfWork.InstructorRatings.GetByIdAsync(ratingId);
        if (existingRating == null)
        {
            return null;
        }

        _mapper.Map(updateRatingDto, existingRating);
        _unitOfWork.InstructorRatings.Update(existingRating);
        await _unitOfWork.SaveChangesAsync();

        var ratingWithDetails = await _unitOfWork.InstructorRatings.GetWithDetailsAsync(ratingId);
        return _mapper.Map<InstructorRatingDto>(ratingWithDetails);
    }

    /// <summary>
    /// Deletes a rating
    /// </summary>
    public async Task<bool> DeleteRatingAsync(Guid ratingId)
    {
        var rating = await _unitOfWork.InstructorRatings.GetByIdAsync(ratingId);
        if (rating == null)
        {
            return false;
        }

        _unitOfWork.InstructorRatings.Remove(rating);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Gets a rating by student and course
    /// </summary>
    public async Task<InstructorRatingDto?> GetRatingByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        var rating = await _unitOfWork.InstructorRatings.GetByStudentAndCourseAsync(studentId, courseId);
        return rating != null ? _mapper.Map<InstructorRatingDto>(rating) : null;
    }
}
