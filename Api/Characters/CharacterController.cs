using Dapper;
using LonelyVale.Database;
using LonelyVale.Tenancy;
using Microsoft.AspNetCore.Mvc;

namespace LonelyVale.Api.Characters;

[ApiController]
public class CharacterController : ControllerBase
{
    private readonly ILogger<CharacterController> _logger;
    private readonly DatabaseContext _databaseContext;
    private readonly ITenantIdResolver _tenantResolver;

    public CharacterController(ILogger<CharacterController> logger, DatabaseContext databaseContext,
        ITenantIdResolver tenantResolver)
    {
        _logger = logger;
        _databaseContext = databaseContext;
        _tenantResolver = tenantResolver;
    }

    [HttpGet("characters/{characterId}", Name = "GetCharacter")]
    public async Task<GetCharacterContract> Get(long characterId)
    {
        using (var connection = _databaseContext.GetConnection("Primary"))
        {
            var tenant = _tenantResolver.GetTenantId();
            tenant = "tenant_1";
            var character = await connection.QueryAsync<CharacterEntity>(
                $"SELECT id as Id, user_id as UserId from \"{tenant}\".characters WHERE id=@characterId", new
                {
                    characterId = characterId
                });
            return new GetCharacterContract()
            {
                Id = character.Single().Id,
                UserId = character.Single().UserId
            };
        }
    }
}