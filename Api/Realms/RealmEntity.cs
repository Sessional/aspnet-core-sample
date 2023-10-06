namespace LonelyVale.Api.Realms;

public record RealmEntity(long? Id, string Name, string Auth0OrgId, bool IsPublic, string schemaName);