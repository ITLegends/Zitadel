using Zitadel.ApiAccess.Examples;

// 1. Use a Service account PAT token to retrieve my user.  
await PatExamples.GetMyUser();

// 2. Use a Service account with client credentials (clientId, clientSecret) to retrieve my user.
await ClientCredentialsExamples.GetMyUser();

// 3. Use a Service account with a client assertion (JWT) signed with a locally provided private key to retrieve my user.
await ClientAssertionExamples.GetMyUserWithLocalKey();

// 4. Use a service account with a client assertion (JWT) signed by an Azure key vault to retrieve my user.
await ClientAssertionExamples.GetMyUserWithExternalKey();

// 5. Use multiple configured service accounts in the same application.
await MultipleCredentialsExample.GetMyUsers();
