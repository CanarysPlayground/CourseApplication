using System.ComponentModel.DataAnnotations;

namespace CourseRegistration.Application.DTOs;

/// <summary>
/// DTO for creating a new instructor rating
/// </summary>
public class CreateInstructorRatingDto
{
    /// <summary>
    /// Course ID for which the instructor is being rated
    /// </summary>
    [Required]
    public Guid CourseId { get; set; }

    /// <summary>
    /// Student ID who is submitting the rating
    /// </summary>
    [Required]
    public Guid StudentId { get; set; }

    /// <summary>
    /// Rating value (1-5 scale)
    /// </summary>
    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    /// <summary>
    /// Optional comment/review text
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
    public string? Comment { get; set; }
}

/// <summary>
/// DTO for updating an existing instructor rating
/// </summary>
public class UpdateInstructorRatingDto
{
    /// <summary>
    /// Rating value (1-5 scale)
    /// </summary>
    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    /// <summary>
    /// Optional comment/review text
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
    public string? Comment { get; set; }
}

/// <summary>
/// DTO for returning instructor rating details
/// </summary>
public class InstructorRatingDto
{
    /// <summary>
    /// Unique identifier for the rating
    /// </summary>
    public Guid RatingId { get; set; }

    /// <summary>
    /// Course ID
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Course name
    /// </summary>
    public string CourseName { get; set; } = string.Empty;

    /// <summary>
    /// Instructor name
    /// </summary>
    public string InstructorName { get; set; } = string.Empty;

    /// <summary>
    /// Student ID who submitted the rating
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Student name
    /// </summary>
    public string StudentName { get; set; } = string.Empty;

    /// <summary>
    /// Rating value (1-5 scale)
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Optional comment/review text
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Date when the rating was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the rating was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for instructor rating statistics
/// </summary>
public class InstructorRatingStatsDto
{
    /// <summary>
    /// Instructor name
    /// </summary>
    public string InstructorName { get; set; } = string.Empty;

    /// <summary>
    /// Average rating
    /// </summary>
    public double AverageRating { get; set; }

    /// <summary>
    /// Total number of ratings
    /// </summary>
    public int TotalRatings { get; set; }

    /// <summary>
    /// Number of 5-star ratings
    /// </summary>
    public int FiveStarCount { get; set; }

    /// <summary>
    /// Number of 4-star ratings
    /// </summary>
    public int FourStarCount { get; set; }

    /// <summary>
    /// Number of 3-star ratings
    /// </summary>
    public int ThreeStarCount { get; set; }

    /// <summary>
    /// Number of 2-star ratings
    /// </summary>
    public int TwoStarCount { get; set; }

    /// <summary>
    /// Number of 1-star ratings
    /// </summary>
    public int OneStarCount { get; set; }

    /// <summary>
    /// List of courses taught by this instructor
    /// </summary>
    public List<Guid> CourseIds { get; set; } = new();
}
