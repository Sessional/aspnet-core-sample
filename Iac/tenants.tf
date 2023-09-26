resource "auth0_organization" "lonely_vale" {
  display_name = "Lonely Vale"
  name         = "lonely-vale"
}

resource "auth0_organization_connections" "lonely_vale" {
  organization_id = auth0_organization.lonely_vale.id
  enabled_connections {
    assign_membership_on_login = false
    connection_id              = auth0_connection.username_password_authentication.id
  }
}
