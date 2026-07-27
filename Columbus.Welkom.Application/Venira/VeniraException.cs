namespace Columbus.Welkom.Application.Venira;

/// <summary>
/// Thrown when the Venira Paradox database cannot be read: the folder or a table is missing,
/// a table is unreadable, or the native pxlib library could not be loaded.
/// </summary>
public class VeniraException : Exception
{
    public VeniraException(string message) : base(message) { }

    public VeniraException(string message, Exception innerException) : base(message, innerException) { }
}
