schema "tenant_schema" {
  name    = "tenant_${var.tenant_id}"
  comment = "The schema that holds the data for ${var.tenant_id}"
}

table "characters" {
  schema = schema.tenant_schema
  column "id" {
    type = bigserial
  }
  
  column "user_id" {
    type = bigint
  }
  
  primary_key {
    columns = [column.id, column.user_id]
  }

  foreign_key "owner_id" {
    columns     = [column.user_id]
    ref_columns = [table.users.column.id]
    on_update   = NO_ACTION
    on_delete   = NO_ACTION
  }
}

table "galaxies" {
  schema = schema.tenant_schema
  column "id" {
    type = bigserial
  }
  
  column "realm_id" {
    type = bigint
  }
  
  column "name" {
    type = text
  }

  primary_key {
    columns = [column.id]
  }
  
  foreign_key "realm_id" {
    columns     = [column.realm_id]
    ref_columns = [table.realms.column.id]
    on_update   = NO_ACTION
    on_delete   = NO_ACTION
  }
}

table "planets" {
  schema = schema.tenant_schema
  column "id" {
    type = bigserial
  }
  
  column "galaxy_id" {
    type = bigint
  }

  column "name" {
    type = text
  }

  primary_key {
    columns = [column.id]
  }

  foreign_key "galaxy_id" {
    columns     = [column.galaxy_id]
    ref_columns = [table.galaxies.column.id]
    on_update   = NO_ACTION
    on_delete   = NO_ACTION
  }
}
