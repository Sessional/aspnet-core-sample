resource "auth0_resource_server" "crimes_against_nature" {
  enforce_policies                                = true
  identifier                                      = "http://lonelyvale.com"
  name                                            = "Crimes Against Nature"
  signing_alg                                     = "RS256"
  skip_consent_for_verifiable_first_party_clients = true
  token_dialect                                   = "access_token_authz"
  token_lifetime                                  = 6000
  token_lifetime_for_web                          = 6000
}

resource "auth0_resource_server_scopes" "crimes_against_nature" {
  resource_server_identifier = auth0_resource_server.crimes_against_nature.identifier
  scopes {
    description = "Permits a user to create realms"
    name        = "realms:CreateRealm"
  }
  scopes {
    description = "Permits a user to describe a realm"
    name        = "realms:DescribeRealm"
  }
  scopes {
    description = "Permits a user to list available realms"
    name        = "realms:ListRealms"
  }
}
