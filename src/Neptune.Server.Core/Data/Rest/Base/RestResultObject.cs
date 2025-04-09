using System.Net;
using System.Text.Json.Serialization;

namespace Neptune.Server.Core.Data.Rest.Base;

/// <summary>
/// Generic class for standardized REST API responses
/// </summary>
/// <typeparam name="T">The type of data returned</typeparam>
public class RestResultObject<T>
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    [JsonIgnore] // This property will not be serialized in the response
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Status code as integer for serialization in the response
    /// </summary>
    public int Status => (int)StatusCode;

    /// <summary>
    /// Descriptive message about the operation
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Data returned by the operation
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// Any errors that occurred during the operation
    /// </summary>
    public List<string> Errors { get; set; }

    /// <summary>
    /// Timestamp of the operation
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public RestResultObject()
    {
        Errors = new List<string>();
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a success result
    /// </summary>
    /// <param name="data">The data to return</param>
    /// <param name="message">Optional message</param>
    /// <param name="statusCode">HTTP status code (default is OK)</param>
    /// <returns>A RestResultObject with Success = true</returns>
    public static RestResultObject<T> CreateSuccess(
        T data, string message = "Operation completed successfully", HttpStatusCode statusCode = HttpStatusCode.OK
    )
    {
        return new RestResultObject<T>
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Creates a success result with no data (for void operations)
    /// </summary>
    /// <param name="message">Optional message</param>
    /// <param name="statusCode">HTTP status code (default is OK)</param>
    /// <returns>A RestResultObject with Success = true and default data</returns>
    public static RestResultObject<T> CreateSuccess(
        string message = "Operation completed successfully", HttpStatusCode statusCode = HttpStatusCode.OK
    )
    {
        return new RestResultObject<T>
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = default,
            Errors = null
        };
    }

    /// <summary>
    /// Creates an error result
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code (default is BadRequest)</param>
    /// <param name="errors">Optional list of errors</param>
    /// <returns>A RestResultObject with Success = false</returns>
    public static RestResultObject<T> CreateError(
        string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, List<string> errors = null
    )
    {
        var result = new RestResultObject<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        };

        if (errors != null)
        {
            result.Errors.AddRange(errors);
        }

        return result;
    }

    /// <summary>
    /// Creates an error result for a specific exception
    /// </summary>
    /// <param name="ex">The exception that occurred</param>
    /// <param name="message">Optional custom message (default uses exception message)</param>
    /// <param name="statusCode">HTTP status code (default is InternalServerError)</param>
    /// <returns>A RestResultObject with Success = false</returns>
    public static RestResultObject<T> CreateError(
        Exception ex, string message = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError
    )
    {
        return new RestResultObject<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message ?? "An error occurred: " + ex.Message,
            Errors = new List<string> { ex.Message }
        };
    }
}
