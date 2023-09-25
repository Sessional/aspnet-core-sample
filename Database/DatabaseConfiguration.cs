using System.ComponentModel.DataAnnotations;

namespace LonelyVale.Database;

public class DatabaseConfiguration
{
    public const string SECTION_KEY = "Database";

    [Required] public Dictionary<string, string> ConnectionStrings { get; set; } = new();
}