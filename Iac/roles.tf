resource "auth0_role" "moderator" {
  description = "A moderator role."
  name        = "Moderator"
}

resource "auth0_role_permissions" "moderator" {
  role_id = auth0_role.moderator.id
  permissions {
    name                       = "realms:CreateRealm"
    resource_server_identifier = auth0_resource_server.crimes_against_nature.identifier
  }
  permissions {
    name                       = "realms:DescribeRealm"
    resource_server_identifier = auth0_resource_server.crimes_against_nature.identifier
  }
  permissions {
    name                       = "realms:ListRealms"
    resource_server_identifier = auth0_resource_server.crimes_against_nature.identifier
  }
}

resource "auth0_role" "admin" {
  description = "An administrator"
  name        = "Admin"
}

resource "auth0_role_permissions" "admin" {
  role_id = auth0_role.admin.id
  permissions {
    name                       = "realms:CreateRealm"
    resource_server_identifier = auth0_resource_server.crimes_against_nature.identifier
  }
  permissions {
    name                       = "realms:DescribeRealm"
    resource_server_identifier = auth0_resource_server.crimes_against_nature.identifier
  }
  permissions {
    name                       = "realms:ListRealms"
    resource_server_identifier = auth0_resource_server.crimes_against_nature.identifier
  }
}
