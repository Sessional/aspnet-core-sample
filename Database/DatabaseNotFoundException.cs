namespace LonelyVale.Database;

public class DatabaseNotFoundException : Exception
{
    public DatabaseNotFoundException(string message) : base(message)
    {
    }
}