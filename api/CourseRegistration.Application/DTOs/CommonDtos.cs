namespace CourseRegistration.Application.DTOs;

/// <summary>
/// Generic paginated response for list endpoints
/// </summary>
/// <typeparam name="T">Type of items in the page</typeparam>
public class PagedResponseDto<T>
{
    /// <summary>
    /// Items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indicates if there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Indicates if there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
}

/// <summary>
/// Standard API response wrapper
/// </summary>
/// <typeparam name="T">Type of the response data</typeparam>
public class ApiResponseDto<T>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// List of errors (if any)
    /// </summary>
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static ApiResponseDto<T> SuccessResponse(T data, string message = "Operation completed successfully")
    {
        return new ApiResponseDto<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    public static ApiResponseDto<T> ErrorResponse(string message, IEnumerable<string>? errors = null)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? Enumerable.Empty<string>()
        };
    }
}