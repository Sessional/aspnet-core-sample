namespace LonelyVale.Api.Characters;

public record GetCharacterContract
{
    public long Id { get; set; }
    public long UserId { get; set; }
}