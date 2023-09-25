namespace LonelyVale.OAuth.Exceptions;

public class OAuthHttpException : Exception
{
    public string? HttpResponse { get; }
    public OAuthHttpException(string? message, string? HttpResponse) : base(message)
    {
        this.HttpResponse = HttpResponse;
    }
}