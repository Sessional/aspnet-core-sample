using System.Security.Cryptography;
using System.Text;

namespace LonelyVale.Api.Tests.Utilities;

public static class Arbitrary
{
    private static readonly Random Random = new();
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string ArbitraryAuth0Id()
    {
        var builder = new StringBuilder("auth0|");

        builder.Append(ArbitraryString(14));

        return builder.ToString();
    }

    public static string ArbitraryOrgId()
    {
        var builder = new StringBuilder("org_");
        builder.Append(ArbitraryString(14));
        return builder.ToString();
    }

    public static string ArbitraryString(int length = 14)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < 14; i++) builder.Append(Chars[Random.Next(Chars.Length)]);
        return builder.ToString();
    }
}