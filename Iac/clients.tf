resource "auth0_client" "crimes_against_nature_test_application" {
  app_type                              = "non_interactive"
  custom_login_page_on                  = true
  grant_types                           = ["client_credentials"]
  name                                  = "Crimes Against Nature (Server)"
  refresh_token {
    expiration_type              = "non-expiring"
    idle_token_lifetime          = 2592000
    infinite_idle_token_lifetime = true
    infinite_token_lifetime      = true
    leeway                       = 0
    rotation_type                = "non-rotating"
    token_lifetime               = 31557600
  }
}

resource "auth0_client" "crimes_against_nature_web" {
  allowed_logout_urls                   = ["http://localhost:3000/"]
  app_type                              = "spa"
  callbacks                             = ["http://localhost:3000/"]
  grant_types                           = ["authorization_code", "implicit", "refresh_token"]
  name                                  = "Crimes Against Nature (Web)"
  organization_require_behavior         = "post_login_prompt"
  organization_usage                    = "require"
}

resource "auth0_connection_clients" "username_password_authentication" {
  connection_id   = auth0_connection.username_password_authentication.id
  enabled_clients = [auth0_client.crimes_against_nature_web.id]
}
