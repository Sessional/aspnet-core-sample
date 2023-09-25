
schema "public" {
  comment = "A public schema that has data core to running the system for everyone."
}

table "users" {
  schema = schema.public
  column "id" {
    type = bigserial
  }
  column "auth0_id" {
    type = text
  }
  primary_key {
    columns = [column.id]
  }
}

table "realms" {
  schema = schema.public

  column "id" {
    type = bigserial
  }

  column "realm_name" {
    type = text
  }

  column "auth0_org_id" {
    type = text
  }
  
  column "is_public" {
    type = boolean
  }
  
  column "schema_name" {
    type = text
  }

  primary_key {
    columns = [column.id]
  }
}

table "realm_membership" {
  schema = schema.public
  
  column "id" {
    type = bigserial
  }
  
  column "realm_id" {
    type = bigint
  }
  
  column "user_id" {
    type = bigint
  }

  foreign_key "user_id" {
    columns     = [column.user_id]
    ref_columns = [table.users.column.id]
    on_update   = NO_ACTION
    on_delete   = NO_ACTION
  }
  
  foreign_key "realm_id" {
    columns     = [column.realm_id]
    ref_columns = [table.realms.column.id]
    on_update   = NO_ACTION
    on_delete   = NO_ACTION
  }
}