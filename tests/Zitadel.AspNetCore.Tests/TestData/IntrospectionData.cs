namespace Zitadel.AspNetCore.Tests.TestData;

public static class IntrospectionData
{
    public const string KeysResponse = """
                                       {
                                         "keys" : [ {
                                           "use" : "sig",
                                           "kty" : "RSA",
                                           "kid" : "334331490633390330",
                                           "alg" : "RS256",
                                           "n" : "kvt1hZnI4qVOvnje6kaIZ7uZHClSD08VBubk-12YQBHLk6xOLEjOvNzvCsjSTBTS9OwUSJn5f48USXUNB0433TY5vvTUwy7sBplJB3ybMwR3dFDu1TbYzgjhmxhzZn7shNrjrPPtK3YrDK0zMDNF4_9Y1kmIym0JXMMkiGO5q1B2zklPyshJFOnlXC0LL-C90v5G09r5gk_W8_LbDWB2s3LgrV0JOVPeanwNY8SoTzVqSH8k3t6yMkC6DIh3pjyn4wOYvfnsCYLQywTzVBTsEP5Mg4iMB6Q3TBZIoIKDaEnGx_BTZJRgwfNJuQNXdUdAOP2UXK7M8gjj68EOUmDlNw",
                                           "e" : "AQAB"
                                         }, {
                                           "use" : "sig",
                                           "kty" : "RSA",
                                           "kid" : "334331490633390330",
                                           "alg" : "RS256",
                                           "n" : "12JbbKrWukVA1rgTu-6jQbv_Aqs7_jcoHYkk_-UtzyfPqAoIvhOAcVJ1PVfxTBY7tof5o84Zu5XnEM1gj45QJU4O_9VdzeJyPg5OjLJwSCKUoxKtTzRA2Ho6RJN3byNa-0NgOVhtSU95gIXRgrMlqBgpexoMRZSUkheQdu3qKUP82NBAukS9QXNaLD2N8LVajmlrTGGUxskL5JWBsHI62yeEGjhrJDwPqeUVrlNjyD584EEr3GuoAgaE4BguIo4wUUSY9Q758HrJ_gggy9DXvOC051kfBLycH3N1AV6alp9gxCC5EEEOZNiWUsjO9jn-yZSCWr-CkIR5yMFDRD_laQ",
                                           "e" : "AQAB"
                                         }]
                                       }
                                       """;

    public const string DiscoveryDocumentResponse = """
                                                    {
                                                      "issuer" : "https://zitadel.localhost",
                                                      "authorization_endpoint" : "https://zitadel.localhost/oauth/v2/authorize",
                                                      "token_endpoint" : "https://zitadel.localhost/oauth/v2/token",
                                                      "introspection_endpoint" : "https://zitadel.localhost/oauth/v2/introspect",
                                                      "userinfo_endpoint" : "https://zitadel.localhost/oidc/v1/userinfo",
                                                      "revocation_endpoint" : "https://zitadel.localhost/oauth/v2/revoke",
                                                      "end_session_endpoint" : "https://zitadel.localhost/oidc/v1/end_session",
                                                      "device_authorization_endpoint" : "https://zitadel.localhost/oauth/v2/device_authorization",
                                                      "jwks_uri" : "https://zitadel.localhost/oauth/v2/keys",
                                                      "scopes_supported" : [ "openid", "profile", "email", "phone", "address", "offline_access" ],
                                                      "response_types_supported" : [ "code", "id_token", "id_token token" ],
                                                      "response_modes_supported" : [ "query", "fragment", "form_post" ],
                                                      "grant_types_supported" : [ "authorization_code", "implicit", "refresh_token", "client_credentials", "urn:ietf:params:oauth:grant-type:jwt-bearer", "urn:ietf:params:oauth:grant-type:device_code" ],
                                                      "subject_types_supported" : [ "public" ],
                                                      "id_token_signing_alg_values_supported" : [ "EdDSA", "RS256", "RS384", "RS512", "ES256", "ES384", "ES512" ],
                                                      "request_object_signing_alg_values_supported" : [ "RS256" ],
                                                      "token_endpoint_auth_methods_supported" : [ "none", "client_secret_basic", "client_secret_post", "private_key_jwt" ],
                                                      "token_endpoint_auth_signing_alg_values_supported" : [ "RS256" ],
                                                      "revocation_endpoint_auth_methods_supported" : [ "none", "client_secret_basic", "client_secret_post", "private_key_jwt" ],
                                                      "revocation_endpoint_auth_signing_alg_values_supported" : [ "RS256" ],
                                                      "introspection_endpoint_auth_methods_supported" : [ "client_secret_basic", "private_key_jwt" ],
                                                      "introspection_endpoint_auth_signing_alg_values_supported" : [ "RS256" ],
                                                      "claims_supported" : [ "sub", "aud", "exp", "iat", "iss", "auth_time", "nonce", "acr", "amr", "c_hash", "at_hash", "act", "scopes", "client_id", "azp", "preferred_username", "name", "family_name", "given_name", "locale", "email", "email_verified", "phone_number", "phone_number_verified" ],
                                                      "code_challenge_methods_supported" : [ "S256" ],
                                                      "ui_locales_supported" : [ "nl", "en" ],
                                                      "request_parameter_supported" : true,
                                                      "request_uri_parameter_supported" : false,
                                                      "backchannel_logout_supported" : true,
                                                      "backchannel_logout_session_supported" : true
                                                    }
                                                    """;
}
