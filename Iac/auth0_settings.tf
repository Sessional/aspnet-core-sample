resource "auth0_tenant" "tenant" {
  allow_organization_name_in_authentication_api = false
  allowed_logout_urls                           = []
  enabled_locales                               = ["en"]
  sandbox_version                               = "16"
  session_cookie {
    mode = null
  }
  sessions {
    oidc_logout_prompt_enabled = false
  }
}

resource "auth0_prompt" "prompts" {
  identifier_first               = true
  universal_login_experience     = "new"
  webauthn_platform_first_factor = false
}

resource "auth0_connection" "username_password_authentication" {
  name     = "Username-Password-Authentication"
  realms   = ["Username-Password-Authentication"]
  strategy = "auth0"
  options {
    brute_force_protection = true
    password_policy        = "good"
    mfa {
      active                 = true
      return_enroll_settings = true
    }
  }
}
