namespace QLNhanVien.src.Common;

/// <summary>
/// Custom exception cho validation errors
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Custom exception cho business rule violations
/// </summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }

    public BusinessRuleException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Custom exception cho resource not found
/// </summary>
public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string message) : base(message) { }

    public ResourceNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
